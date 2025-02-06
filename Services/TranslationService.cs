using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoreBridge.Models;
using LoreBridge.Translation;

namespace LoreBridge.Services;

public class TranslationService(ITranslator translator, MessagesModel messages)
{
    private readonly ConcurrentQueue<MessageEntry> _taskQueue = new();
    private readonly object _processingLock = new();
    private bool _isProcessing;

    public void Add(MessageEntry message)
    {
        if (message.TimeStamp == 0) message.TimeStamp = (uint)DateTime.UtcNow.Ticks;

        _taskQueue.Enqueue(message);

        lock (_processingLock)
        {
            if (_isProcessing) return;

            _isProcessing = true;
            _ = ProcessQueueAsync();
        }
    }

    private async Task ProcessQueueAsync()
    {
        while (_isProcessing)
        {
            await Task.Delay(100);

            var tasksToProcess = new List<MessageEntry>();
            while (_taskQueue.TryDequeue(out var message)) tasksToProcess.Add(message);

            if (tasksToProcess.Count > 0)
            {
                tasksToProcess.Sort((x, y) => x.TimeStamp.CompareTo(y.TimeStamp));
                foreach (var message in tasksToProcess)
                    await ProcessTranslationAsync(message).ConfigureAwait(false);
            }

            lock (_processingLock)
            {
                if (!_taskQueue.IsEmpty) continue;

                _isProcessing = false;
                break;
            }
        }
    }

    private async Task ProcessTranslationAsync(MessageEntry message)
    {
        try
        {
            var translation = await translator.TranslateAsync(message.Text).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(translation))
            {
                message.Text = translation;
                messages.Add(message);
            }
        }
        catch (Exception e)
        {
            messages.Add(new MessageEntry { Text = e.Message });
        }
    }
}
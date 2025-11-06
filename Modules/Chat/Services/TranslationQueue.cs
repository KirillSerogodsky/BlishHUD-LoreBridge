using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoreBridge.Modules.Chat.Models;
using LoreBridge.Services;

namespace LoreBridge.Modules.Chat.Services;

public class TranslationQueue(Messages messages)
{
    private readonly ConcurrentQueue<Message> _taskQueue = new();
    private readonly object _processingLock = new();
    private bool _isProcessing;

    public void Add(Message message)
    {
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

            var tasksToProcess = new List<Message>();
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

    private async Task ProcessTranslationAsync(Message message)
    {
        try
        {
            var translation = await Service.Translation.TranslateAsync(message.Text).ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(translation))
            {
                message.Text = translation;
                messages.Add(message);
            }
        }
        catch (Exception e)
        {
            messages.Add(new Message { Text = e.Message });
        }
    }
}
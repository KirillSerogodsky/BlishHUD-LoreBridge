using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoreBridge.Models;
using LoreBridge.Translation;

namespace LoreBridge.Services;

public class TranslationService(ITranslator translator, MessagesModel messages)
{
    private readonly object _lock = new();
    private readonly List<MessageEntry> _taskList = [];
    private bool _isProcessing;

    public void Add(MessageEntry message)
    {
        lock (_lock)
        {
            var id = message.TimeStamp;

            if (id == 0) id = _taskList.Count > 0 ? _taskList.Max(t => t.TimeStamp) + 1 : 1;

            message.TimeStamp = id;
            _taskList.Add(message);

            if (_isProcessing) return;

            _isProcessing = true;
            _ = ProcessQueueAsync();
        }
    }

    private async Task ProcessQueueAsync()
    {
        while (_isProcessing)
        {
            List<MessageEntry> tasksToProcess;

            await Task.Delay(100);

            lock (_lock)
            {
                tasksToProcess = _taskList.ToList();
                _taskList.Clear();
            }

            if (tasksToProcess.Count > 0)
            {
                tasksToProcess.Sort((x, y) => x.TimeStamp.CompareTo(y.TimeStamp));
                foreach (var message in tasksToProcess)
                    await ProcessTranslationAsync(message).ConfigureAwait(false);
            }

            lock (_lock)
            {
                if (_taskList.Count != 0) continue;

                _isProcessing = false;
                break;
            }
        }
    }

    private async Task ProcessTranslationAsync(MessageEntry message)
    {
        try
        {
            var translation = await translator.TranslateAsync(message.Text);
            if (!string.IsNullOrWhiteSpace(translation))
            {
                message.Text = translation;
                messages.Add(message);
            }
        }
        catch (Exception e)
        {
            messages.Add(new MessageEntry
            {
                Text = e.Message
            });
        }
    }
}
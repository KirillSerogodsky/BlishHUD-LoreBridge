using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoreBridge.Models;
using LoreBridge.Translation;

namespace LoreBridge.Services;

public class TranslationService(ITranslator translator, TranslationListModel translationList)
{
    private readonly List<(string text, string name, int messageId)> _taskList = [];
    private readonly object _lock = new();
    private bool _isProcessing;
    
    public void Add(string text, string name = null, int? messageId = null)
    {
        lock (_lock)
        {
            var id = messageId ?? 0;

            if (!messageId.HasValue)
            {
                id = _taskList.Count > 0 ? _taskList.Max(t => t.messageId) + 1 : 1;
            }

            _taskList.Add((text, name, id));

            if (_isProcessing) return;

            _isProcessing = true;
            _ = ProcessQueueAsync();
        }
    }

    private async Task ProcessQueueAsync()
    {
        while (_isProcessing)
        {
            List<(string text, string name, int messageId)> tasksToProcess;

            lock (_lock)
            {
                tasksToProcess = _taskList.ToList();
                _taskList.Clear();
            }

            if (tasksToProcess.Count > 0)
            {
                tasksToProcess.Sort((x, y) => x.messageId.CompareTo(y.messageId));
                foreach (var taskData in tasksToProcess)
                {
                    await ProcessTranslationAsync(taskData.text, taskData.name).ConfigureAwait(false);
                }
            }

            await Task.Delay(100);

            lock (_lock)
            {
                if (_taskList.Count != 0) continue;
                
                _isProcessing = false;
                break;
            }
        }
    }

    private async Task ProcessTranslationAsync(string text, string name = null)
    {
        try
        {
            var translation = await translator.TranslateAsync(text);
            if (!string.IsNullOrWhiteSpace(translation))
            {
                translationList.Add(translation, name);
            }
        }
        catch (Exception e)
        {
            translationList.Add(e.Message, name);
        }
    }
}
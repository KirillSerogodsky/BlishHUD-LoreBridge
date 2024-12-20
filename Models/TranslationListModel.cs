using System;
using System.Collections.Generic;
using Blish_HUD;

namespace LoreBridge.Models
{
    public class TranslationListModel
    {
        private readonly List<string> TranslationsList = new();
        public List<string> Value
        {
            get
            {
                return TranslationsList;
            }
        }

        public event EventHandler<ValueChangedEventArgs<List<string>>> Added;
        public event EventHandler<ValueChangedEventArgs<List<string>>> Updated;
        public event EventHandler Cleared;

        public TranslationListModel()
        {
        }

        private void OnAdd(ValueChangedEventArgs<List<string>> e)
        {
            Added?.Invoke(this, e);
        }

        private void OnListUpdated(ValueChangedEventArgs<List<string>> e)
        {
            Updated?.Invoke(this, e);
        }

        public void Add(string text)
        {
            List<string> value2 = Value;
            TranslationsList.Add(text);
            OnAdd(new ValueChangedEventArgs<List<string>>(value2, TranslationsList));
            OnListUpdated(new ValueChangedEventArgs<List<string>>(value2, TranslationsList));
        }

        public void ClearAll()
        {
            TranslationsList.Clear();
            Cleared.Invoke(this, null);
        }
    }
}
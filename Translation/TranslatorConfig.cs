using LoreBridge.Enums;

namespace LoreBridge.Translation
{
    public class TranslatorConfig
    {
        private Languages _targetLang;
        private Translators _translator;

        public TranslatorConfig()
        {
        }

        public Languages TargetLang
        {
            get => _targetLang;
            set
            {
                _targetLang = value;
            }
        }
    }
}

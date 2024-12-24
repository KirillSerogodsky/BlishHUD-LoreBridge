using System;
using System.Threading.Tasks;

namespace LoreBridge.Translation;

public interface ITranslator : IDisposable
{
    Task<string> TranslateAsync(string text);
}
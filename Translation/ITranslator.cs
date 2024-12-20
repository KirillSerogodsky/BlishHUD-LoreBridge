using System.Threading.Tasks;

namespace LoreBridge.Translation
{
    public interface ITranslator
    {
        Task<string> TranslateAsync(string text);
    }
}

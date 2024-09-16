using _Game.Utils.Popups;
using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._AlertPopup
{
    public interface IAlertPopupProvider
    {
        UniTask<Disposable<AlertPopup>> Load();
    }
}
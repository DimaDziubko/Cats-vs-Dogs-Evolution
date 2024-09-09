using Assets._Game.Utils.Disposable;
using Assets._Game.Utils.Popups;
using Cysharp.Threading.Tasks;

namespace _Game.UI._AlertPopup
{
    public interface IAlertPopupProvider
    {
        UniTask<Disposable<AlertPopup>> Load();
    }
}
using Cysharp.Threading.Tasks;

namespace _Game.Utils.Popups
{
    public interface IAlertPopupProvider
    {
        UniTask<Disposable.Disposable<AlertPopup>> Load();
    }
}
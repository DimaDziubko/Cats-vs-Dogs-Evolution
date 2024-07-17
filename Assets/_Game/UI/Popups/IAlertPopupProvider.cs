using Cysharp.Threading.Tasks;

namespace Assets._Game.Utils.Popups
{
    public interface IAlertPopupProvider
    {
        UniTask<Disposable.Disposable<AlertPopup>> Load();
    }
}
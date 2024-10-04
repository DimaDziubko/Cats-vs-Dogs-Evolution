using Assets._Game.Utils.Disposable;
using Cysharp.Threading.Tasks;

namespace _Game.UI._CardsGeneral._Cards.Scripts
{
    public interface ICardAppearancePopupProvider
    {
        UniTask<Disposable<CardAppearancePopup>> Load();
    }
}
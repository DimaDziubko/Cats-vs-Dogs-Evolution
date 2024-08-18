namespace _Game.Core._GameListenerComposite
{
    public interface IFoodListener
    {
        void OnFoodBalanceChanged(int value);
        void OnFoodGenerated();
    }
}
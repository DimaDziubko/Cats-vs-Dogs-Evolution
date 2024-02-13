namespace _Game.Core.Services.Evolution.Scripts
{
    public interface IEvolutionService
    {
        void MoveToNextAge();
        int GetTimelineNumber();
        bool IsNextAgeAvailable();
        float GetEvolutionPrice();
    }
}
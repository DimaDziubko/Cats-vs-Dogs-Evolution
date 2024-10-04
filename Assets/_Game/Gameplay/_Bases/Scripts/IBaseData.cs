namespace _Game.Gameplay._Bases.Scripts
{
    public interface IBaseData
    {
        public Base BasePrefab { get; }
        public float CoinsAmount { get; }
        public int Layer { get; }
        float Health { get; }
    }
}
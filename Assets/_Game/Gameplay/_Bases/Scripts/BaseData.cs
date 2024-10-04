namespace _Game.Gameplay._Bases.Scripts
{
    public class BaseData : IBaseData
    {
        public Base BasePrefab => _staticData.BasePrefab;
        public float CoinsAmount => _staticData.CoinsAmount;
        public int Layer => _staticData.Layer;
        public float Health { get; set; }
        
        private readonly BaseStaticData _staticData;

        public BaseData(BaseStaticData staticData)
        {
            _staticData = staticData;
        }
    }
}
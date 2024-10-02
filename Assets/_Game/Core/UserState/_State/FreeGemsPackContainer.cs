using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace _Game.Core.UserState._State
{
    public class FreeGemsPackContainer : IFreeGemsPackContainer
    {
        [ShowInInspector]
        private readonly Dictionary<int, FreeGemsPackState> _freeGemsPacks = new Dictionary<int, FreeGemsPackState>();

        public IReadOnlyDictionary<int, FreeGemsPackState> FreeGemsPacks => _freeGemsPacks;

        public void AddPack(int packId, FreeGemsPackState pack)
        {
            if (_freeGemsPacks.ContainsKey(packId))
            {
                _freeGemsPacks[packId] = pack;
            }
            else
            {
                _freeGemsPacks.Add(packId, pack);
            }
        }

        public bool TryGetPack(int packId, out FreeGemsPackState pack)
        {
            return _freeGemsPacks.TryGetValue(packId, out pack);
        }

        public void RemovePack(int packId)
        {
            if (_freeGemsPacks.ContainsKey(packId))
            {
                _freeGemsPacks.Remove(packId);
            }
        }

        public void ClearAllPacks()
        {
            _freeGemsPacks.Clear();
        }
    }
}
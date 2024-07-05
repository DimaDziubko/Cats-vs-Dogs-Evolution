using Assets._Game.Utils;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets._Game.Gameplay._Units.Scripts
{
    public class DynamicSortingOrder : MonoBehaviour
    {
        [SerializeField] private Transform _sortingPoint;
        [SerializeField] private SortingGroup _sortingGroup;

        private Vector3 Position => _sortingPoint.position;
        
        
        public void GameUpdate()
        {
            float sortingOrder = -Position.y / Constants.SortingLayer.SORTING_TRESHOLD;

            int newOrder = Mathf.RoundToInt(sortingOrder);
            
            newOrder = Mathf.Clamp( newOrder, 
                Constants.SortingLayer.SORTING_ORDER_MIN, 
                Constants.SortingLayer.SORTING_ORDER_MAX);

            
            if (newOrder != _sortingGroup.sortingOrder)
            {
                _sortingGroup.sortingOrder = newOrder;
            }
        }
        
#if UNITY_EDITOR
        //Helper
        [Button]
        public void ManualInit()
        {
            TryGetComponent(out _sortingGroup);
        }
#endif
    }
}
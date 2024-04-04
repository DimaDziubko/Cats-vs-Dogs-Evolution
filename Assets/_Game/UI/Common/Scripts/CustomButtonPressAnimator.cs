using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    [RequireComponent(typeof(Button))]
    public class CustomButtonPressAnimator : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private RectTransform _buttonTransform;
        [SerializeField] private RectTransform[] _animatableObjects;
        
        private Vector3[] _originalPositions;
        
        [FormerlySerializedAs("pressOffsetFactor")] 
        [SerializeField, Tooltip("Fraction of the object's height by which it will shift when pressed.")]
        private float _pressOffsetFactor = 5f;

        private Button _button;
        
        private void Awake()
        {

            _button = GetComponent<Button>();
            
            _originalPositions = new Vector3[_animatableObjects.Length];
            for (int i = 0; i < _animatableObjects.Length; i++)
            {
                if (_animatableObjects[i] != null)
                    _originalPositions[i] = _animatableObjects[i].anchoredPosition;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if(_button.interactable == false) return;

            foreach (var rectTransform in _animatableObjects)
            {
                var initialPosition = rectTransform.anchoredPosition;
                
                if (rectTransform != null)
                    rectTransform.anchoredPosition = new Vector3(initialPosition.x,  (initialPosition.y -(_buttonTransform.sizeDelta.y / _pressOffsetFactor)), 0);
            }
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            for (int i = 0; i < _animatableObjects.Length; i++)
            {
                if (_animatableObjects[i] != null)
                    _animatableObjects[i].anchoredPosition = _originalPositions[i];
            }
        }
        
#if UNITY_EDITOR
        //Helper
        [Button]
        private void ManualInit()
        {
            _buttonTransform = GetComponent<RectTransform>();
            _animatableObjects = GetComponentsInChildren<RectTransform>();
        }
#endif
    }
}
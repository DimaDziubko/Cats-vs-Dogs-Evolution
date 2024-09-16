using UnityEngine;

namespace _Game.Gameplay._Tutorial.Scripts
{
    public class TutorialPointersParent : MonoBehaviour
    {
        [SerializeField] private RectTransform _rectTransform;
        public RectTransform RectTransform => _rectTransform;    
        public void Enable()
        {
            gameObject.SetActive(true);
        }

        public void Disable()
        {
            gameObject.SetActive(false);
        }
    }
}
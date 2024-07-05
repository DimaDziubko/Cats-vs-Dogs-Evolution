using UnityEngine;

namespace Assets._Game.UI.Pin.Scripts
{
    public class PinView : MonoBehaviour
    {
        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}

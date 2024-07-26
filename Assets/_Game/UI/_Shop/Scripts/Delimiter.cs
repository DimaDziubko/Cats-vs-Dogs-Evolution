using UnityEngine;

namespace _Game.UI._Shop.Scripts
{
    public class Delimiter : MonoBehaviour
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

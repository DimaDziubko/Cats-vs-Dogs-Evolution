using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI.Common.Scripts
{
    public class LivesUI : MonoBehaviour
    {
        [SerializeField] private Image[] _livesImages;

        public void UpdateLives(int currentLives, int maxLives)
        {
            if (_livesImages.Length != maxLives)
            {
                Debug.LogError("Wrong lives view");
            }

            foreach (var livesImage in _livesImages)
            {
                livesImage.enabled = false;
            }

            for (int i = 0; i < currentLives ; i++)
            {
                _livesImages[i].enabled = true;
            }
        }
    }
}
using System.Collections;
using TMPro;
using UnityEngine;

namespace Assets._Game.UI.Common.Scripts
{
    public class PlayerLivesView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
        [SerializeField] private float _scaleDuration;
        [SerializeField] private float _targetScale;

        private bool _isAnimating;

        public void UpdateLives(int newLivesCount)
        {
            if (!_isAnimating)
            {
                StartCoroutine(ScaleText(newLivesCount));
            }
        }

        private IEnumerator ScaleText(int newLivesCount)
        {
            _isAnimating = true;

            float elapsedTime = 0f;
            Vector3 startScale = Vector3.one;
            Vector3 targetScaleVector = new Vector3(_targetScale, _targetScale, 1f);

            while (elapsedTime < _scaleDuration / 2)
            {
                _scoreText.transform.localScale =
                    Vector3.Lerp(startScale, targetScaleVector, elapsedTime / _scaleDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _scoreText.transform.localScale = targetScaleVector;

            elapsedTime = 0f;
            while (elapsedTime < _scaleDuration / 2)
            {
                _scoreText.transform.localScale =
                    Vector3.Lerp(targetScaleVector, startScale, elapsedTime / _scaleDuration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            _scoreText.transform.localScale = startScale;

            _isAnimating = false;

            _scoreText.text = newLivesCount.ToString();
        }
        
    }
}
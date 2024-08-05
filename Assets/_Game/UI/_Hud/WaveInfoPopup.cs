using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Hud
{
    public class WaveInfoPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private Image _image;
        [SerializeField] private Sprite _allWavesSprite;
        [SerializeField] private Sprite _finalWaveSprite;
        [SerializeField] private WaveInfoPopupAnimation _animation;

        public void ShowWave(int wave, int wavesCount)
        {
            if (_label == null) return;
            if (_image == null) return;
            if (wave < wavesCount - 1)
            {
                _label.text = $"Wave {wave}";
                //_label.color = Color.white;
                _image.sprite = _allWavesSprite;
            }
            else
            {
                _label.text = $"Final wave";
                _image.sprite = _finalWaveSprite;
                //_label.color = Color.red;
            }
            if (_animation == null) return;
            _animation.PlayAnimation();
        }

        public void HideWave() => _animation.StopAnimation();
    }
}

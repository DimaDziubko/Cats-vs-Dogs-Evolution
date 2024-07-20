using TMPro;
using UnityEngine;

namespace _Game.UI._Hud
{
    public class WaveInfoPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private WaveInfoPopupAnimation _animation;

        public void ShowWave(int wave, int wavesCount)
        {
            if(_label == null) return;
            if(wave < wavesCount - 1)
                _label.text = $"Wave {wave}";
            else
            {
                _label.text = $"Final wave";
            }
            if(_animation == null) return;
            _animation.PlayAnimation();
        }
    }
}

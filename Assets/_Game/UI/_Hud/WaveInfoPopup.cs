using TMPro;
using UnityEngine;

namespace _Game.UI._Hud
{
    public class WaveInfoPopup : MonoBehaviour
    {
        [SerializeField] private TMP_Text _label;
        [SerializeField] private WaveInfoPopupAnimation _animation;

        public void ShowWave(int wave)
        {
            if(_label == null) return;
            _label.text = $"Wave {wave}";
            if(_animation == null) return;
            _animation.PlayAnimation();
        }
    }
}

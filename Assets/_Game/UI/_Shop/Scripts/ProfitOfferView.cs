using _Game.UI._BoostPopup._Game.UI._BoostPopup;
using _Game.UI.Common.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.UI._Shop.Scripts
{
    public class ProfitOfferView : ShopItemView
    {
        [SerializeField] private RectTransform _transform;
        
        [Header("Inactive view")]
        [SerializeField] private GameObject _inactivePanel;
        [SerializeField] private Image _majorIconHolder;
        [SerializeField] private Image _inactiveMinorIconHolder;
        [SerializeField] private TMP_Text _inactiveNameLabel;
        [SerializeField] private TMP_Text _inactiveDescriptionLabel;
        [SerializeField] private TMP_Text _valueLabel;
        [SerializeField] private TransactionButton _button;
        [SerializeField] private AmountListView _rewardElements;
        [SerializeField] private float _inactiveHeight;
        
        [Space]
        [Header("Active view")]
        [SerializeField] private GameObject _activePanel;
        [SerializeField] private TMP_Text _activeNameLabel;
        [SerializeField] private TMP_Text _activeDescriptionLabel;
        [SerializeField] private Image _activeMinorIconHolder;
        [SerializeField] private float _activeHeight;

        [SerializeField] private FadeAnimation _fadeAnimation;

        public TransactionButton Button => _button;

        public void Init() => _button.Init();

        public void Cleanup()
        {
            ClearResourceElements();
            _button.Cleanup();
            _fadeAnimation.Cleanup();
        }

        public void SetMajorIcon(Sprite sprite) => 
            _majorIconHolder.sprite = sprite;

        public void SetDescription(string description)
        {
            _inactiveDescriptionLabel.text = description;
            _activeDescriptionLabel.text = description;
        }
        
        public void SetName(string name)
        {
            _inactiveNameLabel.text = name;
            _activeNameLabel.text = name;
        }

        public void SetValue(string value) => 
            _valueLabel.text = value;

        public void SetActive(bool isActive, bool showAnimation = false)
        {
            if (showAnimation)
            {
                _fadeAnimation.Play(2,() => ChangeState(isActive));
            }
            else
            {
                ChangeState(isActive);
            }
        }

        private void ChangeState(bool isActive)
        {
            _activePanel.SetActive(isActive);
            _inactivePanel.SetActive(!isActive);

            if (isActive)
            {
                _transform.sizeDelta = new Vector2(_transform.sizeDelta.x, _activeHeight);
            }
            else
            {
                _transform.sizeDelta = new Vector2(_transform.sizeDelta.x, _inactiveHeight);  
            }
        }
        
        public AmountView SpawnResourceElement()
        {
            return _rewardElements.SpawnElement();
        }

        public void ClearResourceElements()
        {
            _rewardElements.Clear();
        }

        public void SetMinorIcon(Sprite minorIcon)
        {
            _inactiveMinorIconHolder.sprite = minorIcon;
            _activeMinorIconHolder.sprite = minorIcon;
        }
    }
}
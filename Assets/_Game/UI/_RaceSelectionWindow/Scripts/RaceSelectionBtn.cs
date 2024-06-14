using System;
using _Game.Gameplay.Common.Scripts;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class RaceSelectionBtn : MonoBehaviour
{
    [SerializeField] private Sprite _offSprite;
    [SerializeField] private Sprite _onSprite;
    [SerializeField] private Image _changableImage;
    [SerializeField] private Button _button;

    public bool IsOn { get; private set; }

    private Action<Race> _callback;
    private Race _race;
    
    public void Init(Race race, Action<Race> callback, bool isOn)
    {
        IsOn = isOn;
        _race = race;
        _callback = callback;
        _button.onClick.AddListener(OnButtonClicked);
        UpdateVisual();
    }

    private void OnButtonClicked()
    {
        IsOn = true;
        UpdateVisual();
        _callback?.Invoke(_race);
    }

    private void UpdateVisual()
    {
        _changableImage.sprite = IsOn ? _onSprite : _offSprite;
    }

    public void SetState(bool isOn)
    {
        IsOn = isOn;
        UpdateVisual();
    }

    public void Cleanup()
    {
        _button.onClick.RemoveAllListeners();
    }
}

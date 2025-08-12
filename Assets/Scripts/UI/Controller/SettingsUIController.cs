using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.UI;

public class SettingsUIController : UIControllerBase
{
    [SerializeField] private CanvasGroup body;
    [SerializeField] private Button[] closeButtons;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button exitButton;

    protected override void SetUp()
    {
        foreach (var closeButton in closeButtons)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => UIManager.Instance().HideController<SettingsUIController>());
        }

        bgmSlider.SetValueWithoutNotify(SoundManager.Instance().GetVolume(SoundType.BGM));
        sfxSlider.SetValueWithoutNotify(SoundManager.Instance().GetVolume(SoundType.SFX));

        bgmSlider.onValueChanged.RemoveAllListeners();
        bgmSlider.onValueChanged.AddListener(volume => SoundManager.Instance().SetVolume(SoundType.BGM, volume));

        sfxSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.AddListener(volume => SoundManager.Instance().SetVolume(SoundType.SFX, volume));
        
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(() => GameManager.Instance().ExitGame());
        
        UIManager.Instance().RegisterController(this);
        gameObject.SetActive(false);
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        UIManager.Instance().FadeIn(body, 0.1f, "bigger 0.8 1.0");
    }

    public override void Hide()
    {
        UIManager.Instance().FadeOut(body, 0.1f, "smaller 1.0 0.8");
        Invoke(nameof(CloseSetting), 0.11f);
    }

    private void CloseSetting()
    {
        gameObject.SetActive(false);
    }
}

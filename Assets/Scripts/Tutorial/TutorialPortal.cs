using System;
using System.Collections;
using Michsky.UI.Dark;
using UnityEngine;

public class TutorialPortal : PortalObject
{
    [SerializeField] private UIDissolveEffect tutorialPortalUI;
    [SerializeField] private ButtonManager tutorialPortalButton;

    protected override void Init()
    {
        base.Init();
        tutorialPortalButton.clickEvent.RemoveAllListeners();
        tutorialPortalButton.clickEvent.AddListener(() => tutorialPortalUI.DissolveOut());
        tutorialPortalButton.clickEvent.AddListener(() => SceneLoader.Instance().LoadScene(GameConstants.Scene.VILLAGE_SCENE));
        detector.Detected.Changed = DetectPlayer;
    }

    private void Start()
    {
        tutorialPortalButton.soundSource = SoundManager.Instance().SFXSource;
    }

    public override void OnInteract()
    {
        PlayerManager.Instance().FullHeal();
        tutorialPortalUI.gameObject.SetActive(true);
        tutorialPortalUI.DissolveIn();
    }

    protected override void DetectPlayer(bool detect)
    {
        base.DetectPlayer(detect);
        if (detect)
        {
            UIManager.Instance().GetController<TutorialUI>().TryTutorial(TutorialType.Exploration_Portal);
        }
    }
}

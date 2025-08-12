using Michsky.UI.Dark;
using UnityEngine;

public class FadeUIController : UIControllerBase
{
    private UIDissolveEffect backGround;
    private bool fadeIn;

    private void Reset()
    {
        Init();
    }

    private void Awake()
    {
        if (backGround == null)
        {
            Init();
        }

        backGround.location = 1f;
        backGround.gameObject.SetActive(false);
    }

    protected override void SetUp()
    {
        UIManager.Instance().RegisterController(this);
    }

    private void Init()
    {
        backGround = transform.FindComponent<UIDissolveEffect>("Background");
    }

    public override void Show()
    {
        FadeIn();
    }

    public override void Hide()
    {
        FadeOut();
    }

    private void FadeIn()
    {
        fadeIn = true;
        SwitchActive();
        backGround.DissolveIn();
    }

    private void FadeOut()
    {
        fadeIn = false;
        backGround.DissolveOut();
        Invoke(nameof(SwitchActive), 1f);
    }

    private void SwitchActive()
    {
        backGround.gameObject.SetActive(fadeIn);
    }
}

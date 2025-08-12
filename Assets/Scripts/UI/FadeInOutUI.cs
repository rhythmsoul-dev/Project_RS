using Michsky.UI.Dark;
using UnityEngine;

public class FadeInOutUI : MonoBehaviour
{
    public UIDissolveEffect FadeEffect { get; private set; }

    private void Reset()
    {
        OnReset();
    }

    private void Awake()
    {
        if (FadeEffect == null)
        {
            OnReset();
        }

        Init();
    }

    //private void Start()
    //{
    //    UIManager.Instance().RegisterController(this);
    //}

    private void OnReset()
    {
        FadeEffect = TransformExtensions.FindComponent<UIDissolveEffect>(transform, "Background");
    }

    public void Init()
    {
        FadeEffect.location = 1f;
        FadeEffect.gameObject.SetActive(false);

        FadeEffect.DissolveInOver = null; 
        
        FadeEffect.DissolveOutOver = () =>
        {
            FadeEffect.gameObject.SetActive(false);
        };
    }

    public void FadeIn()
    {
        FadeEffect.gameObject.SetActive(true);
        FadeEffect.DissolveIn();
    }

    public void FadeOut()
    {
        FadeEffect.DissolveOut();
    }
}

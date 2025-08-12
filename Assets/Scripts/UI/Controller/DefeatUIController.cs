using Michsky.UI.Dark;
using UnityEngine;

public class DefeatUIController : UIControllerBase
{
    private CanvasGroup canvasGroup;
    private UIDissolveEffect backGround;
    private UIDissolveEffect content;
    private ButtonManager toTownButton;
    [SerializeField] private RectTransform rectTransform;

    private void Reset()
    {
        Init();
    }

    private void Awake()
    {
        if (backGround == null || content == null || toTownButton == null)
        {
            Init();
        }
        
        toTownButton.clickEvent.RemoveAllListeners();
        toTownButton.clickEvent.AddListener(() => PlayerManager.Instance().FullHeal());
        toTownButton.clickEvent.AddListener(() => PortalManager.Instance().MoveToPortal());
        toTownButton.clickEvent.AddListener(Hide);

        backGround.location = 1f;
        content.location = 1f;
        canvasGroup.interactable = false;
        backGround.gameObject.SetActive(false);

        backGround.DissolveInOver = () =>
        {
            canvasGroup.interactable = true;
            content.DissolveIn();
        };
        backGround.DissolveOutOver = () =>
        {
            backGround.gameObject.SetActive(false);
        };

        content.DissolveOutOver = () =>
        {
            canvasGroup.interactable = false;
            backGround.DissolveOut();
        };
    }

    protected override void SetUp()
    {
        UIManager.Instance().RegisterController(this);
        toTownButton.soundSource = SoundManager.Instance().SFXSource;
    }

    private void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        backGround = TransformExtensions.FindComponent<UIDissolveEffect>(transform, "Background");
        content = TransformExtensions.FindComponent<UIDissolveEffect>(transform, "Content");
        toTownButton = TransformExtensions.FindComponent<ButtonManager>(transform, "ToTown");
    }

    public override void Show()
    {
        GameManager.Instance().PauseGame();
        backGround.gameObject.SetActive(true);
        rectTransform.SetAsLastSibling();
        backGround.DissolveIn();
    }

    public override void Hide()
    {
        GameManager.Instance().ResumeGame();
        content.DissolveOut();
    }
}

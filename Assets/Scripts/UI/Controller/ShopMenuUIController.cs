using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ShopMenuUIController : UIControllerBase
{
    [SerializeField] private GameObject shopMenuUI;
    [SerializeField] private Button shopStallButton;
    [SerializeField] private Button conversationButton;
    [SerializeField] private Button exitButton;
    [SerializeField] private GameObject fog;

    protected override void SetUp()
    {
        shopStallButton.onClick.RemoveAllListeners();
        shopStallButton.onClick.AddListener(OpenShopStallUI);
        
        conversationButton.onClick.RemoveAllListeners();
        conversationButton.onClick.AddListener(OpenConversationUI);
        
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(Exit);
    }

    public override void Show()
    {
        fog.SetActive(true);
        shopMenuUI.SetActive(true);
    }

    public override void Hide()
    {
        fog.SetActive(false);
        shopMenuUI.SetActive(false);
    }

    private void OpenShopStallUI()
    {
        Hide();
        fog.SetActive(false);
        UIManager.Instance().ShowController<ShopStallUIController>();
    }

    private void OpenConversationUI()
    {
        Hide();
        fog.SetActive(false);
        UIManager.Instance().ShowController<ConversationUIController>();
    }

    private void Exit()
    {
        SceneLoader.Instance().LoadScene(GameConstants.Scene.VILLAGE_SCENE);
    }
}

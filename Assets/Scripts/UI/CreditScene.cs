using System.Collections;
using Michsky.UI.Dark;
using UnityEngine;

public class CreditScene : MonoBehaviour
{
    [Header("컴포넌트들")]
    [SerializeField] private RectTransform creditPanelRect;
    [SerializeField] private float creditPanelSizeY;
    [SerializeField] private UIDissolveEffect gameLogo;
    [SerializeField] private ModalWindowManager endOfCredit;
    [SerializeField] private ButtonManager backToTownButton;
    [SerializeField] private ButtonManager exitButton;
    
    [Header("스크롤 속도 조절")]
    [SerializeField] private float defaultSpeed = 100f;
    [SerializeField] private float speedUp = 500f;

    [Header("연출 딜레이")]
    [SerializeField] private float scrollStartDelay = 2f;
    [SerializeField] private float scrollEndDelay = 2f;
    
    private bool dissolveInOver;
    private float scrollSpeed;

    private void Reset()
    {
        Init();
    }

    private void Awake()
    {
        if (creditPanelSizeY != creditPanelRect.sizeDelta.y)
        {
            Init();
        } 
        
        backToTownButton.clickEvent.RemoveAllListeners();
        backToTownButton.clickEvent.AddListener(() => PortalManager.Instance().MoveToPortal());
        
        exitButton.clickEvent.RemoveAllListeners();
        exitButton.clickEvent.AddListener(() => GameManager.Instance().ExitGame());

        gameLogo.location = 1f;
        gameLogo.DissolveInOver = (() => dissolveInOver = true);
    }

    private void Start()
    {
        backToTownButton.soundSource = SoundManager.Instance().SFXSource;
        exitButton.soundSource = SoundManager.Instance().SFXSource;
        
        StartCoroutine(MoveCredit());
    }

    private void Init()
    {
        creditPanelRect = transform.FindComponent<RectTransform>("CreditPanel");
        creditPanelSizeY = creditPanelRect.sizeDelta.y;
        
        gameLogo = transform.FindComponent<UIDissolveEffect>("GameLogo");

        endOfCredit = transform.FindComponent<ModalWindowManager>("EndOfCredit");
        backToTownButton = transform.FindComponent<ButtonManager>("BackToTownButton");
        exitButton = transform.FindComponent<ButtonManager>("ExitButton");
    }

    private void Update()
    {
        scrollSpeed = Input.anyKey ? speedUp : defaultSpeed;
    }

    private IEnumerator MoveCredit()
    {
        yield return new WaitUntil(() => !SceneLoader.Instance().IsLoading);
        yield return new WaitForSeconds(0.4f);
        
        gameLogo.DissolveIn();
        
        yield return new WaitUntil(() => dissolveInOver);

        yield return new WaitForSeconds(scrollStartDelay);
        
        while (true)
        {
            creditPanelRect.position += Vector3.up * (scrollSpeed * Time.deltaTime);

            if (creditPanelRect.position.y >= creditPanelSizeY)
            {
                Vector3 targetVector = new Vector3(creditPanelRect.position.x, creditPanelSizeY, creditPanelRect.position.z);
                creditPanelRect.position = targetVector;

                break;
            }
            
            yield return null;
        }

        yield return new WaitForSeconds(scrollEndDelay);

        PlayerManager.Instance().LocalContext.Dialogues["Credit"] = true;
        
        endOfCredit.ModalWindowIn();
    }
}

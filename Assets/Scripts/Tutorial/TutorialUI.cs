using System;
using System.Collections.Generic;
using Michsky.UI.Dark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialUI : UIControllerBase
{
    [Header("캔버스 그룹")]
    [SerializeField] private CanvasGroup canvasGroup;
    
    [Header("연출용")]
    [SerializeField] private UIDissolveEffect backGround;
    [SerializeField] private UIDissolveEffect imageBackGround;
    [SerializeField] private UIDissolveEffect textBackGround;
    [SerializeField] private UIDissolveEffect buttonBackGround;
    
    [Header("표시요소")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI guideText;
    [SerializeField] private Image guideImage;

    [Header("확인 버튼")]
    [SerializeField] private ButtonManager confirmButton;
    
    [Header("여러종류일때")]
    [SerializeField] private GameObject indexButtons;
    [SerializeField] private TextMeshProUGUI indexText;
    [SerializeField] private Button nextButton;
    [SerializeField] private Button previousButton;
    
    [Header("데이터")]
    [SerializeField] private TutorialData tutorialData;
    private Tutorial curTutorial;
    private Dictionary<TutorialType, Tutorial> tutorialDic = new Dictionary<TutorialType, Tutorial>();
    
    private bool openTutorial;
    private bool switchingIndex;
    
    private void Reset()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        
        confirmButton = transform.FindComponent<ButtonManager>("MainButton");
        buttonBackGround = confirmButton.transform.FindComponent<UIDissolveEffect>("Background");
        
        backGround = transform.FindComponent<UIDissolveEffect>("Background");
        imageBackGround = transform.FindComponent<UIDissolveEffect>("ImageBackground");
        textBackGround = transform.FindComponent<UIDissolveEffect>("TextBackground");
        
        titleText = transform.FindComponent<TextMeshProUGUI>("TitleText");
        guideText = transform.FindComponent<TextMeshProUGUI>("GuideText");
        guideImage = transform.FindComponent<Image>("GuideImage");
        
        indexButtons = transform.FindComponent<Transform>("IndexButtons").gameObject;
        indexText = indexButtons.transform.FindComponent<TextMeshProUGUI>("IndexText");
        nextButton = indexButtons.transform.FindComponent<Button>("NextButton");
        previousButton = indexButtons.transform.FindComponent<Button>("PreviousButton");
    }

    private void Init()
    {
        canvasGroup.interactable = false;
        
        backGround.DissolveInOver = () =>
        {
            canvasGroup.interactable = true;
            imageBackGround.DissolveIn();
            textBackGround.DissolveIn();
            buttonBackGround.DissolveIn();
        };
        buttonBackGround.DissolveInOver = () =>
        {
            openTutorial = true;
        };

        buttonBackGround.DissolveOutOver = () =>
        {
            openTutorial = false;
            backGround.DissolveOut();
        };
        backGround.DissolveOutOver = () =>
        {
            backGround.gameObject.SetActive(false);
        };

        tutorialDic.Clear();
        foreach (Tutorial tutorial in tutorialData.tutorials)
        {
            tutorialDic[tutorial.Type] = tutorial;
        }
                
        backGround.location = 1f;
        imageBackGround.location = 1f;
        textBackGround.location = 1f;
        buttonBackGround.location = 1f;
        
        confirmButton.clickEvent.RemoveAllListeners();
        confirmButton.clickEvent.AddListener(CloseTutorial);
        
        nextButton.onClick.RemoveAllListeners();
        previousButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextTutorial);
        previousButton.onClick.AddListener(PreviousTutorial);
        
        backGround.gameObject.SetActive(false);
        
        indexButtons.SetActive(false);
    }

    private void Awake()
    {
        Init();
    }

    protected override void SetUp()
    {
        UIManager.Instance().RegisterController(this);
        confirmButton.soundSource = SoundManager.Instance().SFXSource;
    }

    public override void Show()
    {
        throw new NotImplementedException();
    }

    public override void Hide()
    {
        throw new NotImplementedException();
    }

    public void TryTutorial(TutorialType type)
    {
        if (!switchingIndex && TutorialSystem.Instance().IsCompleted(type))
        {
            return;
        }

        if (!tutorialDic.TryGetValue(type, out curTutorial))
        {
            Debug.LogError($"튜토리얼 타입 {type}을 찾을 수 없습니다.");
            return;
        }
        
        TutorialSystem.Instance().AddCompleted(type);
        TutorialSystem.Instance().OnContextChanged();

        Tutorial();
    }

    private void Tutorial()
    {
        switchingIndex = false;

        if (!GameManager.Instance().IsGamePaused)
        { 
            GameManager.Instance().PauseGame();
        }

        titleText.text = curTutorial.TutorialName;
        guideText.text = curTutorial.TutorialDescription;
        if (curTutorial.TutorialImage != null)
        {
            guideImage.sprite = curTutorial.TutorialImage;
        }
        else
        {
            guideImage.sprite = null;
            Debug.Log($"{curTutorial.TutorialName}의 튜토리얼 이미지 없음");
        }

        textBackGround.DissolveOutOver = null;
        indexButtons.SetActive(curTutorial.HasIndex);

        if (curTutorial.HasIndex)
        {
            indexText.text = $"< {curTutorial.TutorialIndex + 1} / {curTutorial.LastIndex + 1} >";
            if (curTutorial.TutorialIndex == 0)
            {
                for (int i = 1; i <= curTutorial.LastIndex; i++)
                {
                    if (!TutorialSystem.Instance().IsCompleted(curTutorial.Type + i))
                    {
                        TutorialSystem.Instance().AddCompleted(curTutorial.Type + i);
                        TutorialSystem.Instance().OnContextChanged();
                    }
                }
                nextButton.gameObject.SetActive(true);
                previousButton.gameObject.SetActive(false);
            }
            else if (curTutorial.TutorialIndex == curTutorial.LastIndex)
            {
                nextButton.gameObject.SetActive(false);
                previousButton.gameObject.SetActive(true);
            }
            else
            {
                nextButton.gameObject.SetActive(true);
                previousButton.gameObject.SetActive(true);
            }
        }

        backGround.gameObject.SetActive(true);
        backGround.DissolveIn();
    }

    private void NextTutorial()
    {
        switchingIndex = true;
        
        TutorialType nextTutorialType = curTutorial.Type + 1;

        textBackGround.DissolveOutOver = () =>
        {
            TryTutorial(nextTutorialType);
        };

        imageBackGround.DissolveOut();
        textBackGround.DissolveOut();
    }

    private void PreviousTutorial()
    {
        switchingIndex = true;
        
        TutorialType prevTutorialType = curTutorial.Type - 1;

        textBackGround.DissolveOutOver = () =>
        {
            TryTutorial(prevTutorialType);
        };

        imageBackGround.DissolveOut();
        textBackGround.DissolveOut();
    }

    private void CloseTutorial()
    {
        if (!openTutorial)
        {
            return;
        }
        
        GameManager.Instance().ResumeGame();
        
        canvasGroup.interactable = false;
        buttonBackGround.DissolveOut();
        imageBackGround.DissolveOut();
        textBackGround.DissolveOut();
    }
}

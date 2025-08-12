using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Michsky.UI.Dark;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class MemoUIController : UIControllerBase
{
    [Header("공통 사용 부분")]
    [SerializeField] private Button previousPageButton;
    [SerializeField] private Button nextPageButton;
    [SerializeField] private Button[] closeButtons;
    
    [Header("일반 메모용")]
    [SerializeField] private GameObject memoObject;
    [SerializeField] private Image memoImage;
    [SerializeField] private TMP_Text memoText;
    
    [Header("튜토리얼 메모용")]
    [SerializeField] private TutorialData tutorial;
    [SerializeField] private GameObject tutorialObject;
    [SerializeField] private Image tutorialImage;
    [SerializeField] private TMP_Text tutorialTitleText;
    [SerializeField] private TMP_Text tutorialDescriptionText;
    
    [Header("북마크 버튼")]
    [SerializeField] private ButtonManager tutorialBookmark;
    [SerializeField] private ButtonManager memoBookmark;
    private BookMarkState curBookMark;

    [Header("이미지")]
    [SerializeField] private List<string> memoSpritePaths = new List<string>();
    private Dictionary<string, Sprite> memoSpriteDic;
    private List<(string memoText, Sprite memoSprite)> memoList = new List<(string memoText, Sprite memoSprite)>();
    
    private int page;
    
    private enum BookMarkState
    {
        Memo,
        Tutorial,
    }
    
    protected override void SetUp()
    {
        previousPageButton.onClick.RemoveAllListeners();
        previousPageButton.onClick.AddListener(GoToPreviousPage);
        
        nextPageButton.onClick.RemoveAllListeners();
        nextPageButton.onClick.AddListener(GoToNextPage);

        foreach (var closeButton in closeButtons)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(() => UIManager.Instance().HideController<MemoUIController>());
        }
        
        BookMarkSetting(tutorialBookmark, BookMarkState.Tutorial);
        BookMarkSetting(memoBookmark, BookMarkState.Memo);
        
        UIManager.Instance().RegisterController(this);
        gameObject.SetActive(false);

        _ = LoadSprite();
    }

    private async Task LoadSprite()
    {
        memoSpriteDic = new Dictionary<string, Sprite>();

        foreach (var path in memoSpritePaths)
        {
            var handle = Addressables.LoadAssetAsync<Sprite>(path);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                memoSpriteDic[path] = handle.Result;
            }
            else
            {
                Debug.LogWarning($"이미지 불러오기 실패: {path}");
            }
        }
    }

    private void BookMarkSetting(ButtonManager bookmark, BookMarkState state)
    {
        bookmark.soundSource = SoundManager.Instance().SFXSource;
        Image bookmarkImage = bookmark.GetComponent<Image>();
        float defaultFill = bookmarkImage.fillAmount;
        
        bookmark.clickEvent.RemoveAllListeners();
        bookmark.hoverEvent.RemoveAllListeners();
        bookmark.onLeave.RemoveAllListeners();

        bookmark.hoverEvent.AddListener(() => bookmarkImage.fillAmount = 1f);
        bookmark.onLeave.AddListener(() => bookmarkImage.fillAmount = defaultFill);

        switch (state)
        {
            case BookMarkState.Tutorial:
                bookmark.clickEvent.AddListener(() =>
                {
                    SwitchBookmark(BookMarkState.Tutorial);
                });
                break;
            case BookMarkState.Memo:
                bookmark.clickEvent.AddListener(() =>
                {
                    SwitchBookmark(BookMarkState.Memo);
                });
                break;
        }
    }

    private void SwitchBookmark(BookMarkState bookmarkState)
    {
        curBookMark = bookmarkState;
        
        switch (bookmarkState)
        {
            case BookMarkState.Memo:
                memoObject.SetActive(true);
                tutorialObject.SetActive(false);
                page = 0;
                SetMemoPage();
                memoBookmark.buttonVar.interactable = false;
                tutorialBookmark.buttonVar.interactable = true;
                break;
            case BookMarkState.Tutorial:
                memoObject.SetActive(false);
                tutorialObject.SetActive(true);
                page = 0;
                SetMemoPage();
                memoBookmark.buttonVar.interactable = true;
                tutorialBookmark.buttonVar.interactable = false;
                break;
        }
    }

    private void SetMemoPage()
    {
        if (page < 0)
        {
            page = 0;
            return;
        }
        
        switch (curBookMark)
        {
            case BookMarkState.Memo:
                if (page > PlayerManager.Instance().LocalContext.Memo.Count - 1)
                {
                    page = PlayerManager.Instance().LocalContext.Memo.Count - 1;
                    return;
                }
                memoText.SetText(memoList[page].memoText);
                memoImage.sprite = memoList[page].memoSprite;
                break;
            case BookMarkState.Tutorial:
                if (page > tutorial.tutorials.Length - 1)
                {
                    page = tutorial.tutorials.Length - 1;
                    return;
                }
                tutorialTitleText.SetText(tutorial.tutorials[page].TutorialName);
                tutorialDescriptionText.SetText(tutorial.tutorials[page].TutorialDescription);
                tutorialImage.sprite = tutorial.tutorials[page].TutorialMemoImage;
                break;
        }
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        UpdateMemoList();
        SoundManager.Instance().Play(GameConstants.Sound.MEMO_OPEN);
    }

    private void UpdateMemoList()
    {
        foreach (Memo memo in PlayerManager.Instance().LocalContext.Memo)
        {
            Sprite memoSprite = memoSpriteDic[memo.MemoImagePath];
            if (memoList.Contains((memo.MemoText, memoSprite)))
            {
                continue;
            }
            memoList.Add((memo.MemoText, memoSprite));
        }
    }

    public override void Hide()
    {
        gameObject.SetActive(false);
        SoundManager.Instance().Play(GameConstants.Sound.MEMO_CLOSE);
    }
    
    private void GoToNextPage()
    {
        SoundManager.Instance().Play(GameConstants.Sound.TURN_PAGE);
        
        if (CheckTutorialPage())
        {
            return;
        }
        
        page++;
        
        SetMemoPage();
    }

    private void GoToPreviousPage()
    {
        SoundManager.Instance().Play(GameConstants.Sound.TURN_PAGE);
        
        page--;
        
        SetMemoPage();
    }

    private bool CheckTutorialPage()
    {
        if (SceneLoader.Instance().CurMapType != MapType.Tutorial || curBookMark != BookMarkState.Tutorial)
        {
            return false;
        }
        
        if (TutorialSystem.Instance() == null)
        {
            TutorialSystem.Instance().Init(false);
        }

        return !TutorialSystem.Instance().IsCompleted((TutorialType)(page + 1));
    }
}

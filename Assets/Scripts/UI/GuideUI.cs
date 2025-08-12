using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Michsky.UI.Dark;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GuideUI : MonoBehaviour
{
    [Header("피수요소")]
    [SerializeField] private UIDissolveEffect guideScreen;
    [SerializeField] private TextMeshProUGUI guideText;
    [SerializeField] private GuideData guideData;
    [SerializeField] private Button button;
    [SerializeField] private float textSpeed = 3f;
    private PlayerContext playerContext;
    
    [Header("미필수요소")]
    [SerializeField] private Image background;
    [SerializeField] private TextMeshProUGUI continueText;
    
    private List<GuideStruct> ableList = new List<GuideStruct>();
    private WaitUntil waitForConfirm;
    private WaitUntil waitForDissoveOut;
    private Tween guideTextTween;
    private bool isDissolveOver = true;
    private bool confirm = false;
    
    public bool IsAllGuideOver { get; private set; }
    public Action GuideOverAction;

    private void Reset()
    {
        guideScreen = transform.FindComponent<UIDissolveEffect>("GuideScreen");
        guideText = transform.FindComponent<TextMeshProUGUI>("Description");
        button = transform.FindComponent<Button>("Confirm");
    }

    public void Init()
    {
        playerContext = PlayerManager.Instance().LocalContext;
        
        int finishedGuideCount = 0;
        
        foreach (var checkGuide in guideData.GuideList)
        {
            if (playerContext.Dialogues.ContainsKey(checkGuide.Name))
            {
                finishedGuideCount++;
            }
        }

        if (finishedGuideCount == guideData.GuideList.Count)
        {
            IsAllGuideOver = true;
            gameObject.SetActive(false);
            background?.gameObject.SetActive(false);
            return;
        }

        if (background != null)
        {
            background.enabled = true;
        }

        if (continueText != null)
        {
            Color color = continueText.color;
            color.a = 0f;
            continueText.color = color;
        }
        
        waitForConfirm = new WaitUntil(() => confirm);
        waitForDissoveOut = new WaitUntil(() => isDissolveOver);

        guideScreen.location = 1f;
        guideScreen.DissolveInOver = () => button.interactable = true;
        guideScreen.DissolveOutOver = () =>
        {
            button.interactable = false;
            guideScreen?.gameObject.SetActive(false);
            isDissolveOver = true;
        };
        
        button.interactable = false;
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() =>
        {
            confirm = true;
        });
    }

    private bool IsGuideAble(GuideStruct guide)
    {
        bool isUnFinished = false;
        switch (guide.Type)
        {
            case GuideType.NormalGuide:
                isUnFinished = !playerContext.Dialogues.ContainsKey(guide.Name);
                break;
            case GuideType.BossGuide:
                isUnFinished = !playerContext.Dialogues.ContainsKey(guide.Name) &&
                               playerContext.KilledEnemies.ContainsKey(guide.BossId);
                break;
            case GuideType.MemoGuide:
                bool hasMemo = false;
                foreach (var memo in playerContext.Memo.Where(memo => memo.memoId == guide.MemoId))
                {
                    hasMemo = true;
                }
                isUnFinished = !playerContext.Dialogues.ContainsKey(guide.Name) && hasMemo;
                break;
            case GuideType.AfterDialogueGuide:
                bool isDialogueOver = false;
                foreach (var dialogue in playerContext.Dialogues.Where(dialogue => dialogue.Key == guide.DialogueId))
                {
                    if (dialogue.Value)
                    {
                        isDialogueOver = true;
                    }
                }
                isUnFinished = !playerContext.Dialogues.ContainsKey(guide.Name) && isDialogueOver;
                break;
        }
        return isUnFinished;
    }
    
    public IEnumerator GuideCheck()
    {
        yield return new WaitUntil(() => !SceneLoader.Instance().IsLoading);
        
        foreach (var guide in guideData.GuideList)
        {
            if (IsGuideAble(guide))
            {
                ableList.Add(guide);
            }
        }

        if (ableList.Count <= 0)
        {
            gameObject.SetActive(false);
            background?.gameObject.SetActive(false);
            yield break;
        }

        var sort = ableList.OrderBy(a => a.Priority).ToList();
        ableList = sort;
        
        PlayerManager.Instance().SwitchPlayerInputState(PlayerInputState.Menu_State);

        for (int i = 0; i < ableList.Count; i++)
        {
            var guide = ableList[i];

            guideText.text = string.Empty;

            yield return waitForDissoveOut;

            guideScreen.gameObject.SetActive(true);
            guideScreen.DissolveIn();

            yield return guideTextTween = guideText.DOText(guide.Text, textSpeed).SetEase(Ease.Linear);
            yield return continueText?.DOFade(1f, 0.3f);

            yield return waitForConfirm;
            confirm = false;

            if (guideText.text != guide.Text)
            {
                guideTextTween.Kill(true);
                guideText.text = guide.Text;

                yield return waitForConfirm;
                confirm = false;
            }

            yield return continueText?.DOFade(0f, 0.1f);

            isDissolveOver = false;
            guideScreen.DissolveOut();

            playerContext.Dialogues[guide.Name] = true;

            if (i == ableList.Count - 1)
            {
                yield return waitForDissoveOut;

                if (background != null)
                {
                    yield return background.DOFade(0f, 0.5f).WaitForCompletion();
                    background.enabled = false;
                }

                gameObject.SetActive(false);
                background?.gameObject.SetActive(false);

                IsAllGuideOver = true;

                playerContext?.Save();

                PlayerManager.Instance().SwitchPlayerInputState(PlayerInputState.Move_State);

                GuideOverAction?.Invoke();

                yield break;
            }
        }
    }
}

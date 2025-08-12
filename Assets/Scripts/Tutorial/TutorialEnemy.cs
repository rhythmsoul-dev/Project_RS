using System.Collections;
using UnityEngine;

public class TutorialEnemy : Enemy
{
    private TutorialUI tutorialUI;
    private CombatUIController combatUIController;

    private bool isSetTutorial;

    protected override void OnAwake()
    {
        base.OnAwake();
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return new WaitUntil(() => UIManager.Instance().HasController<TutorialUI>());
        yield return new WaitUntil(() => UIManager.Instance().HasController<CombatUIController>());

        AddTutorialEvent();
        yield break;
    }

    public void AddTutorialEvent()
    {
        tutorialUI = UIManager.Instance().GetController<TutorialUI>();
        combatUIController = UIManager.Instance().GetController<CombatUIController>();

        //적 조우 튜토리얼(페이드 인 하고 나타나야함)
        combatUIController.BlendEffect.DissolveInOver += () =>
        {
            tutorialUI.TryTutorial(TutorialType.Battle_Encounter);
        };

        //인카운트 노트 튜토리얼(인카운트 노트가 생성되면 나타나야함)
        EncountEvent += () =>
        {
            tutorialUI.TryTutorial(TutorialType.Battle_EncountNote);
        };

        //체력, 체간 / 공격 튜토리얼(페이드 아웃 하고 나타나야함)
        combatUIController.FadeEffect.DissolveOutOver += () =>
        {
            tutorialUI.TryTutorial(TutorialType.Battle_Health);
        };

        //처형 튜토리얼(처형 노트가 생성되면 나타나야함)
        GroggyEvent += () =>
        {
            if (TutorialSystem.Instance().IsCompleted(TutorialType.Battle_Health))
            {
                tutorialUI.TryTutorial(TutorialType.Battle_Execution);
            }
        };
    }
}


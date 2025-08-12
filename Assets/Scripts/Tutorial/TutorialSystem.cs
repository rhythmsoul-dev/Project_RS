using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum TutorialType
{
    Exploration_Movement = 0,
    Exploration_Memo,
    Exploration_Interaction,
    Exploration_Portal,
    Battle_Encounter,
    Battle_NoteSystem1,
    Battle_NoteSystem2,
    Battle_EncountNote,
    Battle_Health,
    Battle_EnemyHealth,
    Battle_Attack,
    Battle_Heal,
    Battle_Execution,
    General_Town,
    General_Dialogue,
    General_ShopUI,
}

[Serializable]
public struct Tutorial
{
    public string TutorialName;
    public TutorialType Type;
    public Sprite TutorialImage;
    public Sprite TutorialMemoImage;
    public string TutorialDescription;

    public int TutorialIndex;
    public bool HasIndex => TutorialIndex >= 0;
    public int LastIndex;

    public Tutorial(TutorialType type)
    {
        Type = type;
        TutorialIndex = -1;
        TutorialName = Enum.GetName(typeof(TutorialType), type) + "의 이름";
        TutorialImage = null;
        TutorialMemoImage = null;
        TutorialDescription = Enum.GetName(typeof(TutorialType), type) + "의 설명";
        LastIndex = -1;
    }
}

public class TutorialSystem : GameSystem<TutorialSystem>
{
    private const string tutorialUIPath = "TutorialUI";

    private TutorialContext context;

    public void Init(bool isTryTutorial = true)
    {
        if (context == null)
        {
            context = SaveLoadManager.Load<TutorialContext>();
        }

        if (isTryTutorial)
        {
            Addressables.LoadAssetAsync<GameObject>(tutorialUIPath).Completed += (handle) =>
            {
                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    TutorialUI tutorialUI = Instantiate(handle.Result).GetComponent<TutorialUI>();
                    UIManager.Instance().RegisterController(tutorialUI);
                }
                else
                {
                    Debug.LogErrorFormat("[Addressable] Cannot Found Path: {0}", tutorialUIPath);
                }
            };
        }
    }

    public bool IsCompleted(TutorialType type)
    {
        return context.Completed.Contains(type);
    }

    public void AddCompleted(TutorialType type)
    {
        context.Completed.Add(type);
    }

    public void RemoveCompleted(TutorialType type)
    {
        context.Completed.Remove(type);
    }

    public void OnContextChanged()
    {
        context.Save();
    }
}

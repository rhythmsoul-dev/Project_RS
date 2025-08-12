using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class TitleScene : MonoBehaviour
{
    [SerializeField] private AudioClip titleBGM;
    
    private void Awake()
    {
        GameManager.Instance().Init();
    }

    private void Start()
    {
        if (titleBGM != null)
        {
            SoundManager.Instance().PlayBGM(titleBGM);
        }
    }

    public void NewGame()
    {
        SaveLoadManager.DeleteAllSaveFiles();
        PlayerManager.Instance().ResetData();

        SceneLoader.Instance().LoadScene(GameConstants.Scene.TUTORIAL_SCENE, mapType: MapType.Tutorial);
    }

    public void LoadGame()
    {
        TutorialContext tutorial = SaveLoadManager.Load<TutorialContext>();
        if (tutorial == null || !tutorial.Completed.Contains(TutorialType.General_Town))
        {
            Debug.Log("튜토리얼 미완료");
            return;
        }

        SceneLoader.Instance().LoadScene(GameConstants.Scene.VILLAGE_SCENE);
    }

    public void Settings()
    {
        UIManager.Instance().ShowController<SettingsUIController>();
    }

    public void ExitGame()
    {
        GameManager.Instance().ExitGame();
    }
}

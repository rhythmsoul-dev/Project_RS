using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public bool IsGamePaused { get; private set; }
    
    public async void Init()
    {
        Application.targetFrameRate = 120;

        await NetworkManager.Instance().Init();
        await PortalManager.Instance().Init();

        SaveLoadManager.Init();
        PlayerManager.Instance().Init();

        Time.timeScale = 1f;
    }

    protected override void OnAwake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    protected override void OnDestroyed()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void PauseGame()
    {
        IsGamePaused = true;
        if (PlayerManager.Instance().LocalPlayer != null)
        {
            PlayerManager.Instance().LocalPlayer.IsLocked = true;
        }
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        IsGamePaused = false;
        if (PlayerManager.Instance().LocalPlayer != null)
        {
            PlayerManager.Instance().LocalPlayer.IsLocked = false;
        }
        Time.timeScale = 1f;
    }
}

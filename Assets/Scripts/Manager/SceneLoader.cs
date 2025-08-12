using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SceneLoader : Singleton<SceneLoader>
{
    public bool IsLoading { get; private set; }

    [SerializeField] private Canvas sceneLoaderCanvas;
    [SerializeField] private CanvasGroup sceneLoaderCanvasGroup;
    [SerializeField] private Slider progressBar;
    
    [SerializeField] private Light2D backgroundLight;
    [SerializeField] private Color graveyardColor;
    [SerializeField] private Color dungeonColor;
    
    public MapType CurMapType { get; private set; }
    public int CurMapIndex { get; private set; }
    public string LoadSceneName { get; private set; }

    private event Action onSceneLoaded;

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
        if (scene.name == LoadSceneName)
        {
            StartCoroutine(LoadSceneEnded());
        }
    }

    private IEnumerator LoadSceneEnded()
    {
        yield return new WaitUntil(() => LoadSceneName == SceneManager.GetActiveScene().name);
        
        if (PlayerManager.Instance().LocalPlayer != null)
        {
            onSceneLoaded?.Invoke();

            yield return new WaitForEndOfFrame();
            
            //Transform mainCamera = CameraManager.Instance().MainCamera.transform;
            //Transform target = CameraManager.Instance().PlayerCamera.transform;
            
            //yield return new WaitUntil(() => Vector3.Distance(mainCamera.position, target.position) <= 0.01f);
            //카메라 위치 정렬 조건 대신 시간 대기로 변경
            yield return new WaitForSeconds(1f);
        }
        
        UIManager.Instance().FadeOut(sceneLoaderCanvasGroup, 0.4f);
        
        onSceneLoaded?.Invoke();
        
        yield return new WaitUntil(() => sceneLoaderCanvasGroup.alpha == 0);
        
        
        IsLoading = false;
        
        yield break;
    }

    public bool IsOpenned()
    {
        return sceneLoaderCanvasGroup.isActiveAndEnabled;
    }

    public void LoadScene(string sceneName, Action onLoaded = null, MapType mapType = MapType.Town, int mapIndex = 0)
    {
        if (IsLoading)
        {
            return;
        }

        CurMapType = mapType;

        switch (mapType)
        {
            case MapType.Graveyard:
                backgroundLight.color = graveyardColor;
                break;
            case MapType.Dungeon:
                backgroundLight.color = dungeonColor;
                break;
        }
        
        CurMapIndex = mapIndex;
        LoadSceneName = sceneName;
        onSceneLoaded = onLoaded;

        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        IsLoading = true;
        progressBar.value = 0f;
        UIManager.Instance().FadeIn(sceneLoaderCanvasGroup, 0.4f);
        yield return new WaitForSecondsRealtime(0.4f);

        AsyncOperation op = SceneManager.LoadSceneAsync(LoadSceneName);
        op.allowSceneActivation = false;

        float timer = 0.0f;
        while (!op.isDone)
        {
            timer += Time.unscaledDeltaTime;
            if (op.progress < 0.9f)
            {
                progressBar.value = Mathf.Lerp(progressBar.value, op.progress, timer);
                if (progressBar.value >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                progressBar.value = Mathf.Lerp(progressBar.value, 1f, timer);
                if (progressBar.value == 1.0f)
                {
                    op.allowSceneActivation = true;
                    break;
                }
            }

            yield return null;
        }

        yield break;
    }

    // public void LoadSceneWithPanelHide(string sceneName, string panelToHide)
    // {
    //     UIManager.Instance().HideAllPanels();
    //     LoadScene(sceneName);
    // }
}
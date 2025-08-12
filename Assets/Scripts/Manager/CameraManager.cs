using System;
using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System.Linq;
using DG.Tweening;

public class CameraManager : Singleton<CameraManager>
{
    private Camera mainCamera;
    public Camera MainCamera
    {
        get
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            return mainCamera;
        }
    }

    private CinemachineCamera[] cinemachineCameras;
    public CinemachineCamera CinemachineCamera
    {
        get
        {
            cinemachineCameras = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            CinemachineCamera cinemachineCamera = cinemachineCameras.Where(c => c.isActiveAndEnabled).ToArray()[0];
            int highestPriority = int.MinValue;

            for (int i = 0; i < cinemachineCameras.Length; i++)
            {
                if (cinemachineCameras[i].isActiveAndEnabled && cinemachineCameras[i].Priority > highestPriority)
                {
                    cinemachineCamera = cinemachineCameras[i];
                    highestPriority = cinemachineCameras[i].Priority;
                }
            }

            return cinemachineCamera;
        }
    }

    
    [Header("플레이어 카메라 관련")]
    public CinemachineCamera PlayerCamera { get; private set; }
    public CinemachinePositionComposer PlayerFollowCamera { get; private set; }
    public CinemachineConfiner2D PlayerCameraConfiner { get; private set; }
    public float PlayerFollowCameraOffset { get; private set; }
    
    private CinemachineCamera combatCamera;
    private CinemachinePositionComposer combatPositionComposer;
    
    private float originSize;
    private Vector3 originOffset;
    private bool isExecuting = false;

    protected override bool CheckDontDestroyOnLoad()
    {
        return false;
    }

    protected override void OnAwake()
    {
        mainCamera = Camera.main;
        //cinemachineCameras = FindObjectsByType<CinemachineCamera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    private void Start()
    {
        if (PlayerManager.Instance().LocalPlayer != null)
        {
            SetPlayerCamera();
        }
    }

    public void ShakeCamera(float intensity, float duration, bool ignoreTimeScale = false)
    {
        StartCoroutine(ProcessShakeCamera(intensity, duration, ignoreTimeScale));
    }

    private IEnumerator ProcessShakeCamera(float intensity, float duration, bool ignoreTimeScale)
    {
        MainCamera.GetComponent<CinemachineBrain>().IgnoreTimeScale = ignoreTimeScale;
        CinemachineBasicMultiChannelPerlin multiChannelPerlin = CinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Noise) as CinemachineBasicMultiChannelPerlin;
        multiChannelPerlin.AmplitudeGain = intensity;

        yield return new WaitForSeconds(duration);

        multiChannelPerlin.AmplitudeGain = 0f;

        yield break;
    }

    public void ZoomIn(float start, float end, float duration, bool ignoreTimeScale = false)
    {
        StartCoroutine(ProcessZoomIn(start, end, duration, ignoreTimeScale));
    }

    private IEnumerator ProcessZoomIn(float start, float end, float duration, bool ignoreTimeScale, CinemachineCamera zoomInCamera = null)
    {
        MainCamera.GetComponent<CinemachineBrain>().IgnoreTimeScale = ignoreTimeScale;

        if (zoomInCamera == null)
        {
            zoomInCamera = CinemachineCamera;
        }
        
        CinemachinePositionComposer positionComposer = zoomInCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;
        
        float t = 0f;
        while (t < 1f)
        {
            t += (ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime) / duration;

            positionComposer.TargetOffset = Vector3.Lerp(positionComposer.TargetOffset, Vector3.zero, t);
            zoomInCamera.Lens.OrthographicSize = Mathf.Lerp(start, end, t);

            yield return null;
        }
        isZoomInOver = true;
    }

    public void SetPlayerCamera()
    {
        if (PlayerManager.Instance().LocalPlayer == null)
        {
            return;
        }

        PlayerCamera = PlayerManager.Instance().LocalPlayer.GetComponentInChildren<CinemachineCamera>();
        PlayerFollowCamera = PlayerCamera.GetComponent<CinemachinePositionComposer>();
        PlayerFollowCameraOffset = PlayerFollowCamera.TargetOffset.x;
        PlayerCameraConfiner = PlayerCamera.GetComponent<CinemachineConfiner2D>();
        
        Collider2D camerabound = GameObject.Find("CameraBound")?.GetComponent<Collider2D>();
        if (camerabound != null) PlayerCameraConfiner.BoundingShape2D = camerabound;
    }

    public void StartExecution()
    {
        if (isExecuting)
        {
            return;
        }

        StartCoroutine(ProcessExecution());
    }

    private IEnumerator ProcessExecution()
    {
        combatCamera = CinemachineCamera;
        combatPositionComposer = combatCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachinePositionComposer;

        isExecuting = true;
        originSize = combatCamera.Lens.OrthographicSize;
        originOffset = combatPositionComposer.TargetOffset;

        Time.timeScale = 0.2f;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        ShakeCamera(15f, 0.1f, true);

        //ZoomIn(originSize, 3f, 0.8f);

        yield return new WaitForSecondsRealtime(1f);

        EndExecution();

        yield break;
    }

    private void EndExecution()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;
        
        combatPositionComposer.TargetOffset = originOffset;
        combatCamera.Lens.OrthographicSize = originSize;

        isExecuting = false;
    }
    
    private float zoomInDuration = 0.5f;
    private bool isZoomInOver;
    
    public void EncounterZoomIn()
    {
        float originalSize = PlayerCamera.Lens.OrthographicSize;
        float targetSize = PlayerCamera.Lens.OrthographicSize / 2f;
        Vector3 composerOriginOffset = PlayerFollowCamera.TargetOffset;
        StartCoroutine(ProcessZoomIn(originalSize, targetSize, zoomInDuration, true, PlayerCamera));
        StartCoroutine(EncounterZoomOut(originalSize, composerOriginOffset));
    }

    private IEnumerator EncounterZoomOut(float originalSize, Vector3 offSet)
    {
        yield return new WaitUntil(() => isZoomInOver);
        isZoomInOver = false;
        yield return new WaitForSeconds(zoomInDuration);
        PlayerCamera.Lens.OrthographicSize = originalSize;
        PlayerFollowCamera.TargetOffset = offSet;
    }
}
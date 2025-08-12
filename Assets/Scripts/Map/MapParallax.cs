using UnityEngine;

public class MapParallax : MonoBehaviour
{
    private Transform cameraTransform;

    [Header("카메라기준 속도 값"), Tooltip("해당 값은 반드시 0보다 작아야함 0보다 크면 플레이어를 추월함")]
    [SerializeField] private float parallaxSpeed;
    [SerializeField] private float smoothness = 100f;
    
    private Vector3 previousCameraPosition;
    private Vector3 targetPosition;
    private float deltaX;
    private Vector3 movePosition = new Vector3(1f, 0f, 0f);

    private bool isSet;

    private void Reset()
    {
        Init();
    }

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        cameraTransform = CameraManager.Instance().MainCamera.transform;
        previousCameraPosition = cameraTransform.position;
    }

    private void FixedUpdate()
    {
        if (PlayerManager.Instance().LocalPlayer.IsLocked || CameraManager.Instance().CinemachineCamera != CameraManager.Instance().PlayerCamera)
        {
            return;
        }
        
        deltaX = cameraTransform.position.x - previousCameraPosition.x;
        targetPosition = transform.position + movePosition * (deltaX * parallaxSpeed);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.fixedDeltaTime * smoothness);
        previousCameraPosition = cameraTransform.position;
    }
}
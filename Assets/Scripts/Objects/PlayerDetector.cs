using System.Linq;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [Header("감지 범위 설정")]
    [SerializeField] private float detectRange = 1f;
    [SerializeField] private Vector3 overlapOffset;
    [SerializeField] private LayerMask targetLayer;
    
    private Observable<bool> detected = new Observable<bool>(false);
    public Observable<bool> Detected => detected;
    [SerializeField] private bool forceActive;

    private void Reset()
    {
        Init();
    }

    private void Awake()
    {
        if (targetLayer == 0)
        {
            Init();
        }
    }

    private void Init()
    {
        targetLayer = 1 << LayerMask.NameToLayer("Player");
    }

    private void Update()
    {
        detected.Value = IsTargetDetected();
    }

    private bool IsTargetDetected()
    {
        if (forceActive)
        {
            return true;
        }
        Vector3 origin = transform.position + overlapOffset;
        Collider2D[] hits = Physics2D.OverlapCircleAll(origin, detectRange, targetLayer);

        return hits.Any(hit => hit.CompareTag("Player"));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 origin = transform.position + overlapOffset;
        Gizmos.DrawWireSphere(origin, detectRange);
    }
}
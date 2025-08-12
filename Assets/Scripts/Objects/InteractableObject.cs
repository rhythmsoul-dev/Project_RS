using UnityEngine;

/// <summary>
/// 루틴 오브젝트인지 적 오브젝트인지 구분하는 열거형값
/// </summary>
public enum ObjectType
{
    Loot,
    Enemy,
    NPC,
    Portal,
    Heal,
}

/// <summary>
/// 맵에 배치될 상호작용 오브젝트들의 상위 클래스
/// </summary>
[RequireComponent(typeof(BoxCollider2D))]
public abstract class InteractableObject : MonoBehaviour
{
    protected SpriteRenderer outLine;
    public ObjectType ObjectType { get; protected set; }
    public bool Interacted { get; protected set; }

    private void Reset()
    {
        OnReset();
        Init();
    }

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        OnStart();
    }

    protected virtual void OnReset() { }

    protected virtual void OnStart() { }

    protected abstract void Init();

    public abstract void OnInteract();

    public void ShowOutline()
    {
        if (outLine != null)
        {
            outLine.enabled = true;
        }
    }

    public void HideOutline()
    {
        if (outLine != null)
        {
            outLine.enabled = false;
        }
    }
}

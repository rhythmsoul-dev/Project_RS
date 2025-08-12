using UnityEngine;

/// <summary>
/// 플레이어 상태 관리 클래스
/// </summary>
public class PlayerStateMachine : MonoBehaviour
{
    public Player Player { get; private set; }

    [Header("애니메이션, 시각적 관련")] 
    public Animator PlayerAnimator { get; private set; }
    public SpriteRenderer PlayerSprite { get; private set; }
    public PlayerAnimationHash AnimationHash { get; private set; }
    
    //현재상태
    private PlayerBaseState currentState;
    
    //사용할 상태들
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerInteractState InteractState { get; private set; }

    private void Init()
    {
        Player = GetComponent<Player>();
        PlayerAnimator = GetComponentInChildren<Animator>();
        PlayerSprite = GetComponentInChildren<SpriteRenderer>();
        AnimationHash = new PlayerAnimationHash();
    }
    private void Reset()
    {
        Init();
    }

    private void Awake()
    {
        if (Player == null || AnimationHash == null)
        {
            Init();
        }
        
        AnimationHash.Initialize();
        
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(this);
        InteractState = new PlayerInteractState(this);
        
        
        ChangeState(IdleState);
    }

    private void Update()
    {
        if (CheckPlayerLocked())
        {
            return;
        }
        
        currentState?.UpdateState();
    }

    private void FixedUpdate()
    {
        if (CheckPlayerLocked())
        {
            return;
        }
        
        currentState?.PhysicUpdate();
    }

    public void ChangeState(PlayerBaseState newState)
    {
        currentState?.ExitState();
        currentState = newState;
        currentState?.EnterState();
    }

    private bool CheckPlayerLocked()
    {
        if (Player.IsLocked)
        {
            if (currentState != IdleState)
            {
                Player.PlayerRigidBody.linearVelocity = Vector2.zero;
                ChangeState(IdleState);
            }
            return true;
        }
        return false;
    }
}

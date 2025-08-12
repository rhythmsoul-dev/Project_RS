using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어의 입력값만을 받아오는 클래스
/// </summary>
public class PlayerInputs : MonoBehaviour
{
    [Header("Movement")]
    public Vector2 InputMove { get; private set; }

    private Player player;
    private PlayerStateMachine stateMachine;
    private HitZone hitZone;

    private void Start()
    {
        player = GetComponent<Player>();
        stateMachine = GetComponent<PlayerStateMachine>();
        
        hitZone = NoteSystem.Instance().HitZone;
    }

    #region PlayMode
    
    public void OnMove(InputAction.CallbackContext context)
    {
        if (SceneLoader.Instance().IsLoading)
        {
            return;
        }
        
        if (context.phase == InputActionPhase.Started)
        {
            InputMove = context.ReadValue<Vector2>();
        }

        if (context.phase == InputActionPhase.Canceled)
        {
            InputMove = Vector2.zero;
        }
    }

    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            stateMachine.ChangeState(stateMachine.InteractState);
        }
    }
    
    #endregion

    #region BattleMode

    public void OnAttack(InputAction.CallbackContext context)
    {
        // if (context.phase == InputActionPhase.Started)
        // {
        //     hitZone.OnAttack();
        // }
    }

    public void OnDefense(InputAction.CallbackContext context)
    {
        // if (context.phase == InputActionPhase.Started)
        // {
        //     hitZone.OnGuard();
        // }
    }

    public void OnUsePotion(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            player.Stats.UsePotion();
        }
    }
    #endregion
    
    public void ToggleMenu(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (PlayerManager.Instance().CurrentState != PlayerInputState.Menu_State)
            {
                UIManager.Instance().ShowController<SettingsUIController>();
            }
            else if (PlayerManager.Instance().CurrentState == PlayerInputState.Menu_State)
            {
                if (!UIManager.Instance().GetController<SettingsUIController>().isActiveAndEnabled)
                {
                    return;
                }
                UIManager.Instance().HideController<SettingsUIController>();
            }
        }
    }

    
}

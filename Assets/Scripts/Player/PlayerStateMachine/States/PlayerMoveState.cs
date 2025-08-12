using UnityEngine;

/// <summary>
/// 플레이어 이동 상태
/// </summary>
public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    
    //이동 처리할 변수
    private Vector2 move;

    
    public override void EnterState()
    {
        if (stateMachine.PlayerAnimator != null)
        {
            StartAnimation(stateMachine.AnimationHash.MoveParameterHash);
        }
    }

    public override void UpdateState()
    {
        //이동 방향에 따라 스프라이트와 카메라 방향 뒤집기
        switch (stateMachine.Player.Input.InputMove.x)
        {
            case < 0:
                stateMachine.PlayerSprite.flipX = true;
                CameraManager.Instance().PlayerFollowCamera.TargetOffset.x = -CameraManager.Instance().PlayerFollowCameraOffset;
                break;
            case > 0:
                stateMachine.PlayerSprite.flipX = false;
                CameraManager.Instance().PlayerFollowCamera.TargetOffset.x = CameraManager.Instance().PlayerFollowCameraOffset;
                break;
        }
        
        //멈춘 상태면 기본 상태로 전환
        if (stateMachine.Player.Input.InputMove == Vector2.zero)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }
    
    /// <summary>
    /// 이동입력값에 따라 x방향 이동을 처리하는 부분
    /// </summary>
    public override void PhysicUpdate()
    {
        move = stateMachine.Player.Input.InputMove * stateMachine.Player.Stats.MoveSpeed;
        
        Vector2 moveDirection = move;

        stateMachine.Player.PlayerRigidBody.linearVelocityX = moveDirection.x;
    }

    public override void ExitState()
    {
        if (stateMachine.PlayerAnimator != null)
        {
            StopAnimation(stateMachine.AnimationHash.MoveParameterHash);
        }
    }
}

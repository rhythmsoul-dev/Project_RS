/// <summary>
/// 플레이어 기본 상태
/// </summary>
public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void EnterState()
    {
        if (stateMachine.PlayerAnimator != null)
        {
            StartAnimation(stateMachine.AnimationHash.IdleParameterHash);
        }
    }

    public override void UpdateState()
    {
        //이동입력값이 0보다 클 때
        if (stateMachine.Player.Input.InputMove.magnitude > 0)
        {
            //이동 상태로 전환
            stateMachine.ChangeState(stateMachine.MoveState);
        }
    }

    public override void PhysicUpdate() { }

    public override void ExitState()
    {
        if (stateMachine.PlayerAnimator != null)
        {
            StopAnimation(stateMachine.AnimationHash.IdleParameterHash);
        }
    }
}

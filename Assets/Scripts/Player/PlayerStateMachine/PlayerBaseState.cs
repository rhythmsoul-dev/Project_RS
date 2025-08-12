/// <summary>
/// 플레이어 상태들이 상속하는 클래스
/// </summary>
public abstract class PlayerBaseState
{
    protected PlayerStateMachine stateMachine;

    protected PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
    
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void PhysicUpdate();
    public abstract void ExitState();
    
    protected void StartAnimation(int animatorHash)
    {
        stateMachine.PlayerAnimator.SetBool(animatorHash, true);
    }

    protected void StopAnimation(int animatorHash)
    {
        stateMachine.PlayerAnimator.SetBool(animatorHash, false);
    }
    
    /// <summary>
    /// 트리거 애니메이션 작동 함수
    /// </summary>
    /// <param name="triggerHash"></param>
    protected void SetAnimationTrigger(int triggerHash)
    {
        stateMachine.PlayerAnimator.SetTrigger(triggerHash);
    }
}

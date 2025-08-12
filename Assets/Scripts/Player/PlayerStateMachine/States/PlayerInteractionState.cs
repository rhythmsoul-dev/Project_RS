using UnityEngine;

/// <summary>
/// 플레이어 상호작용 상태
/// </summary>
public class PlayerInteractState : PlayerBaseState
{
    public PlayerInteractState(PlayerStateMachine stateMachine) : base(stateMachine) { }
    
    public override void EnterState()
    {
        if (!stateMachine.Player.Interaction.HasInteractionObject())
        {
            return;
        }
        
        if (stateMachine.Player.Interaction.ClosestInteractable.ObjectType == ObjectType.Loot && 
            !stateMachine.Player.Interaction.ClosestInteractable.Interacted) 
        {
            //상호작용은 아무상태에서나 작동하기 위해 트리거 사용
            SetAnimationTrigger(stateMachine.AnimationHash.InteractionParameterHash);
        }
        //바로 상호작용 실행
        stateMachine.Player.Interaction.OnInteract();
    }

    public override void UpdateState()
    {
        //상호작용 애니메이션이 끝났을때
        if (IsInteractionOver())
        {
            //이동입력값을 받고 있다면 이동 상태로 전환, 그렇지 않다면 기본 상태로 전환
            if (stateMachine.Player.Input.InputMove.magnitude > 0)
            {
                stateMachine.ChangeState(stateMachine.MoveState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }
    }

    public override void PhysicUpdate() { }

    public override void ExitState() { }
    
    /// <summary>
    /// 인터렉션 애니메이션이 끝났는지 확인하는 함수
    /// </summary>
    /// <returns></returns>
    private bool IsInteractionOver()
    {
        //0번째 레이어(BaseLayer)의 상태를 읽어온다.
        AnimatorStateInfo stateInfo = stateMachine.PlayerAnimator.GetCurrentAnimatorStateInfo(0);
        //해당 애니메이션 태그가 Interaction일때
        if (stateInfo.IsTag("Interaction"))
        { 
            //그 애니메이션의 진행 시간이 1이상(끝났을때) true를 반환
            return stateInfo.normalizedTime >= 1f;
        }
        //태그가 안맞을때(이미 다른 애니메이션으로 넘어간 경우) true를 반환
        return true;
    }
}

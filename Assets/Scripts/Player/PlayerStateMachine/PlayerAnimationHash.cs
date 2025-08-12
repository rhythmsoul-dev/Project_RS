using System;
using UnityEngine;

/// <summary>
/// 플레이어 애니메이션 bool, trigger값을 저장하는 클래스
/// </summary>
[Serializable]
public class PlayerAnimationHash
{
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string moveParameterName = "Move";
    [SerializeField] private string interactionParameterName = "Interaction";
    
    public int IdleParameterHash { get; private set; }
    public int MoveParameterHash { get; private set; }
    public int InteractionParameterHash { get; private set; }
    
    public void Initialize()
    {
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        MoveParameterHash = Animator.StringToHash(moveParameterName);
        InteractionParameterHash = Animator.StringToHash(interactionParameterName);
    }
}

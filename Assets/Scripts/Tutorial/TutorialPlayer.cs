using UnityEngine;

public class TutorialPlayer : Player
{
    protected override void OnAwake()
    {
        base.OnAwake();
        
        //회복 튜토리얼(튜토리얼 적의 첫 패턴이 지나고 나타나야함)
        DamagedEvent += () =>
        {
            if (TutorialSystem.Instance().IsCompleted(TutorialType.Battle_Health))
            {
                UIManager.Instance().GetController<TutorialUI>().TryTutorial(TutorialType.Battle_Heal);
            }
        };
    }

    public override void OnDamaged(Damaged damaged)
    {
        if ((Stats.CurHealth - damaged.AttackDamage) <= 0)
        {
            Stats.CurHealth = (1 + damaged.AttackDamage);
        }
        
        base.OnDamaged(damaged);
    }
}

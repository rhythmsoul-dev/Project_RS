using UnityEngine;

/// <summary>
/// 적과 조우하는 트리거로 사용할 오브젝트 클래스
/// </summary>
public class EnemyObject : InteractableObject
{
    public Enemy NormalEnemy { get; private set; }
    public Collider2D EnemyCollider { get; private set; }
    private Vector2 gizmosBox = new Vector2(2, 4);

    protected override void Init()
    {
        EnemyCollider = GetComponent<Collider2D>();
        ObjectType = ObjectType.Enemy;
        NormalEnemy = GetComponent<Enemy>();
    }
    
    /// <summary>
    /// 강제로 발동시킬 함수
    /// </summary>
    public override void OnInteract()
    {
        EnemyCollider.enabled = false;
        CombatSystem.Instance().StartCombat(NormalEnemy);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, gizmosBox);
    }
}

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class BossObject : InteractableObject
{
    private Enemy bossEnemy;
    private Collider2D bossCollider;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private PlayerDetector detector;
    
    protected override void Init()
    {
        ObjectType = ObjectType.Enemy;

        bossEnemy = GetComponent<Enemy>();
        bossCollider = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        detector = GetComponent<PlayerDetector>();

        bossEnemy.DeathEvent += delegate
        {
            spriteRenderer.enabled = false;
        };

        detector.Detected.Changed += DetectPlayer;

        if (PlayerManager.Instance().LocalContext.KilledEnemies.ContainsKey(bossEnemy.Ids[0]))
        {
            spriteRenderer.enabled = false;
            bossCollider.enabled = false;
        }
    }

    public override void OnInteract()
    {
        bossCollider.enabled = false;
        CombatSystem.Instance().StartCombat(bossEnemy);
    }

    private void DetectPlayer(bool detect)
    {
        animator.SetBool("Detect", detect);
    }
}

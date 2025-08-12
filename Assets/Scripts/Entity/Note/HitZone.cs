using System.Collections;
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

public class HitZone : MonoBehaviour
{
    private CapsuleCollider2D hitCollider;
    private SpriteRenderer spriteRenderer;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color attackColor;
    [SerializeField] private GameObject lockIcon;

    private Player Player => PlayerManager.Instance().LocalPlayer;
    private Enemy curEnemy;

    public bool IsAttackLocked { get; set; }
    public bool IsGuardLocked { get; set; }

    private bool isAttacking;

    private int triedGuardCount = 0;

    private Vector3 originSize;
    private List<Vector3> prevSizes = new List<Vector3>();

    private Coroutine attackCoroutine;
    private Coroutine guardCoroutine;
    private Coroutine sizeCoroutine;
    private Coroutine usedPotionCoroutine;

    /// <summary>
    /// 현재 가드가 가능한 노트
    /// </summary>
    private Observable<Note> curHitNote = new Observable<Note>(null);

    /// <summary>
    /// 가드가 가능한 모든 노트
    /// </summary>
    private List<Note> hitNotes = new List<Note>();
    //private Queue<Note> hitNotes = new Queue<Note>();

    private int comboCount = 0;
    private CombatUIController combatUIController;

    private int guardedCount = 0;
    private int parriedCount = 0;

    private void Awake()
    {
        hitCollider = GetComponent<CapsuleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        combatUIController = UIManager.Instance().GetController<CombatUIController>();
        IsAttackLocked = true;
        curHitNote.Changed += (Note note) =>
        {
            if (triedGuardCount >= 3 && note != null)
            {
                if (note.IsEncount())
                {
                    Encount();
                }
                else if (note.IsExecution())
                {
                    Execution();
                }
                else
                {
                    Guard();
                }

                return;
            }

            for (int i = hitNotes.Count - 1; i >= 0; i--)
            {
                if (hitNotes[i] == null || !hitNotes[i].isActiveAndEnabled || hitNotes[i].IsEnded)
                {
                    hitNotes.RemoveAt(i);
                }
            }
        };

        NoteSystem.Instance().HitZone = this;
    }

    private void Start()
    {
        spriteRenderer.color = normalColor;
        originSize = transform.localScale;

        comboCount = 0;
    }

    private void Update()
    {
        if (GameManager.Instance().IsGamePaused)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnAttack();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            OnGuard();
        }
    }

    private void OnEnable()
    {
        if (Player != null)
        {
            Player.EnteredGroggy += SetLockIcon;
            Player.ExitedGroggy += SetLockIcon;
            Player.UsedPotion += OnUsedPotion;
            Player.Stats.SetStats();
        }

        //curEnemy = CombatSystem.Instance().CurEnemy;
        //if (curEnemy != null)
        //{
        //    curEnemy.PhaseChangeEvent += OnChangedEnemyPhase;
        //}

        guardedCount = 0;
        parriedCount = 0;
        comboCount = 0;
    }

    private void OnDisable()
    {
        StopAllCoroutines();

        triedGuardCount = 0;
        transform.localScale = originSize;
        transform.position = new Vector3(originSize.x * 0.5f - 0.5f, 0f, 0f);
        spriteRenderer.color = normalColor;
        IsAttackLocked = true;

        if (Player != null)
        {
            Player.UsedPotion -= OnUsedPotion;
            Player.EnteredGroggy -= SetLockIcon;
            Player.ExitedGroggy -= SetLockIcon;
        }
    }

    public void ResetCombo()
    {
        comboCount = 0;
    }

    private void OnUsedPotion(int count)
    {
        if (usedPotionCoroutine != null)
        {
            StopCoroutine(usedPotionCoroutine);
            usedPotionCoroutine = null;
        }

        usedPotionCoroutine = StartCoroutine(ProcessUsedPotion());
    }

    private IEnumerator ProcessUsedPotion()
    {
        IsAttackLocked = true;
        SetHitZoneSize((transform.localScale * 0.75f).WithZ(1f), 0.8f, 1.5f);

        yield return new WaitForSeconds(1f);
        IsAttackLocked = false;

        yield break;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
        {
            //curHitNote = collision.GetComponent<Note>();

            Note note = collision.GetComponent<Note>();
            if (!hitNotes.Contains(note))
            {
                hitNotes.Add(note);
            }

            curHitNote.Value = hitNotes[0];
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Note"))
        {
            //curHitNote = null;
            hitNotes.Remove(collision.GetComponent<Note>());
            if (hitNotes.Count > 0)
            {
                curHitNote.Value = hitNotes[0];
            }
            else
            {
                curHitNote.Value = null;
            }
        }
    }

    private bool CheckAttackLocked()
    {
        if (Player.IsGroggy)
        {
            CameraManager.Instance().ShakeCamera(10f, 0.1f);
            SoundManager.Instance().Play(GameConstants.Sound.ATTACK_LOCKED);
            return true;
        }

        return IsAttackLocked;
    }

    /// <summary>
    /// 공격 시도
    /// </summary>
    public void OnAttack()
    {
        if (isAttacking || CombatSystem.Instance().CurEnemy == null || !CombatSystem.Instance().CurEnemy.IsAlive)
        {
            return;
        }

        if (CheckAttackLocked())
        {
            return;
        }

        if (attackCoroutine != null)
        {
            StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        attackCoroutine = StartCoroutine(ProcessAttack());
    }

    /// <summary>
    /// 공격 진행
    /// </summary>
    /// <returns></returns>
    private IEnumerator ProcessAttack()
    {
        isAttacking = true;
        IsGuardLocked = true;
        spriteRenderer.color = attackColor;

        int index = Random.Range(0, 3);
        
        SoundManager.Instance().Play(GameConstants.Sound.ATTACK_SOUNDS[index]);
        
        ObjectPoolManager.Instance().Get("Effects/Attack", UIManager.Instance().GetController<CombatUIController>().EnemyImage.transform.position, Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), 0f));

        float atkDmg = Player.Stats.TotalAttack;
        float balanceDmg = Player.Stats.BalanceAttackPower;

        CameraManager.Instance().ShakeCamera(Mathf.Clamp((atkDmg + balanceDmg) * 2.5f, 5f, 20f), 0.1f);

        DamagedSystem.Instance().Send(new Damaged
        {
            AttackDamage = Mathf.RoundToInt(atkDmg),
            BalanceDamage = Mathf.RoundToInt(balanceDmg),
            Attacker = Player.gameObject,
            Victim = CombatSystem.Instance().CurEnemy.gameObject,
            IgnoreDefense = false
        });

        yield return new WaitForSeconds(0.3f * (1f / Player.Stats.AttackSpeed));

        float elapsed = 0f;
        float duration = 0.7f * (1f / Player.Stats.AttackSpeed);
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            spriteRenderer.color = Color.Lerp(attackColor, normalColor, elapsed / duration);
            yield return null;
        }

        IsGuardLocked = false;
        isAttacking = false;

        yield break;
    }

    private bool CheckGuardLocked()
    {
        if (Player.IsGroggy)
        {
            CameraManager.Instance().ShakeCamera(10f, 0.1f);
            SoundManager.Instance().Play(GameConstants.Sound.ATTACK_LOCKED);
            return true;
        }

        return IsGuardLocked;
    }

    /// <summary>
    /// 가드 시도
    /// </summary>
    public void OnGuard()
    {
        if (CombatSystem.Instance().CurEnemy == null)
        {
            return;
        }

        if (CheckGuardLocked())
        {
            return;
        }

        //Debug.Log("OnGuard");
        if (guardCoroutine != null)
        {
            StopCoroutine(guardCoroutine);
            guardCoroutine = null;
        }

        guardCoroutine = StartCoroutine(ProcessGuard());
    }

    /// <summary>
    /// 가드 진행
    /// </summary>
    /// <returns></returns>
    private IEnumerator ProcessGuard()
    {
        //Debug.Log("ProcessGuard");
        triedGuardCount++;
        ObjectPoolManager.Instance().Get("Effects/TriedGuard", transform.position, Quaternion.identity);

        if (triedGuardCount >= 3)
        {
            if (triedGuardCount == 3)
            {
                SetHitZoneSize((transform.localScale * 0.85f).WithZ(1f), 0.8f, 1f);
            }
            else
            {
                SetHitZoneSize((transform.localScale * 0.7f).WithZ(1f), 0.8f, 1f);
            }

            yield return new WaitForSeconds(0.4f);
            triedGuardCount = 0;

            yield break;
        }

        if (curHitNote.Value != null)
        {
            if (curHitNote.Value.IsEncount())
            {
                Encount();

                yield return new WaitForSeconds(0.4f);
                triedGuardCount = 0;

                yield break;
            }

            if (curHitNote.Value.IsExecution())
            {
                Execution();

                yield return new WaitForSeconds(0.4f);
                triedGuardCount = 0;

                yield break;
            }

            // 판정범위의 크기가 노트의 크기보다 커야 패링 가능
            // 판정범위의 중심점으로부터 노트 중심점의 거리가 판정범위 지름의 40%보다 가까우면 패링
            float parryingDistance = (hitCollider.size * transform.lossyScale).x * 0.4f;
            bool canParrying = transform.localScale.ToVector2().IsGreaterOrEqual(curHitNote.Value.transform.localScale.ToVector2()) && (transform.position - curHitNote.Value.transform.position).sqrMagnitude < parryingDistance * parryingDistance;
            if (canParrying)
            {
                Parrying();
            }
            else
            {
                Guard();
            }

            combatUIController.ComboTextEffect(comboCount);
            curHitNote.Value = null;
        }

        yield return new WaitForSeconds(0.4f);
        triedGuardCount = 0;

        yield break;
    }

    /// <summary>
    /// 가드 완료
    /// </summary>
    private void Guard()
    {
        if (curHitNote.Value == null)
        {
            return;
        }

        ResetCombo();
        
        SoundManager.Instance().Play(GameConstants.Sound.GUARD);
        Debug.Log("Guard");

        float damage = CombatSystem.Instance().CurEnemy.Stats.BalanceAttackPower * 0.8f;
        //CameraManager.Instance().ShakeCamera(Mathf.Clamp(damage, 2.5f, 5f), 0.1f);

        DamagedSystem.Instance().Send(new Damaged
        {
            AttackDamage = 0,
            BalanceDamage = Mathf.RoundToInt(damage),
            Attacker = CombatSystem.Instance().CurEnemy.gameObject,
            Victim = Player.gameObject,
            IgnoreDefense = false
        });

        curHitNote.Value.End();
        curHitNote.Value = null;

        ObjectPoolManager.Instance().Get("Effects/Guard", transform.position, Quaternion.identity);
        //ObjectPoolManager.Instance().Get("Effects/Guard", transform.position, Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), 0f));

        guardedCount++;
    }

    private void Parrying()
    {
        if (curHitNote.Value == null)
        {
            return;
        }

        curHitNote.Value.OnParried();

        comboCount++;
        //Debug.Log("Parrying");

        int index = Random.Range(0, 2);
        SoundManager.Instance().Play(GameConstants.Sound.PARRYING_SOUNDS[index]);

        float damage = Player.Stats.BalanceAttackPower;
        CameraManager.Instance().ShakeCamera(Mathf.Clamp(damage * 2.5f, 10f, 20f), 0.1f);

        DamagedSystem.Instance().Send(new Damaged
        {
            AttackDamage = 0,
            BalanceDamage = Mathf.RoundToInt(damage),
            Attacker = Player.gameObject,
            Victim = CombatSystem.Instance().CurEnemy.gameObject,
            IgnoreDefense = false
        });

        if (comboCount >= 9)
        {
            ObjectPoolManager.Instance().Get("Effects/Combo15", transform.position, Quaternion.identity);
        }
        else if (comboCount >= 6)
        {
            ObjectPoolManager.Instance().Get("Effects/Combo10", transform.position, Quaternion.identity);
        }
        else if (comboCount >= 3)
        {
            ObjectPoolManager.Instance().Get("Effects/Combo5", transform.position, Quaternion.identity);
        }
        else
        {
            ObjectPoolManager.Instance().Get("Effects/Parrying", transform.position, Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), 0f));
        }

        parriedCount++;
    }

    private void Encount()
    {
        if (curHitNote.Value == null)
        {
            return;
        }

        int index = Random.Range(0, 3);
        SoundManager.Instance().Play(GameConstants.Sound.ENCOUNTER_SUCCESS_SOUNDS[index]);

        float atkDmg = (float)CombatSystem.Instance().CurEnemy.Stats.MaxHealth * 0.1f;
        CameraManager.Instance().ShakeCamera(Mathf.Clamp(atkDmg * 2.5f, 2.5f, 7.5f), 0.1f);

        DamagedSystem.Instance().Send(new Damaged
        {
            AttackDamage = Mathf.RoundToInt(atkDmg),
            BalanceDamage = 0,
            Attacker = Player.gameObject,
            Victim = CombatSystem.Instance().CurEnemy.gameObject,
            IgnoreDefense = true
        });

        curHitNote.Value.End();
        curHitNote.Value = null;

        //CombatSystem.Instance().CurEnemy.StartAttack();
        
        SoundManager.Instance().Play(GameConstants.Sound.GUARD);
        ObjectPoolManager.Instance().Get("Effects/Guard", transform.position, Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), 0f));
    }

    private void Execution()
    {
        if (curHitNote.Value == null)
        {
            return;
        }

        DamagedSystem.Instance().Send(new Damaged
        {
            AttackDamage = int.MaxValue,
            BalanceDamage = int.MaxValue,
            Attacker = Player.gameObject,
            Victim = CombatSystem.Instance().CurEnemy.gameObject,
            IgnoreDefense = true
        });

        curHitNote.Value.End();
        curHitNote.Value = null;
        
        SoundManager.Instance().Play(GameConstants.Sound.ENEMY_EXECUTION);
        ObjectPoolManager.Instance().Get("Effects/Execution", transform.position, Quaternion.Euler(Random.Range(0f, 360f), Random.Range(0f, 360f), 0f));
    }

    public void SetHitZoneSize(Vector3 size, float duration, float until)
    {
        if (!transform.localScale.ToVector2().IsGreaterOrEqual(originSize.ToVector2() * 0.4f))
        {
            return;
        }

        if (sizeCoroutine != null)
        {
            StopCoroutine(sizeCoroutine);
            sizeCoroutine = null;
        }

        sizeCoroutine = StartCoroutine(ProcessHitZoneSize(size.WithZ(1f), duration, until));
    }

    private IEnumerator ProcessHitZoneSize(Vector3 targetSize, float duration, float until)
    {
        prevSizes.Add(transform.localScale);
        prevSizes.Sort((a, b) => a.magnitude.CompareTo(b.magnitude));

        Vector3 targetPos = new Vector3(targetSize.x * 0.5f - 0.5f, 0f, 0f);

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime / duration;

            transform.localScale = Vector3.Lerp(transform.localScale, targetSize, t);
            transform.position = Vector3.Lerp(transform.position, targetPos, t);

            //NoteSystem.Instance().BlowZone.SetPosition(transform, t);

            yield return null;
        }

        transform.localScale = targetSize;
        transform.position = targetPos;

        yield return new WaitForSeconds(until);

        for (int i = 0; i < prevSizes.Count; i++)
        {
            targetPos = new Vector3(prevSizes[i].x * 0.5f - 0.5f, 0f, 0f);

            t = 0f;
            while ((prevSizes[i] - transform.localScale).sqrMagnitude > 0.001f)
            {
                t += Time.deltaTime * 2.5f;

                transform.localScale = Vector3.Lerp(transform.localScale, prevSizes[i], t);
                transform.position = Vector3.Lerp(transform.position, targetPos, t);

                //NoteSystem.Instance().BlowZone.SetPosition(transform, t2);

                yield return null;
            }

            transform.localScale = prevSizes[i];
            transform.position = targetPos;
        }

        prevSizes.Clear();

        transform.localScale = originSize;
        transform.position = new Vector3(originSize.x * 0.5f - 0.5f, 0f, 0f);

        yield break;
    }

    private void SetLockIcon()
    {
        lockIcon.SetActive(Player.IsGroggy);
    }
}

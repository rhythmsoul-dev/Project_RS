using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[Serializable]
public struct SpriteInfo
{
    public string Name;
    public string Path;
}

public class Enemy : StateBasedAI<Enemy.State>
{
    public enum State
    {
        Invalid = -1,
        Idle,
        Normal,
        Encount,
        Attack,
        Groggy,
        ChangingPhase,
        Dead
    }

    [Header("Enemy Components")]
    [SerializeField] private int[] ids;
    public int[] Ids => ids;
    [SerializeField] private int totalPhase = 1;
    private int curPhase = 1;
    
    protected override int Id
    {
        get
        {
            if (curPhase - 1 < ids.Length)
            {
                return ids[curPhase - 1];
            }
            else
            {
                return ids.Last();
            }
        }
    }

    protected override State InvalidState => State.Invalid;

    protected override int StateEnumCount => 6;

    [SerializeField] private SpriteInfo[] spriteInfos;
    private Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

    private Observable<Sprite> curSprite = new Observable<Sprite>(null);

    private Coroutine damagedCoroutine;

    public event Action AttackEvent;
    public event Action EncountEvent;
    public event Action PhaseChangeEvent;

    protected override void OnAwake()
    {
        base.OnAwake();

        curSprite.Changed += (sprite) =>
        {
            UIManager.Instance().GetController<CombatUIController>().ChangeEnemyImage(sprite);
        };
    }
    
    protected override IEnumerator OnStart()
    {
        StartCoroutine(base.OnStart());

        for (int i = 0; i < spriteInfos.Length; i++)
        {
            AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(spriteInfos[i].Path);

            yield return handle;
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                sprites.Add(spriteInfos[i].Name, handle.Result);
            }
            else
            {
                Debug.LogErrorFormat("[Addressable] Cannot Found Path: {0}", spriteInfos[i].Path);
            }
        }
        
        CurState = State.Idle;
        yield break;
    }

    protected override void DefineStates()
    {
        //AddState(State.Idle, new StateElem
        //{
        //    Entered = new Action(IdleEntered),
        //    Doing = new Func<IEnumerator>(OnIdle)
        //});

        //AddState(State.Normal, new StateElem
        //{
        //    Doing = new Func<IEnumerator>(OnNormal)
        //});

        AddState(State.Encount, new StateElem
        {
            Entered = new Action(EncountEntered),
            Doing = new Func<IEnumerator>(OnEncount)
        });

        AddState(State.Attack, new StateElem
        {
            Entered = new Action(AttackEntered),
            Doing = new Func<IEnumerator>(OnAttack),
            Exited = new Action(AttackExited)
        });

        AddState(State.Groggy, new StateElem
        {
            Entered = new Action(GroggyEntered),
            Exited = new Action(GroggyExited)
        });

        AddState(State.ChangingPhase, new StateElem
        {
            Doing = new Func<IEnumerator>(OnChangedPhase),
        });

        AddState(State.Dead, new StateElem
        {
            Doing = new Func<IEnumerator>(DeathDoing)
        });
    }

    public void EventIdle()
    {
        CurState = State.Idle;
    }

    private void IdleEntered()
    {
        curSprite.Value = sprites["Idle_" + curPhase];
    }

    private IEnumerator OnIdle()
    {
        yield break;
    }

    //private IEnumerator OnNormal()
    //{
    //    if (CombatSystem.Instance().IsInCombat && CombatSystem.Instance().CurEnemy == this && CurState != State.Encount)
    //    {
    //        CurState = State.Attack;
    //    }
    //    else
    //    {
    //        CurState = State.Idle;
    //    }

    //    yield return new WaitForSeconds(0.1f);
    //    yield break;
    //}

    public void EventEncount()
    {
        CurState = State.Encount;
    }

    private void EncountEntered()
    {
        curSprite.Value = sprites["Idle_" + curPhase];
    }

    private IEnumerator OnEncount()
    {
        Note note = NoteSystem.Instance().MakeNote(Info.EncountNoteId, 7, 1000);
        yield return new WaitForSeconds(0.5f);

        EncountEvent?.Invoke();
        yield return new WaitUntil(() => note.IsEnded);

        CurState = State.Attack;

        yield break;
    }

    public void StartAttack()
    {
        if (!IsAlive || IsGroggy)
        {
            return;
        }

        CurState = State.Attack;
    }

    private void AttackEntered()
    {
        NoteSystem.Instance().HitZone.IsAttackLocked = false;
        UIManager.Instance().GetController<CombatUIController>().HideFadeEffect();
        curSprite.Value = sprites["Attack_" + curPhase];

        AttackEvent?.Invoke();
    }

    private IEnumerator OnAttack()
    {
        if (!NoteSystem.Instance().PatternProcesser.IsProcessing)
        {
            //yield return new WaitForSeconds(1f);
            NoteSystem.Instance().StartPatterns(Info.PatternIds);
        }

        yield return new WaitForSeconds(0.1f);
        yield break;
    }

    private void AttackExited()
    {
        NoteSystem.Instance().StopAllPatterns();
    }

    public override Damaged CalculateDamaged(Damaged damaged)
    {
        // 방어력 100 -> 일반 공격력의 50% 데미지.
        int attackDmg = damaged.IgnoreDefense ? damaged.AttackDamage : Mathf.RoundToInt((float)damaged.AttackDamage * (100f / (100f + Stats.DefensePower)));

        // 현재 체력 100% -> 체간 공격력의 25% 데미지, 체력이 10% 감소할 때마다 데미지 15%씩 증가
        int balanceDmg;
        float hpPercent = (float)Stats.CurHealth / (float)Stats.MaxHealth;
        if (hpPercent <= 0.5f)
        {
            balanceDmg = damaged.BalanceDamage;
        }
        else
        {
            float reduction = 0.75f;
            int thresholdsPassed = (int)((1f - hpPercent) / 0.1f);
            float reductionRelieved = thresholdsPassed * 0.15f;
            reduction = Mathf.Clamp01(reduction - reductionRelieved);
            balanceDmg = Mathf.RoundToInt((float)damaged.BalanceDamage * (1f - reduction));
        }

        return new Damaged
        {
            AttackDamage = attackDmg,
            BalanceDamage = balanceDmg,
            Attacker = damaged.Attacker,
            Victim = damaged.Victim,
            IgnoreDefense = damaged.IgnoreDefense
        };
    }

    public override void OnDamaged(Damaged damaged)
    {
        base.OnDamaged(damaged);

        if (damagedCoroutine != null)
        {
            StopCoroutine(damagedCoroutine);
            damagedCoroutine = null;
        }

        damagedCoroutine = StartCoroutine(ProcessDamaged());
    }

    private IEnumerator ProcessDamaged()
    {
        Image image = UIManager.Instance().GetController<CombatUIController>().EnemyImage;
        yield return image.DOColor(new Color(1f, 0f, 0f), 0.5f);
        yield return new WaitForSeconds(0.5f);
        yield return image.DOColor(new Color(1f, 1f, 1f), 0.6f);

        yield break;
    }

    public override void OnGroggy()
    {
        base.OnGroggy();
        CurState = State.Groggy;
    }

    private void GroggyEntered()
    {
        curSprite.Value = sprites["Groggy_" + curPhase];
        SoundManager.Instance().Play(GameConstants.Sound.ENEMY_BALANCE_DOWN);

        NoteSystem.Instance().HitZone.IsAttackLocked = true;
        NoteSystem.Instance().StopAllPatterns();
        NoteSystem.Instance().MakeNote(9999, 7, 1000);
    }

    public override void OffGroggy()
    {
        base.OffGroggy();
        if (PrevState != State.Dead && CurState != State.Dead)
        {
            CurState = State.Attack;
        }
    }

    private void GroggyExited()
    {
        if (PrevState != State.Dead && CurState != State.Dead)
        {
            curSprite.Value = sprites["Idle_" + curPhase];
            Stats.CurBalanceGauge = Mathf.RoundToInt((float)Stats.MaxBalanceGauge * 0.1f);
        }

        NoteSystem.Instance().HitZone.IsAttackLocked = false;
    }

    private IEnumerator OnChangedPhase()
    {
        if (IsGroggy)
        {
            OffGroggy();
        }

        curPhase++;
        PhaseChangeEvent?.Invoke();
        NoteSystem.Instance().HitZone.IsAttackLocked = true;

        yield return new WaitForSeconds(2f);

        Info = EntityManager.Instance().GetEntityInfo(Id);
        Stats.UpdateStats(Info);

        NoteSystem.Instance().HitZone.IsAttackLocked = false;

        CurState = State.Attack;
        yield break;
    }

    public override void OnDeath()
    {
        NoteSystem.Instance().StopAllPatterns();

        if (curPhase >= totalPhase)
        {
            DoDeath();
        }
        else
        {
            ChangePhase();
        }
    }

    private void DoDeath()
    {
        CurState = State.Dead;

        PlayerContext playerContext = PlayerManager.Instance().LocalContext;
        if (!playerContext.KilledEnemies.ContainsKey(ids[0]))
        {
            playerContext.KilledEnemies[ids[0]] = 1;
            if (ids[0] == 404)
            {
                playerContext.Dialogues[DialogueKey.MerchantFirstBossClear.ToId()] = false;
                playerContext.Dialogues[DialogueKey.NunFirstBossClear.ToId()] = false;
            }
            else if (ids[0] == 408)
            {
                playerContext.Dialogues[DialogueKey.MerchantSecondBossClear.ToId()] = false;
                playerContext.Dialogues[DialogueKey.NunSecondBossClear.ToId()] = false;
            }
            else if (ids[0] == 396)
            {
                playerContext.Dialogues[DialogueKey.MerchantThirdBossClear.ToId()] = false;
                playerContext.Dialogues[DialogueKey.NunThirdBossClear.ToId()] = false;
            }
            else if (ids[0] == 394)
            {
                playerContext.Dialogues[DialogueKey.MerchantFinalBossClear.ToId()] = false;
                playerContext.Dialogues[DialogueKey.NunFinalBossClear.ToId()] = false;
            }
        }
        else
        {
            playerContext.KilledEnemies[ids[0]] += 1;
        }

        base.OnDeath();
        PlayerManager.Instance().OnContextChanged();
    }

    public void ChangePhase()
    {
        CurState = State.ChangingPhase;
    }

    private IEnumerator DeathDoing()
    {
        CameraManager.Instance().StartExecution();
        yield return new WaitForSecondsRealtime(1.5f);

        CombatSystem.Instance().EndCombat();
        PlayerManager.Instance().LocalPlayer.AddGold(Stats.Gold);

        yield return new WaitUntil(() => IsAlive);
        CurState = State.Idle;

        yield break;
    }

    protected override bool IsAIEnded()
    {
        return false;
    }

    protected override bool IsTerminalState(State state)
    {
        return false;
    }

    public int GetTotalPhase()
    {
        return totalPhase;
    }

    public int GetCurPhase()
    {
        return curPhase;
    }

    public bool IsBoss()
    {
        if (totalPhase >= 2)
        {
            return true;
        }
        return false;
    }
}

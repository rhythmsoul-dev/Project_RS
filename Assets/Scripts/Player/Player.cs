using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 플레이어 관련 컴포넌트, 데이터 관리만을 하는 클래스
/// </summary>
public class Player : Entity
{
    protected override int Id => 1;

    // Reset, Awake에서 초기화하는 로직이 있으니 SerializeField는 제거하고 프로퍼티만 남기는 구조로 리팩토링하는 것도 고려해볼 수 있을 것 같습니다
    [Header("입력, 이동 관련")]
    public Rigidbody2D PlayerRigidBody { get; private set; }
    public PlayerInteraction Interaction {get; private set;}
    public PlayerInput InputSystem { get; private set; }
    public PlayerInputs Input { get; private set; }

    private List<SheetMusic> sheetMusics;
    
    private List<Memo> memos;

    public bool IsLocked { get; set; }

    public event Action EnteredGroggy;
    public event Action ExitedGroggy;
    
    public new PlayerStats Stats
    {
        get => base.Stats as PlayerStats;
        private set => base.Stats = value;
    }

    public Action<int> UsedPotion;
    
    private void Reset()
    {
        InitComponents();
    }

    protected override void OnAwake()
    {
        InitComponents();
        PlayerManager.Instance().LocalPlayer = this;
    }

    private void Start()
    {
        StartCoroutine(ProcessBalanceHeal());
    }

    public void Init()
    {
        sheetMusics = PlayerManager.Instance().LocalContext.SheetMusics;
        memos = PlayerManager.Instance().LocalContext.Memo;
        Stats = PlayerManager.Instance().LocalContext.Stats;
        Stats.SetStats();
        Revive();
    }

    private void InitComponents()
    {
        PlayerRigidBody = GetComponent<Rigidbody2D>();
        Interaction = GetComponentInChildren<PlayerInteraction>();
        InputSystem = GetComponent<PlayerInput>();
        Input = GetComponent<PlayerInputs>();
    }

    public void AddGold(long gold)
    {
        Stats.Gold += gold;
        OnContextChanged();
    }

    private IEnumerator ProcessBalanceHeal()
    {
        while (IsAlive)
        {
            while (IsDamaged || Stats.CurBalanceGauge >= Stats.TotalBalance)
            {
                yield return null;
            }

            yield return new WaitForSeconds(1f);

            float hpPercent = (float)Stats.CurHealth / (float)Stats.TotalHealth;
            int amount = hpPercent <= 0.4f ? 1 : Mathf.FloorToInt(hpPercent / 0.2f) - 1; // <=40% -> 1, 60% -> 2, 80% -> 3, 100% -> 4

            Stats.CurBalanceGauge += amount;
        }

        yield break;
    }

    public override Damaged CalculateDamaged(Damaged damaged)
    {
        // 방어력 100 -> 일반 공격력의 50% 데미지
        int attackDmg = damaged.IgnoreDefense ? damaged.AttackDamage : Mathf.RoundToInt((float)damaged.AttackDamage * (100f / (100f + Stats.DefensePower)));

        return new Damaged
        {
            AttackDamage = attackDmg,
            BalanceDamage = damaged.BalanceDamage,
            Attacker = damaged.Attacker,
            Victim = damaged.Victim,
            IgnoreDefense = damaged.IgnoreDefense
        };
    }

    public override void OnDamaged(Damaged damaged)
    {
        base.OnDamaged(damaged);

        float damage = damaged.AttackDamage + damaged.BalanceDamage;
        CameraManager.Instance().ShakeCamera(Mathf.Clamp(damage * 2.5f, 2f, 20f), 0.1f);

        if (damage > 0f)
        {
            UIManager.Instance().GetController<CombatUIController>().ShowVignetteEffect();
        }

        OnContextChanged();
    }

    public override void OffGroggy()
    {
        base.OffGroggy();
        ExitedGroggy?.Invoke();
        SoundManager.Instance().SetUnderwater(false);
    }

    public override void OnGroggy()
    {
        base.OnGroggy();
        EnteredGroggy?.Invoke();
        SoundManager.Instance().SetUnderwater(true);
        
        StartCoroutine(GroggyRoutine());
    }

    private IEnumerator GroggyRoutine()
    {
        yield return new WaitForSeconds(3f);
        
        OffGroggy();
    }

    public override void OnDeath()
    {
        base.OnDeath();
        CombatSystem.Instance().EndCombat();
        Invoke(nameof(DelayedDeathEvent), 1f);
        
        Stats.Gold = (long)Math.Round((decimal)Stats.Gold * 0.5m);
        
        if (!PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.MerchantFirstDeath.ToId(), out _))
        {
            PlayerManager.Instance().LocalContext.Dialogues[DialogueKey.MerchantFirstDeath.ToId()] = false;
        }
        if (!PlayerManager.Instance().LocalContext.Dialogues.TryGetValue(DialogueKey.NunFirstDeath.ToId(), out _))
        {
            PlayerManager.Instance().LocalContext.Dialogues[DialogueKey.NunFirstDeath.ToId()] = false;
        } 
        PlayerManager.Instance().LocalContext.DeathCount++;
        OnContextChanged();
    }

    private void DelayedDeathEvent()
    {
        UIManager.Instance().ShowController<DefeatUIController>();
    }

    public override void Revive()
    {
        if (IsGroggy)
        {
            OffGroggy();
        }

        Stats.CurHealth = Stats.TotalHealth;
        Stats.CurBalanceGauge = Stats.TotalBalance;
        SetAlive(true);
    }

    public void OnContextChanged()
    {
        PlayerManager.Instance().LocalContext.Stats = Stats;
        PlayerManager.Instance().LocalContext.SheetMusics = sheetMusics;
        PlayerManager.Instance().LocalContext.Memo = memos;

        PlayerManager.Instance().OnContextChanged();
    }

    public void AddSheetMusic(SheetMusic sheetMusic)
    {
        sheetMusics.Add(sheetMusic);
        OnContextChanged();
    }

    public void AddMemo(Memo memoData)
    {
        foreach (var memo in memos)
        {
            if (memo.memoId == memoData.memoId)
            {
                return;
            }
        }
        if (memoData.memoId == "5")
        {
            PlayerManager.Instance().LocalContext.Dialogues[DialogueKey.MerchantMemo5.ToId()] = false;
            Debug.Log("5들어감");
        }
        else if (memoData.memoId == "10")
        {
            PlayerManager.Instance().LocalContext.Dialogues[DialogueKey.MerchantMemo10.ToId()] = false;
        }
        else if (memoData.memoId == "4")
        {
            PlayerManager.Instance().LocalContext.Dialogues[DialogueKey.NunMemo4.ToId()] = false;
        }
        else if (memoData.memoId == "8")
        {
            PlayerManager.Instance().LocalContext.Dialogues[DialogueKey.NunMemo8.ToId()] = false;
        }
        memos.Add(memoData);
        OnContextChanged();
    }
}

using System;
using System.Collections;
using UnityEngine;

public abstract class Entity : MonoBehaviour, IDamagable
{
    protected abstract int Id { get; }

    public EntityInfo Info { get; protected set; }
    public virtual EntityStats Stats { get; protected set; }

    public bool IsAlive { get; private set; }
    public bool IsDamaged { get; private set; }
    public bool IsGroggy { get; private set; }

    public event Action DamagedEvent;
    public event Action GroggyEvent;
    public event Action DeathEvent;

    private void Awake()
    {
        SetAlive(true);

        OnAwake();
    }

    protected virtual void OnAwake()
    {

    }

    private IEnumerator Start()
    {
        // 플레이어는 PlayerManager에서 정보 가져옴
        if (Id == 1)
        {
            yield break;
        }

        yield return new WaitUntil(() => NetworkManager.Instance().IsInitialized);

        Info = EntityManager.Instance().GetEntityInfo(Id);
        Stats = new EntityStats(Info);

        yield return StartCoroutine(OnStart());
        yield break;
    }

    protected virtual IEnumerator OnStart()
    {
        yield break;
    }

    public abstract Damaged CalculateDamaged(Damaged damaged);

    public virtual void OnDamaged(Damaged damaged)
    {
        Stats.CurHealth -= damaged.AttackDamage;
        Stats.CurBalanceGauge -= damaged.BalanceDamage;

        //Debug.Log($"데미지를 {damaged.Value}만큼 입었습니다. 적의 현재체력은 {Stats.CurHealth}입니다.");

        CancelInvoke(nameof(OffDamaged));
        IsDamaged = true;
        DamagedEvent?.Invoke();

        Invoke(nameof(OffDamaged), 0.4f);
        
        if (Stats.CurHealth <= 0 && IsAlive)
        {
            OnDeath();
            return;
        }

        if (Stats.CurBalanceGauge <= 0 && !IsGroggy)
        {
            OnGroggy();
        }
    }

    public virtual void OffDamaged()
    {
        IsDamaged = false;
    }

    public virtual void OnGroggy()
    {
        IsGroggy = true;
        GroggyEvent?.Invoke();
    }

    public virtual void OffGroggy()
    {
        IsGroggy = false;
    }

    public virtual void OnDeath()
    {
        if (IsGroggy)
        {
            OffGroggy();
        }

        SetAlive(false);
        DeathEvent?.Invoke();
    }

    public void SetAlive(bool alive)
    {
        if (IsAlive != alive)
        {
            IsAlive = alive;
        }
    }

    public virtual void Revive()
    {
        if (IsGroggy)
        {
            OffGroggy();
        }

        Stats.CurHealth = Stats.MaxHealth;
        Stats.CurBalanceGauge = Stats.MaxBalanceGauge;
        SetAlive(true);
    }
}

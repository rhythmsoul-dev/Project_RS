using System;
using System.Collections.Generic;
using UnityEngine;

public class DamagedProcesser
{
    public event Action<Damaged> Damaged;

    private Queue<Damaged> damagedQueue = new Queue<Damaged>();

    public void Add(Damaged damaged)
    {
        damagedQueue.Enqueue(damaged);
    }

    public void Update()
    {
        ProcessDamagedQueue();
    }

    private void ProcessDamagedQueue()
    {
        while (damagedQueue.Count > 0)
        {
            Damaged next = damagedQueue.Dequeue();
            Process(next);
        }
    }

    private void Process(Damaged damaged)
    {
        if (damaged.Victim.TryGetComponent(out IDamagable victim))
        {
            victim.OnDamaged(victim.CalculateDamaged(damaged));
        }

        Damaged?.Invoke(damaged);
    }
}

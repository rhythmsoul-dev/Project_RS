using System;
using UnityEngine;

public class DamagedSystem : GameSystem<DamagedSystem>
{
    private DamagedProcesser damagedProcesser = new DamagedProcesser();

    public void Send(Damaged damaged)
    {
        damagedProcesser.Add(damaged);
        damagedProcesser.Update();
    }
}

using UnityEngine;

public interface IDamagable
{
    Damaged CalculateDamaged(Damaged damaged);

    void OnDamaged(Damaged damaged);
}

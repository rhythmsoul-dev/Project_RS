using UnityEngine;

[CreateAssetMenu(fileName = "EntityData_", menuName = "Scriptable Objects/Entity Data")]
public class EntityData : ScriptableObject
{
    [Header("정보")]
    public int Id;
    public string Description;

    [Header("스탯")]
    public int BaseMaxHealth;
    public float BaseAttackPower;
    public float BaseAttackSpeed;
    public float BaseDefensePower;
    public int BaseBalanceGauge;
    public float BaseMoveSpeed;

    [Header("보유 정보")]
    public long Gold;
}

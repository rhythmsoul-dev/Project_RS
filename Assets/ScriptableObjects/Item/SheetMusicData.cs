using UnityEngine;

[CreateAssetMenu(fileName = "SheetMusicData_", menuName = "Scriptable Objects/SheetMusic Data")]
public class SheetMusicData : ScriptableObject
{
    public int Id;
    public string Name;
    public string Description;
    public string Type; // Base, Mid, Pro 
    public int AttackBonus;
    public int HealthBonus;
    public int BalanceGaugeBonus;
    public int Price;
    public string IconPath;
}
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShopInventory_", menuName = "Scriptable Objects/Shop Inventory")]
public class ShopInventory : ScriptableObject
{
    public List<SheetMusicData> ItemsForSale;
}
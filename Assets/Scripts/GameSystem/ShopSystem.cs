using System;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ShopSystem : GameSystem<ShopSystem>
{
    private const string path = "ScriptableObjects/ShopInventory_1.asset";
    private ShopInventory shopInventory;

    public void LoadShopInventory(Action<ShopInventory> onLoaded = null)
    {
        if (shopInventory != null)
        {
            onLoaded?.Invoke(shopInventory);
            return;
        }

        Addressables.LoadAssetAsync<ShopInventory>(path).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                Debug.Log("ShopInventory 로드 성공");
                shopInventory = handle.Result;
                onLoaded?.Invoke(shopInventory);
            }
            else
            {
                Debug.LogErrorFormat("[Addressable] Cannot Found Path: {0}", path);
            }
        };
    }

    public int TryBuyItem(SheetMusic selectedShopItem) // 1 -> 성공, 2 -> 돈 부족, 3 -> 이미 소유 
    {
        foreach (SheetMusic sheet in PlayerManager.Instance().LocalContext.SheetMusics) // 중복소유불가
        {
            if (sheet.Id == selectedShopItem.Id) return 3;
        }
        if (PlayerManager.Instance().LocalContext.Stats.Gold >= selectedShopItem.Price)
        {
            PlayerManager.Instance().LocalContext.Stats.Gold -= selectedShopItem.Price;
            PlayerManager.Instance().LocalContext.SheetMusics.Add(selectedShopItem);
            PlayerManager.Instance().OnContextChanged();
            
            return 1;
        }

        return 2;
    }
}

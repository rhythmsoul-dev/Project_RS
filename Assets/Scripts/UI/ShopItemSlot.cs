using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemSlot : MonoBehaviour
{
    [SerializeField] private TMP_Text shopItemDescription;
    [SerializeField] private TMP_Text shopItemPrice;
    [SerializeField] private Image shopItemIcon;
    [SerializeField] private Button shopItemButton;
    [SerializeField] private GameObject shopSoldOutBG;

    public void SetShopItemSlot(string description, int id, long price, Sprite icon)
    {
       shopItemDescription.SetText(description);
       shopItemPrice.SetText(price.ToString());
       shopItemIcon.sprite = icon;

       if (PlayerManager.Instance().LocalContext.SheetMusics.Any(sheetMusic => sheetMusic.Id == id))
       {
           shopSoldOutBG.SetActive(true);
       }
       else
       {
           shopSoldOutBG.SetActive(false);
       }
    }

    public Button GetButton()
    {
        return shopItemButton;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SheetMusicSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    
    public void SetIcon(Sprite iconData)
    {
        icon.sprite = iconData;
    }
}

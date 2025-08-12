using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LifeSlots : MonoBehaviour
{
    [SerializeField] private Image[] slots;
    private List<Image> activeSlots = new List<Image>();

    [SerializeField] private Sprite activePhaseSprite;
    [SerializeField] private Sprite inactivePhaseSprite;

    public void Init(int totalPhase)
    {
        activeSlots.Clear();

        if (totalPhase <= 1)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                slots[i].gameObject.SetActive(false);
            }

            return;
        }

        for (int i = 0; i < totalPhase; i++)
        {
            slots[i].gameObject.SetActive(true);
            slots[i].sprite = activePhaseSprite;
            activeSlots.Add(slots[i]);
        }
    }

    public void UpdateSlots(int curPhase)
    {
        for (int i = 0; i < activeSlots.Count; i++)
        {
            if (i < activeSlots.Count - (curPhase - 1))
            {
                activeSlots[i].sprite = activePhaseSprite;
            }
            else
            {
                activeSlots[i].sprite = inactivePhaseSprite;
            }
        }
    }
}

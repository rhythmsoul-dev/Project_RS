using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Toast : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI messageText;

    public bool IsShowing { get; private set; }

    public void Show(string msg, Color color)
    {
        IsShowing = true;
        messageText.text = msg;
        messageText.color = color;
        UIManager.Instance().FadeIn(canvasGroup, 0.1f);
    }

    public void Hide()
    {
        IsShowing = false;
        UIManager.Instance().FadeOut(canvasGroup, 0.1f);
    }
}

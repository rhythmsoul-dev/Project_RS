using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


[RequireComponent(typeof(Image))]
public class AlphaHitTestEnabler : MonoBehaviour
{
    void Awake()
    {
        Image img = GetComponent<Image>();
        img.alphaHitTestMinimumThreshold = 0.2f; // 알파값 0.2 이상일 때만 마우스오버감지
    }
}
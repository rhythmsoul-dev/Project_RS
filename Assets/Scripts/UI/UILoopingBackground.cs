using System.Numerics;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;

public class UILoopingBackground : MonoBehaviour
{
    [SerializeField] private RectTransform[] backgrounds;
    [SerializeField] private float scrollSpeed = 100f;

    private float imageWidth;

    private void Start()
    {
        imageWidth = backgrounds[0].rect.height;
    }

    private void Update()
    {
        foreach (var bg in backgrounds)
        {
            bg.anchoredPosition -= Vector2.right * (scrollSpeed * Time.deltaTime);
            
            if (bg.anchoredPosition.x <= -imageWidth)
            {
                bg.anchoredPosition += Vector2.right * (imageWidth * 2);
            }
            else if (bg.anchoredPosition.x >= imageWidth)
            {
                bg.anchoredPosition += Vector2.left * (imageWidth * 2);
            }
        }
    }
}

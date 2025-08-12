using UnityEngine;
using UnityEngine.EventSystems;

public class UIHoverSound : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private string soundName;
    
    private float lastPlayTime;
    private float minInterval = 0.1f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Time.time - lastPlayTime < minInterval)
        {
            return;
        }

        SoundManager.Instance().Play(soundName);
        lastPlayTime = Time.time;
    }
}

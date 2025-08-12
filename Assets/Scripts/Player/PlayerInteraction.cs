using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 플레이어의 상호작용 만을 관리하는 클래스
/// </summary>
public class PlayerInteraction : MonoBehaviour
{
    [SerializeField] private List<InteractableObject> interactables = new List<InteractableObject>();
    [SerializeField] private GameObject keyGuide;
    public GameObject KeyGuide => keyGuide;
    public InteractableObject ClosestInteractable { get; private set; }

    /// <summary>
    /// 상호작용 입력시 발동할 함수
    /// </summary>
    public void OnInteract()
    {
        switch (ClosestInteractable.ObjectType)
        {
            case ObjectType.Portal:
            case ObjectType.Heal:
                break;
            default:
                interactables.Remove(ClosestInteractable);
                break;
        }
        
        ClosestInteractable.HideOutline();
        ClosestInteractable.OnInteract();

        ShowOutline();
        ShowKeyGuide();
    }
    
    /// <summary>
    /// 상호작용 오브젝트의 유무와 가장 가까운 상호작용 오브젝트를 찾아오는 함수
    /// </summary>
    /// <returns></returns>
    public bool HasInteractionObject()
    {
        //우선 최대값으로 처음 찾을때를 준비
        float closestDistance = float.MaxValue;
        ClosestInteractable = null;
        
        foreach (var interactable in interactables)
        {
            //거리를 재고
            float distance = Vector2.Distance(transform.position, interactable.transform.position);
            
            //처음 제외 가장 가까운 오브젝트로 등록된 오브젝트의 거리보다 먼 경우 넘어가고
            if (!(distance < closestDistance)) continue;
            
            //더 가까우면 해당 오브젝트를 가장 가까운 오브젝트로 등록
            closestDistance = distance;
            ClosestInteractable = interactable;
        }
        
        //가장 가까운 오브젝트를 등록할 수 있다면(하나라도 리스트에 있다면) true반환
        return ClosestInteractable != null;
    }

    private void ShowOutline()
    {
        if (!HasInteractionObject()) return;
        if (ClosestInteractable.Interacted) return;
        
        ClosestInteractable.ShowOutline();
        
        foreach (var interactable in interactables.Where(interactable => interactable != ClosestInteractable))
        {
            interactable.HideOutline();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.TryGetComponent<InteractableObject>(out var interactable)) return;
        
        //오브젝트 종류 구분
        switch (interactable.ObjectType)
        {
            //적 오브젝트면 바로 발동
            case ObjectType.Enemy :
                interactable.OnInteract();
                break;
            default:
                if (interactable.Interacted)
                {
                    return;
                }
                interactables.Add(interactable);
                ShowOutline();
                ShowKeyGuide();
                break;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!other.TryGetComponent<InteractableObject>(out var interactable)) return;

        if (interactables.Contains(interactable))
        {
            interactables.Remove(interactable);
            interactable.HideOutline();  
        }
        ShowOutline();
        ShowKeyGuide();
    }

    private void ShowKeyGuide()
    {
        keyGuide.gameObject.SetActive(ClosestInteractable != null);
    }
}

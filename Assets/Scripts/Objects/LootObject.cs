using System.Linq;
using UnityEngine;

/// <summary>
/// 플레이어가 상호작용할 오브젝트 클래스
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class LootObject : InteractableObject
{
    protected Animator lootAnimator;
    
    [Header("획득 아이템 설정")]
    [SerializeField] protected LootData lootData;
    
    [Header("획득시 사운드"), Tooltip("사운드 이름 종류 : OpenChest / GetNote")]
    protected string soundName;
    
    [SerializeField] private bool dontDisable = false;

    protected override void Init()
    {
        ObjectType = ObjectType.Loot;
        
        switch (lootData.Type)
        {
            case LootType.Memo:
                soundName = GameConstants.Sound.GET_NOTE;
                break;
            default:
                soundName = GameConstants.Sound.OPEN_CHEST;
                break;
        }
        
        outLine = transform.FindComponent<SpriteRenderer>("OutLine");
        outLine.enabled = false;
        transform.TryGetComponent<Animator>(out lootAnimator);
        if (PlayerManager.Instance().LocalContext.LootDataIDs.Contains(lootData.LootID))
        {
            Interacted = true;
            
            if (lootData.Type == LootType.Memo && !dontDisable)
            { 
                gameObject.SetActive(false);
            }
            
            lootAnimator?.SetTrigger("Interact");
        }
    }

    public override void OnInteract()
    {
        if (Interacted)
        {
            AlreadyInteracted();
            return;
        }

        Interacted = true;
        
        SoundManager.Instance().Play(soundName);

        Player localPlayer = PlayerManager.Instance().LocalPlayer;
        
        switch (lootData.Type)
        {
            case LootType.Gold:
                localPlayer.AddGold(lootData.Gold);
                break;
            case LootType.Memo:
                Memo memo = new Memo(lootData.Memo);
                localPlayer.AddMemo(memo);
                if(!dontDisable)
                {
                    gameObject.SetActive(false);
                }
                break;
            case LootType.Potion:
                localPlayer.Stats.AddMaxPotion(lootData.PotionAmount);
                break;
            case LootType.Sheet:
            default:
                Debug.Log("해당 타입 로직 없음");
                break;
        }
        
        PlayerManager.Instance().LocalContext.LootDataIDs.Add(lootData.LootID);
        
        UIManager.Instance().GetController<LogUIController>().ShowLog(lootData);
        
        lootAnimator?.SetTrigger("Interact");
    }

    protected void AlreadyInteracted()
    {
        Debug.Log($"{gameObject.name}은(는) 이미 상호작용함");
    }
}
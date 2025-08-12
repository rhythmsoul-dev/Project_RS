using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PlayerDetector))]
public class PortalObject : InteractableObject
{
    [Header("포탈 정보 지정")]
    [SerializeField] private string mapName;
    [SerializeField, Range(0, 10)] private int portalIndex;
    [SerializeField] private MapPortalData mapPortalData;

    [Header("즉시 전송 포탈")]
    [SerializeField] private bool forcedTransfer;
    [SerializeField] private bool toNextStage;
    
    protected PlayerDetector detector;
    private EnemyObject[] enemies;
    private ParticleSystem particle;
    
    private Vector3 portalPosition;
    private ActivePortalData activePortalData = new ActivePortalData();
    
    public Action InteractionAction;

    protected override void OnReset()
    { 
        SetPortal();
    }

    private void SetPortal()
    {
        //이미 존재하는 포탈 다 찾아오기
        PortalObject[] existingPortals = FindObjectsByType<PortalObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        //이미 사용중인 번호 저장용 리스트
        HashSet<int> usedIndices = new HashSet<int>();
        //이미 존재하는 인덱스 저장(자신 제외)
        foreach (var portal in existingPortals)
        {
            if (portal != this)
            {
                usedIndices.Add(portal.portalIndex);
            }
        }
        
        int newIndex = -1;
        //미사용 인덱스 찾기
        for (int i = 0; i <= 10; i++)
        {
            if (!usedIndices.Contains(i))
            {
                newIndex = i;
                break;
            }
        }
        //미사용 인덱스가 없다면 넘어가기
        if (newIndex == -1)
        {
            Debug.LogWarning("포탈 인덱스 전부 사용중.");
            return;
        }
        //포탈 인덱스 부여하고 이름 자동 변경
        portalIndex = newIndex;
        gameObject.name = $"Portal_{portalIndex}";
        
        //가져올 포탈 위치 데이터와 맵의 이름이 일치해야함.
        mapName = SceneManager.GetActiveScene().name;
        
        var handle = Addressables.LoadAssetAsync<MapPortalData>($"SO/MapPortalData/{mapName}");
        
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            mapPortalData = handle.Result;
            Debug.Log($"불러오기 성공: {mapPortalData.name}");
            //포탈 인덱스와 같은 포탈을 찾음
            if (mapPortalData.TryGetPortal(portalIndex, out var portal))
            {
                //있다면 해당 위치로 위치 변경
                portalPosition = transform.position;
                portalPosition.x = portal.Location.x;
                transform.position = portalPosition;
            }
            else
            {
                //없다면 새로운 포탈 위치 추가
                Vector3 newPortalPosition = new Vector3(transform.position.x, 2.5f, portalPosition.z);
                mapPortalData.AddPortal(portalIndex, newPortalPosition, false);
            }
        }
        else
        {
            Debug.LogError("MapPortalData 로딩 실패");
        }
    }

    protected override void Init()
    {
        ObjectType = ObjectType.Portal;
        detector = GetComponent<PlayerDetector>();
        detector.Detected.Changed = DetectPlayer;
        particle = GetComponentInChildren<ParticleSystem>();
        particle.Stop();
        if (mapPortalData != null)
        {
            activePortalData.SetData(mapPortalData.MapType, mapPortalData.MapIndex, portalIndex);
        }
    }

    private void Start()
    {
        enemies = FindObjectsByType<EnemyObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
    }

    public override void OnInteract()
    {
        InteractionAction?.Invoke();
        
        if (forcedTransfer)
        {
            if (toNextStage)
            {
                PortalManager.Instance().MoveToPortal(mapPortalData.MapType, mapPortalData.MapIndex+1);
                return;
            }

            PortalManager.Instance().MoveToPortal();
            return;
        }
        
        SoundManager.Instance().Play(GameConstants.Sound.USE_PORTAL);
        
        PlayerManager.Instance().FullHeal();
        PlayerManager.Instance().LocalContext.Stats.RefillPotion();
        
        if (!PlayerManager.Instance().LocalContext.ActivePortals.Contains(activePortalData))
        {
            PlayerManager.Instance().LocalContext.ActivePortals.Add(activePortalData);
            UIManager.Instance().GetController<PortalUIController>().UpDateFirstActivePortal();
        }
        
        UIManager.Instance().ShowController<PortalUIController>();

        ReviveAllEnemies();
        
        PlayerManager.Instance().OnContextChanged();
    }
    
    protected virtual void DetectPlayer(bool detect)
    {
        switch (detect)
        {
            case true:
                particle.Play();
                break;
            case false:
                particle.Stop();
                break;
        }
    }

    private void ReviveAllEnemies()
    {
        foreach (var enemy in enemies)
        {
            enemy.NormalEnemy.Revive();
            enemy.EnemyCollider.enabled = true;
        }
    }
}

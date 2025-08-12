using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class PortalManager : Singleton<PortalManager>
{
    //포탈 이동에 쓰는 변수
    private List<MapPortalData> mapPortalDatas;
    private Vector3 movePosition;
    private ActivePortalData checkingPortal = new ActivePortalData();
    private string loadSceneName;

    public async Task Init()
    {
        var handle = Addressables.LoadAssetsAsync<MapPortalData>("MapPortalData");
        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            mapPortalDatas = handle.Result.ToList();
            Debug.Log($"MapPortalData {mapPortalDatas.Count}개 로드 완료");
        }
        else
        {
            Debug.LogError("MapPortalData Addressables 로드 실패");
        }
    }
    
    public void MoveToPortal(MapType mapType = MapType.Town, int mapIndex = 0, int portalIndex = 0)
    {
        if (mapType == MapType.Town)
        {
            SceneLoader.Instance().LoadScene(GameConstants.Scene.VILLAGE_SCENE);
            return;
        }
        
        //맵 타입과 넘버링 구분
        MapPortalData curPortalData = mapPortalDatas.Find(p => p.MapType == mapType && p.MapIndex == mapIndex);
        
        if (curPortalData == null)
        {
            Debug.LogError($"{mapType.ToString()}유형의 {mapIndex}에 해당하는 맵이 존재하지 않습니다.");
            return;
        }
        
        if (portalIndex >= curPortalData.PortalLocations.Count || portalIndex < 0)
        {
            Debug.LogError($"해당하는 맵의 {portalIndex}번째 포탈이 존재하지 않습니다.");
            return;
        }
        
        if (!CheckActivePortal(mapType, mapIndex, portalIndex))
        {
            Debug.LogError($"활성화 되지 않은 {mapType.ToString()}유형의 {mapIndex}번째 맵에 있는 {portalIndex}번째 포탈로 접근하고 있습니다.");
            return;
        }
        
        movePosition = curPortalData.PortalLocations[portalIndex].Location;

        switch (mapType)
        {
            case MapType.Graveyard:
                loadSceneName = GameConstants.Scene.GRAVEYARD_SCNEN;
                break;
            case MapType.Dungeon:
                switch (mapIndex)
                {
                    case 0:
                        loadSceneName = GameConstants.Scene.DUNGEON_0_SCENE;
                        break;
                    case 1:
                        loadSceneName = GameConstants.Scene.DUNGEON_1_SCENE;
                        break;
                }
                break;
        }
        
        //해당 씬과 다르다면 해당 씬 로드
        if (SceneLoader.Instance().LoadSceneName != loadSceneName)
        {
            if (SceneUtility.GetBuildIndexByScenePath($"Assets/Scenes/{loadSceneName}.unity") == -1)
            {
                Debug.LogError("해당씬이 존재하지 않거나 빌드세팅에 포함되지 않았습니다.");
                return;
            }
            
            SceneLoader.Instance().LoadScene(loadSceneName, OnSceneLoaded, mapType, mapIndex);
            return;
        }

        MovePlayer();
    }
    
    private void OnSceneLoaded()
    {
        StartCoroutine(DelayedMovePlayer());
    }
    
    private IEnumerator DelayedMovePlayer()
    {
        yield return new WaitForEndOfFrame();
        MovePlayer();
    }

    private void MovePlayer()
    {
        if (PlayerManager.Instance().LocalPlayer == null)
        {
            return;
        }

        PlayerManager.Instance().LocalPlayer.transform.position = movePosition;
    }

    private bool CheckActivePortal(MapType mapType, int mapIndex, int portalIndex)
    {
        checkingPortal.SetData(mapType, mapIndex, portalIndex);
        return PlayerManager.Instance().LocalContext.ActivePortals.Contains(checkingPortal);
    }
}

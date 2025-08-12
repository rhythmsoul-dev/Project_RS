using System;
using Michsky.UI.Dark;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class PortalUI : MonoBehaviour
{
    [SerializeField] private PortalUIController portalUIController;

    [SerializeField] private CanvasGroup canvasGroup;
    public CanvasGroup CanvasGroup => canvasGroup;

    [SerializeField] private UIDissolveEffect dissolveEffect;
    public UIDissolveEffect DissolveEffect => dissolveEffect;

    [SerializeField] private string portalDataPath;
    [SerializeField] private MapPortalData mapPortalData;
    public MapPortalData MapPortalData => mapPortalData;

    [SerializeField] private int portalIndex;
    public int PortalIndex => portalIndex;

    [SerializeField] private Button buttonManager;

    private void Reset()
    {
        Init();
    }

    private void Awake()
    {
        if (mapPortalData == null)
        {
            Init();
        }
    }

    public void ButtonInit()
    {
        buttonManager.onClick.RemoveAllListeners();
        buttonManager.onClick.AddListener(() =>
        {
            MovePortal();
            portalUIController.BackToMenu();
        });

        DissolveEffect.location = 1f;
        canvasGroup.interactable = false;
    }

    private void Init()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        dissolveEffect = GetComponent<UIDissolveEffect>();
        buttonManager = GetComponent<Button>();

        Transform parent = transform.parent;
        portalDataPath = $"SO/MapPortalData/{parent.gameObject.name}";
        portalUIController = parent.parent.parent.GetComponent<PortalUIController>();

        var handle = Addressables.LoadAssetAsync<MapPortalData>(portalDataPath);

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            mapPortalData = handle.Result;
        }
        else
        {
            Debug.LogError("버튼의 부모 이름이 잘못되었거나 해당 맵 데이터가 존재하지 않습니다.");
        }

        for (int i = 0; i < parent.childCount; i++)
        {
            if (parent.GetChild(i) == transform)
            {
                portalIndex = i;
            }
        }
    }

    private void MovePortal()
    {
        PortalManager.Instance().MoveToPortal(MapPortalData.MapType, MapPortalData.MapIndex, portalIndex);
    }
}

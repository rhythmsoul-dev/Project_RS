using System;
using UnityEngine;

public class PortalUnlocker : MonoBehaviour
{
    [SerializeField] private PortalObject portalObject;
    [SerializeField] private MapPortalData unlockPortalData;
    [SerializeField] private int unlockPortalIndex;
    private ActivePortalData activePortalData = new ActivePortalData();

    private void Reset()
    {
        Init();
    }

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        portalObject = GetComponent<PortalObject>();
        portalObject.InteractionAction = UnlockPortal;
        activePortalData.SetData(unlockPortalData.MapType, unlockPortalData.MapIndex, unlockPortalIndex);
    }

    private void UnlockPortal()
    {
        if (!PlayerManager.Instance().LocalContext.ActivePortals.Contains(activePortalData))
        {
            PlayerManager.Instance().LocalContext.ActivePortals.Add(activePortalData);
        }
    }
}

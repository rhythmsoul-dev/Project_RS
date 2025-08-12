using System.Collections;
using System.Collections.Generic;
using Michsky.UI.Dark;
using UnityEngine;


public class PortalUIController : UIControllerBase
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private UIDissolveEffect backGround;
    
    [SerializeField] private HorizontalSelector horizontalSelector;
    [SerializeField] private UIDissolveEffect[] buttonBackgrounds = new UIDissolveEffect[2];
    private MapType curSelectMapType = MapType.Graveyard;
    private bool isDissolveIn = false;
    private bool isDissolveOut = false;

    [SerializeField] private List<PortalUI> portalUIList;
    [SerializeField] private List<PortalUI> firstActivePortalList = new List<PortalUI>();
    private ActivePortalData curPortalData = new ActivePortalData();

    private WaitUntil waitForDissolveIn;
    private WaitUntil waitForDissolveOut;

    protected override void SetUp() { }

    public void Init()
    {
        waitForDissolveIn = new WaitUntil(() => isDissolveIn);
        waitForDissolveOut = new WaitUntil(() => isDissolveOut);
        
        ResetButtonBackgroundDissolveInEvent();
        ResetButtonBackgroundDissolveOutEvent();
        
        horizontalSelector.itemList[0].onItemSelect.RemoveAllListeners();
        horizontalSelector.itemList[0].onItemSelect.AddListener(() =>
        {
            isDissolveIn = false;
            curSelectMapType = MapType.Graveyard;
            buttonBackgrounds[0].gameObject.SetActive(true);
            if (buttonBackgrounds[1].gameObject.activeInHierarchy)
            {
                buttonBackgrounds[1].DissolveOut();
            }

            StartCoroutine(WaitForTransition(buttonBackgrounds[0]));
        });
        
        horizontalSelector.itemList[1].onItemSelect.RemoveAllListeners();
        horizontalSelector.itemList[1].onItemSelect.AddListener(() =>
        {
            isDissolveIn = false;
            curSelectMapType = MapType.Dungeon;
            buttonBackgrounds[1].gameObject.SetActive(true);
            if (buttonBackgrounds[0].gameObject.activeInHierarchy)
            {
                buttonBackgrounds[0].DissolveOut();
            }
            
            StartCoroutine(WaitForTransition(buttonBackgrounds[1]));
        });
        
        ResetBackgroundDissolveInEvent();
        ResetBackgroundDissolveOutEvent();
        
        foreach (var portalUI in portalUIList)
        {
            portalUI.ButtonInit();
        }
        
        UpDateFirstActivePortal();
    }

    private IEnumerator WaitForTransition(UIDissolveEffect effect)
    {
        yield return waitForDissolveOut;
        effect.DissolveIn();
        isDissolveOut = false;
    }

    private void ResetButtonBackgroundDissolveInEvent()
    {
        foreach (var buttonBackground in buttonBackgrounds)
        {
            buttonBackground.DissolveInOver = () =>
            {
                isDissolveIn = true;
            };
        }
    }

    private void ResetButtonBackgroundDissolveOutEvent()
    {
        foreach (var buttonBackground in buttonBackgrounds)
        {
            buttonBackground.DissolveOutOver = () =>
            {
                buttonBackground.gameObject.SetActive(false);
                isDissolveOut = true;
            };
        }
    }

    private void ResetBackgroundDissolveInEvent()
    {
        backGround.DissolveInOver = () =>
        {
            canvasGroup.interactable = true;
            StartCoroutine(CheckFirstActivePortal());
        };
    }

    private void ResetBackgroundDissolveOutEvent()
    {
        backGround.DissolveOutOver = () =>
        {
            gameObject.SetActive(false);
        };
    }

    public void UpDateFirstActivePortal()
    {
        foreach (var portalUI in portalUIList)
        {
            ActivePortalData existingPortal = FindActivePortal(
                portalUI.MapPortalData.MapType,
                portalUI.MapPortalData.MapIndex,
                portalUI.PortalIndex);

            if (existingPortal == null)
            {
                portalUI.DissolveEffect.location = 1f;
                portalUI.CanvasGroup.interactable = false;
                portalUI.gameObject.SetActive(false);
            }
            else if (existingPortal is { IsFirstActiveOver: false })
            {
                firstActivePortalList.Add(portalUI);
            }
            else if (existingPortal is { IsFirstActiveOver: true })
            {
                portalUI.gameObject.SetActive(true);
                portalUI.DissolveEffect.location = 0f;
                portalUI.CanvasGroup.interactable = true;
            }
        }
    }

    private ActivePortalData FindActivePortal(MapType mapType, int mapIndex, int portalIndex)
    {
        curPortalData.SetData(mapType, mapIndex, portalIndex);
            
        return PlayerManager.Instance().LocalContext.ActivePortals.Find(portal => portal.Equals(curPortalData));
    }

    public override void Show()
    {
        gameObject.SetActive(true);
        backGround.DissolveIn();
    }

    public override void Hide()
    {
        canvasGroup.interactable = false;
        backGround.DissolveOut();
    }

    private IEnumerator  CheckFirstActivePortal()
    {
        if (firstActivePortalList.Count == 0)
        {
            SwitchPortalSection();
            yield break;
        }
        
        canvasGroup.interactable = false;
        
        foreach (var portal in firstActivePortalList)
        {
            ActivePortalData existingPortal = FindActivePortal(portal.MapPortalData.MapType, portal.MapPortalData.MapIndex, portal.PortalIndex);
            
            portal.gameObject.SetActive(true);
            portal.DissolveEffect.DissolveInOver = () => portal.CanvasGroup.interactable = true;

            yield return waitForDissolveIn;
            
            yield return StartCoroutine(UnlockPortal(portal));

            existingPortal.IsFirstActiveOver = true;
        }
        
        firstActivePortalList.Clear();

        SwitchPortalSection();
        
        canvasGroup.interactable = true;
    }

    private void SwitchPortalSection()
    {
        if (curSelectMapType != SceneLoader.Instance().CurMapType)
        {
            switch (SceneLoader.Instance().CurMapType)
            {
                case MapType.Town:
                    if (curSelectMapType == MapType.Graveyard)
                    {
                        break;
                    }
                    horizontalSelector.PreviousClick();
                    break;
                case MapType.Graveyard:
                    horizontalSelector.PreviousClick();
                    break;
                case MapType.Dungeon:
                    horizontalSelector.ForwardClick();
                    break;
            }
        }
    }

    private IEnumerator UnlockPortal(PortalUI portal)
    {
        bool isFinished = false;
        portal.DissolveEffect.DissolveInOver += () => isFinished = true;
        
        if (portal.MapPortalData.MapType != curSelectMapType)
        {
            switch (curSelectMapType)
            {
                case MapType.Graveyard:
                    horizontalSelector.ForwardClick();
                    break;
                case MapType.Dungeon:
                    horizontalSelector.PreviousClick();
                    break;
            }

            yield return null;
        }
        
        yield return waitForDissolveIn;
        
        portal.DissolveEffect.DissolveIn();
        
        yield return new WaitUntil(() => isFinished);
        
        portal.DissolveEffect.DissolveInOver = null;
    }
    
    public void BackToMenu()
    {
        UIManager.Instance().HideController<PortalUIController>();
        
        if (SceneLoader.Instance().CurMapType == MapType.Town)
        {
            backGround.DissolveOutOver += () =>
            {
                UIManager.Instance().ShowController<ChurchMenuUIController>();
                backGround.DissolveOutOver = () => gameObject.SetActive(false);
                ResetBackgroundDissolveOutEvent();
            };
        }
    }

    public void BackToTown()
    {
        PortalManager.Instance().MoveToPortal();
    }
}

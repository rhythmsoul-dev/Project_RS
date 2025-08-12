using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class CombatSystem : GameSystem<CombatSystem>
{
    private const string combatObjPath = "CombatObject";
    private const string combatUIPath = "CombatUI";
    private GameObject combatObject;
    private Player Player => PlayerManager.Instance().LocalPlayer;
    public Enemy CurEnemy { get; private set; }
    private CombatUIController uiController;
    public CombatUIController CombatUI => uiController;
    public bool IsInCombat { get; private set; }

    public void Init()
    {
        Addressables.LoadAssetAsync<GameObject>(combatObjPath).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                combatObject = Instantiate(handle.Result);
            }
            else
            {
                Debug.LogErrorFormat("[Addressable] Cannot Found Path: {0}", combatObjPath);
            }
        };

        Addressables.LoadAssetAsync<GameObject>(combatUIPath).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                uiController = Instantiate(handle.Result).GetComponent<CombatUIController>();
                UIManager.Instance().RegisterController(uiController);
            }
            else
            {
                Debug.LogErrorFormat("[Addressable] Cannot Found Path: {0}", combatUIPath);
            }
        };
    }

    private void UpdateEnemyUI(int p0 = 0)
    {
        if (IsInCombat)
        {
            uiController.UpdateEnemyUI();
        }
    }

    private void UpdatePlayerUI(int p0 = 0)
    {
        if (IsInCombat)
        {
            uiController.UpdatePlayerUI();
        }
    }

    private void PhaseChangeUI()
    {
        if (IsInCombat)
        {
            uiController.ChangePhase(CurEnemy);
        }
    }
    
    public void StartCombat(Enemy enemy)
    {
        PlayerManager.Instance().SwitchPlayerInputState(PlayerInputState.Battle_State);
        
        Player.IsLocked = true;
        CurEnemy = enemy;

        uiController.Init(CurEnemy);
        IsInCombat = true;
        
        Player.Stats.ObservableCurHealth.Changed += UpdatePlayerUI;
        Player.Stats.ObservableBalanceGauge.Changed += UpdatePlayerUI;
        
        CurEnemy.Stats.ObservableCurHealth.Changed += UpdateEnemyUI;
        CurEnemy.Stats.ObservableBalanceGauge.Changed += UpdateEnemyUI;
        CurEnemy.PhaseChangeEvent += PhaseChangeUI;
        
        StartCoroutine(BattleIn());
        
        UpdatePlayerUI();
        UpdateEnemyUI();
        PhaseChangeUI();
    }

    private IEnumerator BattleIn()
    {
        SoundManager.Instance().StopBGM();
        yield return null;
        CameraManager.Instance().EncounterZoomIn();
        yield return null;
        SoundManager.Instance().Play(GameConstants.Sound.ENCOUNTER);
        yield return new WaitForSeconds(0.5f);
        uiController.ShowBlendEffect();
        yield return null;
        if (CurEnemy.IsBoss())
        {
            SoundManager.Instance().Play(GameConstants.Sound.COMBAT_BGM);
        }
        else
        {
            SoundManager.Instance().Play(GameConstants.Sound.BOSS_BGM);
        }
        yield return null;
        uiController.ResetEnemyImage();

        yield return new WaitForSeconds(0.8f);

        uiController.FadeEffect.gameObject.SetActive(true);
        uiController.FadeEffect.location = 0f;

        combatObject.SetActive(true);
        uiController.Show();

        yield return null;
        uiController.HideBlendEffect();

        yield return new WaitForSeconds(1f);
        CurEnemy.EventEncount();

        yield break;
    }

    public void EndCombat()
    {
        IsInCombat = false;
        SoundManager.Instance().SoundReset();
        NoteSystem.Instance().StopAllPatterns();
        //StartCoroutine(ProcessEndCombat());
        //CurEnemy.EventIdle();

        Player.Stats.ObservableCurHealth.Changed -= UpdatePlayerUI;
        Player.Stats.ObservableBalanceGauge.Changed -= UpdatePlayerUI;

        CurEnemy.Stats.ObservableCurHealth.Changed -= UpdateEnemyUI;
        CurEnemy.Stats.ObservableBalanceGauge.Changed -= UpdateEnemyUI;
        CurEnemy.PhaseChangeEvent -= PhaseChangeUI;
        
        StartCoroutine(BattleOut()); //여기가 추가된 코루틴
    }

    private IEnumerator BattleOut()
    {
        uiController.ShowAndHideBlendEffect(0.8f);
        yield return new WaitForSeconds(0.5f);

        combatObject.SetActive(false);
        uiController.Hide();
        if (PlayerManager.Instance().LocalPlayer.IsAlive)
        {
            uiController.ShowResult(CurEnemy.Stats.Gold);
        }
        else
        {
            PlayerManager.Instance().SwitchPlayerInputState(PlayerInputState.Move_State);
        }

        yield break;
    }

    public void BackToAdventrue()
    {
        StartCoroutine(ProcessBackToAdventrue());
    }

    private IEnumerator ProcessBackToAdventrue()
    {
        SoundManager.Instance().StopBGM();
        yield return null;
        if (SceneManager.GetActiveScene().name == GameConstants.Scene.GRAVEYARD_SCNEN)
        {
            SoundManager.Instance().Play(GameConstants.Sound.GRAVE_YARD_BGM);
        }
        else if (SceneManager.GetActiveScene().name == GameConstants.Scene.DUNGEON_0_SCENE)
        {
            SoundManager.Instance().Play(GameConstants.Sound.DUNGEON_BGM);
        }
        else if (SceneManager.GetActiveScene().name == GameConstants.Scene.DUNGEON_1_SCENE)
        {
            SoundManager.Instance().Play(GameConstants.Sound.DUNGEON_BGM);
        }

        uiController.Hide();

        Player.IsLocked = false;
        CurEnemy = null;
    }
}

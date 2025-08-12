using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlayerInputState
{
    Move_State,
    Battle_State,
    Menu_State,
}

public class PlayerManager : Singleton<PlayerManager>
{
    private Player localPlayer;

    public Player LocalPlayer
    {
        get => localPlayer;
        set
        {
            localPlayer = value;
            localPlayer?.Init();
        }
    }

    public PlayerContext LocalContext { get; private set; }

    public PlayerInputState CurrentState { get; private set; }
    private PlayerInputState previousState;

    private const string playModeStr = "PlayMode";
    private const string battleModeStr = "BattleMode";
    private const string menuModeStr = "MenuMode";

    public bool InteractionKeyGuide { get; private set; } = true;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInteractionKeyGuide();
        }
    }

    public void ToggleInteractionKeyGuide()
    {
        if (LocalPlayer != null)
        {
            LocalPlayer.Interaction.KeyGuide.SetActive(false);
        }

        InteractionKeyGuide = !InteractionKeyGuide;
    }

    protected override void OnAwake()
    {
        Init();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public void Init()
    {
        LocalContext = SaveLoadManager.Load<PlayerContext>();
    }

    public void OnContextChanged()
    {
        LocalContext.Save();
    }

    public void SwitchPlayerInputState(PlayerInputState newState)
    {
        if (LocalPlayer == null || CurrentState == newState)
        {
            return;
        }

        previousState = CurrentState;
        CurrentState = newState;

        string actionMap = null;

        switch (newState)
        {
            case PlayerInputState.Move_State:
                actionMap = playModeStr;
                break;
            case PlayerInputState.Battle_State:
                actionMap = battleModeStr;
                break;
            case PlayerInputState.Menu_State:
                actionMap = menuModeStr;
                break;
        }

        if (!string.IsNullOrEmpty(actionMap))
        {
            LocalPlayer.InputSystem.SwitchCurrentActionMap(actionMap);
        }
    }
    
    public void BackToPreviousState()
    {
        SwitchPlayerInputState(previousState);
    }

    public PlayerInputState GetCurrentState()
    {
        return CurrentState;
    }

    public void FullHeal()
    {
        LocalPlayer.Revive();
        LocalContext.Stats.CurHealth = LocalContext.Stats.TotalHealth;
    }

    public void EnterVillage()
    {
        LocalContext.Stats.CurHealth = LocalContext.Stats.TotalHealth;
        LocalContext.Stats.CurBalanceGauge = LocalContext.Stats.TotalBalance;
        LocalContext.Stats.PotionCount = 10;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == GameConstants.Scene.VILLAGE_SCENE)
        {
            if (LocalContext?.Stats != null)
            {
                LocalContext.Stats.RefillPotion();
            }
        }
    }

    public void ResetData()
    {
        LocalContext = null;

        Init();
    }
}

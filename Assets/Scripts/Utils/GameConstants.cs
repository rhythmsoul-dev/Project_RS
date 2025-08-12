using UnityEngine;

public static class GameConstants
{
    public static class Sound
    {
        public const string VILLAGE_BGM = "VillageBGM";
        public const string COMBAT_BGM = "CombatBGM";
        public const string GRAVE_YARD_BGM = "GraveyardBGM";
        public const string DUNGEON_BGM = "DungeonBGM";
        public const string CHURCH_BGM = "ChurchBGM";
        public const string INTRO_BGM = "IntroBGM";
        public const string SHOP_BGM = "ShopBGM";
        public const string BOSS_BGM = "BossBGM";
        public const string LAST_BOSS = "LastBoss";
        public const string GUARD = "Guard";
        public const string PARRYING_1 = "Parrying_1";
        public const string PARRYING_2 = "Parrying_2";
        public const string GRAB_SHEET_MUSIC = "GrabSheetMusic";
        public const string BUY_ITEM = "BuyItem";
        public const string MOVE = "Move";
        public const string OPEN_CHEST = "OpenChest";
        public const string GET_NOTE = "GetNote";
        public const string ATTACK_SOUND_1 = "AttackSound1";
        public const string ATTACK_SOUND_2 = "AttackSound2";
        public const string ATTACK_SOUND_3 = "AttackSound3";
        public const string TAKE_DAMAGE_1 = "TakeDamage1";
        public const string TAKE_DAMAGE_2 = "TakeDamage2";
        public const string USE_PORTAL = "UsePortal";
        public const string ENEMY_BALANCE_DOWN = "EnemyBalanceDown";
        public const string ENEMY_EXECUTION = "EnemyExecution";
        public const string ENCOUNTER_SUCCESS_1 = "EnounterSuccess1";
        public const string ENCOUNTER_SUCCESS_2 = "EnounterSuccess2";
        public const string ENCOUNTER_SUCCESS_3 = "EnounterSuccess3";
        public const string ENCOUNTER_FAILED_1 = "EnounterFailed1";
        public const string ENCOUNTER_FAILED_2 = "EnounterFailed2";
        public const string ATTACK_LOCKED = "AttackLocked";
        public const string ENCOUNTER = "Encounter";
        public const string POTION_GRAB = "PotionGrab";
        public const string USE_POTION = "UsePotion";
        public const string MEMO_OPEN = "MemoOpen";
        public const string MEMO_CLOSE = "MemoClose";
        public const string TURN_PAGE = "TurnPage";
        
        public static readonly string[] ATTACK_SOUNDS =
        {
            ATTACK_SOUND_1,
            ATTACK_SOUND_2,
            ATTACK_SOUND_3
        };
        
        public static readonly string[] TAKE_DAMAGE_SOUNDS =
        {
            TAKE_DAMAGE_1,
            TAKE_DAMAGE_2,
        };
        
        public static readonly string[] PARRYING_SOUNDS =
        {
            PARRYING_1,
            PARRYING_2,
        };
        
        public static readonly string[] ENCOUNTER_SUCCESS_SOUNDS =
        {
            ENCOUNTER_SUCCESS_1,
            ENCOUNTER_SUCCESS_2,
            ENCOUNTER_SUCCESS_3,
        };
        
        public static readonly string[] ENCOUNTER_FAILED_SOUNDS =
        {
            ENCOUNTER_FAILED_1,
            ENCOUNTER_FAILED_2,
        };
    }

    public static class Scene
    {
        public const string VILLAGE_SCENE = "VillageScene";
        public const string CHURCH_SCENE = "ChurchScene";
        public const string SHOP_SCENE = "ShopScene";
        public const string CREDIT_SCENE = "Credit";
        public const string TUTORIAL_SCENE = "Tutorial";
        public const string DUNGEON_0_SCENE = "Dungeon_0";
        public const string DUNGEON_1_SCENE = "Dungeon_1";
        public const string GRAVEYARD_SCNEN = "Graveyard_0";
    }
}

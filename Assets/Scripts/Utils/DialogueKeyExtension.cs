using UnityEngine;

public enum DialogueKey
{
    MerchantGreet,
    MerchantFirstMeet,
    MerchantFirstMeet2,
    MerchantDefault,
    MerchantFirstBossClear,
    MerchantSecondBossClear,
    MerchantThirdBossClear,
    MerchantFinalBossClear,
    MerchantFirstDeath,
    MerchantPurchaseFail1,
    MerchantPurchaseFail2,
    MerchantPurchaseSuccess,
    MerchantMemo5,
    MerchantMemo10,
    NunGreet,
    NunFirstMeet,
    NunFirstMeet2,
    NunDefault,
    NunFirstBossClear,
    NunSecondBossClear,
    NunThirdBossClear,
    NunFinalBossClear,
    NunFirstDeath,
    NunMemo4,
    NunMemo8,
    NunAfterMerchant,
    BaseSheetMusic,
    NunQuestMessage
}

public static class DialogueKeyExtension
{
    public static string ToId(this DialogueKey key)
    {
        return key.ToString();
    }
}
using System;
using UnityEngine;
using UnityEditor;

public class PlayerEditor : EditorWindow
{
    private bool isSuperMode = false;
    private bool previousSuperMode = false;

    private float originalAttack;
    private float originalBalanceAttack;

    [MenuItem("Tools/Player Editor")]
    public static void ShowWindow()
    {
        GetWindow<PlayerEditor>("Player Editor");
    }

    private void OnEnable()
    {
        if (PlayerManager.Instance().LocalContext == null)
        {
            return;
        }

        var stats = PlayerManager.Instance().LocalContext.Stats;
        originalAttack = stats.TotalAttack;
        originalBalanceAttack = stats.BalanceAttackPower;
    }

    private void OnDisable()
    {
        UnsubscribeSuperMode();
        SetAttackDown();
    }

    private void OnGUI()
    {
        GUILayout.Label("Player Editor", EditorStyles.boldLabel);

        isSuperMode = EditorGUILayout.Toggle("Super Mode", isSuperMode);

        if (isSuperMode != previousSuperMode)
        {
            if (isSuperMode)
            {
                SubscribeSuperMode();
                SetAttackUp();
                SetSuperMode(0);
            }
            else
            {
                UnsubscribeSuperMode();
                SetAttackDown();
            }

            previousSuperMode = isSuperMode;
        }
    }

    private void SubscribeSuperMode()
    {
        if (PlayerManager.Instance().LocalContext == null)
        {
            return;
        }

        var stats = PlayerManager.Instance().LocalContext.Stats;
        stats.ObservableCurHealth.Changed -= SetSuperMode;
        stats.ObservableCurHealth.Changed += SetSuperMode;
        stats.ObservableBalanceGauge.Changed -= SetSuperMode;
        stats.ObservableBalanceGauge.Changed += SetSuperMode;
    }

    private void UnsubscribeSuperMode()
    {
        if (PlayerManager.Instance().LocalContext == null)
        {
            return;
        }

        var stats = PlayerManager.Instance().LocalContext.Stats;
        stats.ObservableCurHealth.Changed -= SetSuperMode;
        stats.ObservableBalanceGauge.Changed -= SetSuperMode;
    }

    private void SetSuperMode(int value)
    {
        if (PlayerManager.Instance().LocalContext == null)
        {
            return;
        }

        var stats = PlayerManager.Instance().LocalContext.Stats;
        stats.CurHealth = stats.TotalHealth;
        stats.CurBalanceGauge = stats.TotalBalance;
    }

    private void SetAttackUp()
    {
        if (PlayerManager.Instance().LocalContext == null)
        {
            return;
        }

        var stats = PlayerManager.Instance().LocalContext.Stats;
        stats.TotalAttack = 100f;
        stats.BalanceAttackPower = 100f;
    }

    private void SetAttackDown()
    {
        if (PlayerManager.Instance().LocalContext == null)
        {
            return;
        }

        var stats = PlayerManager.Instance().LocalContext.Stats;
        stats.TotalAttack = 10f;
        stats.BalanceAttackPower = 10f;
    }

    private void OnInspectorUpdate()
    {
        Repaint();
    }
}

using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class StageManager : Singleton<StageManager>
{
    [SerializeField] private List<Enemy> stageEnemies; 
    private int currentEnemyIndex = 0;
    private int currentStage = 1;

    private void Start()
    {
        StartStage();
    }

    public void StartStage()
    {
        if (stageEnemies == null || stageEnemies.Count == 0)
        {
            stageEnemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None)
                .OrderBy(e => e.transform.position.x) 
                .ToList();
        }
        
        currentEnemyIndex = 0;
    }

    public void StartCombat()
    {
        if (currentEnemyIndex >= stageEnemies.Count)
        {
            FinishStage();
            return;
        }

        Enemy currentEnemy = stageEnemies[currentEnemyIndex];
        CombatSystem.Instance().StartCombat(currentEnemy);
    }

    public void EndCombat()
    {
        currentEnemyIndex++;
    }
    
    private void FinishStage()
    {
        currentStage++;
        currentEnemyIndex = 0;
        stageEnemies.Clear();
        Debug.Log("스테이지 클리어");
    }
}

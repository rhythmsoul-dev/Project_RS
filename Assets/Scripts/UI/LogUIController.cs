using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LogUIController : UIControllerBase
{
    [Serializable]
    private struct LootLogIcon
    {
        public LootType Type;
        public Sprite Icon;
    }
    
    [Header("로그 프리팹 설정")]
    [SerializeField] private GameObject logPrefab;
    [SerializeField] private int preInstantiateCount = 5;

    [Header("로그 위치 조정 속도")]
    [SerializeField] private float logMoveSpeed = 5f;

    [Header("로그 표시 시간")]
    [SerializeField] private float displayTime = 2f;
    [SerializeField] private float fadeDuration = 1f;
    
    [Header("로그 아이콘")]
    [SerializeField] private List<LootLogIcon> logIcons = new List<LootLogIcon>();
    private Dictionary<LootType, Sprite> logIconDic = new Dictionary<LootType, Sprite>();
    
    private List<LogUI> activeLogs = new List<LogUI>();
    private Queue<LogUI> logPool = new Queue<LogUI>();
    
    protected override void SetUp()
    {
        InitializeLogPool();
        
        foreach (var logIcon in logIcons)
        {
            logIconDic.Add(logIcon.Type, logIcon.Icon);
        }
        
        UIManager.Instance().RegisterController(this);
    }

    public override void Show() { }

    public override void Hide() { }
    
    private void InitializeLogPool()
    {
        for (int i = 0; i < preInstantiateCount; i++)
        {
            LogUI log = Instantiate(logPrefab, transform).GetComponent<LogUI>();
            log.gameObject.SetActive(false);
            logPool.Enqueue(log);
        }
    }
    
    public void ShowLog(LootData data)
    {
        LogUI log = GetLogInstance();

        Sprite icon;
        if (logIconDic.ContainsKey(data.Type))
        {
            icon = logIconDic[data.Type];
        }
        else
        {
            Debug.LogWarning("해당 아이템 타입 아이콘 없음: " + data.Type);
            icon = null;
        }
        
        log.Setup(data, icon, fadeDuration);
        log.transform.SetAsLastSibling();
        
        activeLogs.Add(log);
        
        MoveLog(log);
        
        StartCoroutine(HandleLogLifecycle(log));
    }
    
    private LogUI GetLogInstance()
    {
        if (logPool.Count > 0)
        {
            var log = logPool.Dequeue();
            log.gameObject.SetActive(true);
            return log;
        }
        
        return Instantiate(logPrefab, transform).GetComponent<LogUI>();
    }
    
    private IEnumerator HandleLogLifecycle(LogUI log)
    {
        yield return new WaitUntil(() => log.Activated);
        yield return new WaitForSeconds(displayTime);
        yield return log.FadeInOut(fadeDuration, false);

        activeLogs.Remove(log);
        logPool.Enqueue(log);
    }

    private void MoveLog(LogUI log)
    {
        foreach (var activeLog in activeLogs.Where(activeLog => activeLog != log))
        {
            activeLog.MoveUp(logMoveSpeed);
        }
    }
}

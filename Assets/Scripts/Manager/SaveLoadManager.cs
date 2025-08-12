using System;
using System.Collections.Generic;
using System.IO;
using JetBrains.Annotations;
using Newtonsoft.Json;
using UnityEngine;

public static class SaveLoadManager
{
    // 저장할 데이터 파일들이 들어갈 폴더 경로
    private static string dataDirectory;

    /// <summary>
    /// SaveLoadManager 초기화
    /// persistentDataPath 안에 "Data" 폴더를 생성
    /// </summary>
    public static void Init()
    {
        dataDirectory = Path.Combine(Application.persistentDataPath, "Data");
        Directory.CreateDirectory(dataDirectory); // 폴더 없으면 생성
    }

    /// <summary>
    /// 데이터를 JSON 형식으로 직렬화하여 파일로 저장
    /// </summary>
    public static void Save<T>(T data) where T : Context
    {
        try
        {
            string path = Path.Combine(dataDirectory, data.FileName); // 파일 경로
            string json = JsonConvert.SerializeObject(data, Formatting.Indented); // 들여쓰기
            File.WriteAllText(path, json); // 파일에 기록
        }
        catch (Exception e)
        {
            // 저장 중 오류 발생 시 콘솔에 예외 출력
            Debug.LogException(e);
        }
    }

    /// <summary>
    /// 저장된 JSON 데이터를 불러와 객체로 역직렬화
    /// 저장 파일이 없으면 새 객체를 생성 후 초기화 & 저장
    /// </summary>
    [CanBeNull]
    public static T Load<T>() where T : Context, new()
    {
        T context = new T();

        string path = Path.Combine(dataDirectory, context.FileName);

        // 저장된 파일이 없으면 새로 생성하고 저장
        if (!File.Exists(path))
        {
            context.Init();  // Context 내부 초기화
            context.Save();  // 기본 상태 저장
            return context;
        }

        try
        {
            // 파일 읽어서 JSON → 객체 변환
            context = JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
            context?.Init();
        }
        catch (Exception e)
        {
            Debug.LogException(e); // 로드 실패 시 예외 출력
        }

        return context;
    }

    /// <summary>
    /// 저장된 모든 JSON 파일 삭제
    /// </summary>
    public static void DeleteAllSaveFiles()
    {
        string saveDir = Path.Combine(Application.persistentDataPath, "Data");
        foreach (var file in Directory.GetFiles(saveDir, "*.json"))
        {
            File.Delete(file);
        }
    }
}

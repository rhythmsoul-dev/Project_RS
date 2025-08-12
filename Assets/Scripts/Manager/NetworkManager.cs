using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkManager : Singleton<NetworkManager>
{
    public bool IsInitialized { get; private set; }

    public async Task Init()
    {
        await Task.WhenAll(InitEntityInfos(), InitNoteInfos(), InitNotePatternInfos());

        IsInitialized = true;
        Debug.Log("Initialized");
    }

    /// <summary>
    /// 서버에서 엔티티 정보를 불러옵니다.
    /// </summary>
    /// <returns></returns>
    public async Task InitEntityInfos()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://db.kyllox.pe.kr/rhythm_soul/data/entity/files.txt");
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            return;
        }

        string pathData = request.downloadHandler.text;
        string[] paths = pathData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < paths.Length; i++)
        {
            string path = $"http://db.kyllox.pe.kr/rhythm_soul/data/entity/{paths[i]}";

            UnityWebRequest request2 = UnityWebRequest.Get(path);
            await request2.SendWebRequest();
            if (request2.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request2.error);
                return;
            }

            string jsonData = request2.downloadHandler.text;
            EntityInfo info = JsonConvert.DeserializeObject<EntityInfo>(jsonData);
            EntityManager.Instance().SetEntityInfo(info);
        }
    }

    /// <summary>
    /// 서버에서 노트 정보를 불러옵니다.
    /// </summary>
    /// <returns></returns>
    public async Task InitNoteInfos()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://db.kyllox.pe.kr/rhythm_soul/data/note/files.txt");
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            return;
        }

        string pathData = request.downloadHandler.text;
        string[] paths = pathData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < paths.Length; i++)
        {
            string path = $"http://db.kyllox.pe.kr/rhythm_soul/data/note/{paths[i]}";

            UnityWebRequest request2 = UnityWebRequest.Get(path);
            await request2.SendWebRequest();
            if (request2.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request2.error);
                return;
            }

            string jsonData = request2.downloadHandler.text;
            NoteInfo info = JsonConvert.DeserializeObject<NoteInfo>(jsonData);
            EntityManager.Instance().SetNoteInfo(info);
        }
    }

    /// <summary>
    /// 서버에서 노트 패턴 정보를 불러옵니다.
    /// </summary>
    /// <returns></returns>
    public async Task InitNotePatternInfos()
    {
        UnityWebRequest request = UnityWebRequest.Get("http://db.kyllox.pe.kr/rhythm_soul/data/pattern/files.txt");
        await request.SendWebRequest();
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error);
            return;
        }

        string pathData = request.downloadHandler.text;
        string[] paths = pathData.Split(new[] { "\r\n", "\n" }, StringSplitOptions.RemoveEmptyEntries);
        for (int i = 0; i < paths.Length; i++)
        {
            string path = $"http://db.kyllox.pe.kr/rhythm_soul/data/pattern/{paths[i]}";

            UnityWebRequest request2 = UnityWebRequest.Get(path);
            await request2.SendWebRequest();
            if (request2.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request2.error);
                return;
            }

            string jsonData = request2.downloadHandler.text;
            NotePatternInfo info = JsonConvert.DeserializeObject<NotePatternInfo>(jsonData);
            EntityManager.Instance().SetNotePatternInfo(info);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

[Serializable]
public class Memo
{
    public Memo()
    {
    }

    public Memo(MemoData data)
    {
        MemoText = data.Text;
        MemoImagePath = data.ImagePath;
        memoId = data.MemoId;
        memoImage = null;
    }
    
    [JsonProperty("memo_id")]
    public string memoId;
    
    [JsonProperty("memo_text")]
    public string MemoText;

    [JsonProperty("memo_image_path")]
    public string MemoImagePath;

    private Sprite memoImage;
    
    public void LoadImage(Action<Sprite> onLoaded = null)
    {
        if (memoImage != null)
        {
            onLoaded?.Invoke(memoImage);
            return;
        }

        Addressables.LoadAssetAsync<Sprite>(MemoImagePath).Completed += (handle) =>
        {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                memoImage = handle.Result;
                onLoaded?.Invoke(memoImage);
            }
            else
            {
                Debug.LogErrorFormat("[Addressable] Cannot Found Image Path: {0}", MemoImagePath);
            }
        };
    }
}

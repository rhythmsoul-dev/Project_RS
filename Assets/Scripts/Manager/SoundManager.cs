using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine.Audio;

public enum SoundType
{
    BGM,
    SFX,
    SFXCon
}

[System.Serializable]
public class Sound
{
    public string Name;
    public AudioClip Clip;
    public SoundType Type;
}

// 스크립터블로 사운드데이터관리하는거는 논의하고 추가할게요 
public class SoundManager : Singleton<SoundManager>
{
    [Header("오디오 믹서")]
    [SerializeField] private AudioMixer audioMixer;

    [Header("오디오 소스")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource sfxConditionalSource;
    public AudioSource SFXSource => sfxSource;

    [Header("사운드 리스트")]
    [SerializeField] private List<Sound> sounds;

    public void SetBGMVolume(float v) => SetVolume(SoundType.BGM, v);
    public void SetSFXVolume(float v) => SetVolume(SoundType.SFX, v);
    
    private Dictionary<string, Sound> soundDict;
    protected override void OnAwake()
    {
        InitializeDictionary();
    }

    private void InitializeDictionary()
    {
        soundDict = new Dictionary<string, Sound>();
        foreach (var sound in sounds)
        {
            if (!soundDict.ContainsKey(sound.Name))
            {
                soundDict.Add(sound.Name, sound);
            }
        }
    }

    public void Play(string soundName)
    {
        if (!soundDict.TryGetValue(soundName, out var sound))
        {
            return;
        }

        switch (sound.Type)
        {
            case SoundType.BGM:
                PlayBGM(sound.Clip);
                break;
            case SoundType.SFX:
                PlaySFX(sound.Clip);
                break; 
            case SoundType.SFXCon:
                PlaySFXCon(sound.Clip);
                break;
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (bgmSource.clip == clip && bgmSource.isPlaying)
        {
            return;
        }

        bgmSource.clip = clip;
        bgmSource.loop = true;
        bgmSource.Play();
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
    public void PlaySFXCon(AudioClip clip)
    {
        sfxConditionalSource.PlayOneShot(clip);
    }

    public void SetVolume(SoundType type, float volume)
    {
        string parameterName = type switch
        {
            SoundType.BGM => "BGM",
            SoundType.SFX => "SFX",
            _ => ""
        };

        float dB = Mathf.Log10(Mathf.Clamp(volume, 0.0001f, 1f)) * 20f;
        audioMixer.SetFloat(parameterName, dB);

        PlayerPrefs.SetFloat(parameterName, dB);
        PlayerPrefs.Save();
    }

    public float GetVolume(SoundType type)
    {
        string parameterName = type switch
        {
            SoundType.BGM => "BGM",
            SoundType.SFX => "SFX",
            _ => ""
        };

        audioMixer.SetFloat(parameterName, PlayerPrefs.GetFloat(parameterName)); // PlayerPrefs에 저장된 음량 가져오기

        if (audioMixer.GetFloat(parameterName, out float dB))
        {
            return Mathf.Pow(10f, dB / 20f);
        }

        return 1f;
    }

    public bool IsPlayingSFX()
    {
        return sfxSource.isPlaying;
    }
   
    public void SetUnderwater(bool isUnderwater)
    {
        float cutoff = isUnderwater ? 1000f : 22000f;
       
        StartCoroutine(SmoothLowpassTransition("LowpassCutoff", cutoff));
        StartCoroutine(SmoothLowpassTransition("LowpassCutoffSFX", cutoff));
    }
    
    private IEnumerator SmoothLowpassTransition(string lowpassParam, float target)
    {
        audioMixer.GetFloat(lowpassParam, out float current);

        float time = 0f;

        while (time < 1f)
        {
            time += Time.deltaTime;
            float t = time / 1f;

            float value = Mathf.Lerp(current, target, t);
            audioMixer.SetFloat(lowpassParam, value);

            yield return null;
        }

        audioMixer.SetFloat(lowpassParam, target);
    }

    public void SoundReset()
    {
        audioMixer.SetFloat("LowpassCutoff", 22000f);
        audioMixer.SetFloat("LowpassCutoffSFX", 22000f);
    }

    public void SwitchBGM(string bgmName, float duration)
    {
        if (!soundDict.TryGetValue(bgmName, out var sound))
        {
            return;
        }

        if (sound.Type != SoundType.BGM)
        {
            Debug.LogError($"BGM \"{bgmName}\" is not BGM");
            return;
        }
        
        StartCoroutine(SwitchBGMCoroutine(bgmName, duration));
    }

    private IEnumerator SwitchBGMCoroutine(string bgmName, float duration)
    {
        float originalVolume = GetVolume(SoundType.BGM);
        
        yield return DOTween.To(
            () => GetVolume(SoundType.BGM),
            x => SetVolume(SoundType.BGM, x),
            0f,
            duration
        ).SetEase(Ease.Linear)
         .SetUpdate(true)
         .WaitForCompletion();

        yield return null;
        StopBGM();
        yield return null;
        Play(bgmName);
        
        yield return DOTween.To(
            () => GetVolume(SoundType.BGM),
            x => SetVolume(SoundType.BGM, x),
            originalVolume,
            duration / 2f
        ).SetEase(Ease.Linear)
         .SetUpdate(true)
         .WaitForCompletion();
    }
}

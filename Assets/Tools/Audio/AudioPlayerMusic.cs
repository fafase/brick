using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using UniRx;
using UnityEngine;

public class AudioPlayerMusic : MonoBehaviour
{
    [SerializeField] private float m_maxVolume = 0.8f;
    [SerializeField] private float m_volumeIncreaseSpeed = 1f;

    [Header("Main Audio")]
    [SerializeField] private List<AudioData> m_audioData;

    private AudioSource[] m_musicSources;
    private int m_currentMainAudioSource = 0;

    private void Start()
    {
        DontDestroyOnLoad(this);
        SetMusicSources();

        StartAmbianceMusic()
            .Subscribe()
            .AddTo(this);

        BindEndLevel();
        BindSceneLoading();
    }

    private void SetMusicSources()
    {
        m_musicSources = new[]
        {
            gameObject.AddComponent<AudioSource>(),
            gameObject.AddComponent<AudioSource>()
        };
    }


    private IObservable<float> StartAmbianceMusic()
    {
        return Observable.Create<float>(observer =>
        {
            AudioSource audioSource = m_musicSources[m_currentMainAudioSource];
            audioSource.clip = m_audioData.Find(audio => audio.name.Equals("Idle")).audioClip;
            audioSource.Play();
            audioSource.volume = 0f;
            audioSource.loop = true;
            float elapsedTime = 0f;

            var sub = Observable.EveryUpdate()
                .Select(_ =>
                {
                    elapsedTime += Time.deltaTime;
                    float fadeProgress = Mathf.Clamp01(elapsedTime / m_volumeIncreaseSpeed);
                    audioSource.volume = Mathf.Lerp(0f, m_maxVolume, fadeProgress);
                    return fadeProgress;
                })
                .TakeWhile(progress => progress < 1f)
                .Subscribe(progress => observer.OnNext(progress),
                    error => observer.OnError(error),
                    () =>
                    {
                        audioSource.volume = 1f;
                        observer.OnNext(1f);
                        observer.OnCompleted();
                    });
            return sub;
        });
    }

    private void BindEndLevel()
    {
        ObservableSignal.AsObservable<EndLevelSignal>()
           .Subscribe(data =>
           {
               string audioName = (data.IsWinning) ? "Win" : "Lose";
               FadeMusic(GetMusicClip(audioName)).Subscribe();
           }).AddTo(this);
    }

    private IObservable<float> FadeMusic(string clip)
    {
        var audioClip = GetMusicClip(clip);
        if (audioClip == null)
        {
            Debug.LogError("Could not find clip " + clip);
            return Observable.Empty<float>();
        }
        return FadeMusic(audioClip);
    }

    private IObservable<float> FadeMusic(AudioClip nextClip)
    {
        return Observable.Create<float>(observer =>
        {
            AudioSource current = m_musicSources[m_currentMainAudioSource];
            AudioSource next = m_musicSources[(m_currentMainAudioSource == 0) ? 1 : 0];
            next.volume = 0f;
            next.loop = true;
            next.clip = nextClip;
            next.Play();

            float elapsedTime = 0f;
            var update = Observable.EveryUpdate()
                .Select(_ =>
                {
                    elapsedTime += Time.deltaTime;
                    float fadeProgress = Mathf.Clamp01(elapsedTime / m_volumeIncreaseSpeed);
                    current.volume = Mathf.Lerp(m_maxVolume, 0f, fadeProgress);
                    next.volume = Mathf.Lerp(0f, m_maxVolume, fadeProgress);
                    return fadeProgress;
                }).TakeWhile(fadeProgress => fadeProgress < 1f)
                .Subscribe(progress => observer.OnNext(progress),
                error => observer.OnError(error),
                () =>
                {
                    current.volume = 0f;
                    current.Stop();
                    current.volume = m_maxVolume;
                    m_currentMainAudioSource = m_currentMainAudioSource == 0 ? 1 : 0;
                    observer.OnNext(1f);
                    observer.OnCompleted();
                });
            return update;
        });
    }
    private AudioClip GetMusicClip(string name)
    {
        return m_audioData.FirstOrDefault(audio => audio.name.Equals(name)).audioClip;
    }

    private void BindSceneLoading()
    {
        ObservableSignal.AsObservable<SceneChangeSignal>()
            .Where(data => data.NextScene.Equals("Meta") ||
            (data.NextScene.Equals("Core") && data.PreviousScene.Equals("Core")))
            .Do(_ =>
            {
                FadeMusic("Idle").Subscribe();
            })
            .Subscribe()
            .AddTo(this);
    }
}

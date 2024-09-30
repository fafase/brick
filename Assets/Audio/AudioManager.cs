using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tools;
using UniRx;
using UnityEngine;
using Zenject;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioClip m_music;
    [SerializeField] private AudioSource m_mainAudioSource;
    [SerializeField] private float m_maxVolume = 0.8f;
    [SerializeField] private float m_volumeIncreaseSpeed = 1f;
    [SerializeField] private GameObject m_audioSourceContainer;

    [Header("SFX")]
    [SerializeField] private List<AudioData> m_data;

    private IDictionary<string, AudioClip> m_dict;
    private List<AudioSource> m_sources;

    private void Start()
    {
        DontDestroyOnLoad(this);
        StartAmbianceMusic();
        PopulateAudioDictionary();
        PrepareAudioSources();
        BindAudioSignal();
        BindPopupSignal();
    }

    private void StartAmbianceMusic() 
    {
        m_mainAudioSource.clip = m_music;
        m_mainAudioSource.Play();
        m_mainAudioSource.volume = 0f;
        m_mainAudioSource.loop = true;
        Observable.EveryUpdate()
            .TakeWhile(_ => m_mainAudioSource.volume < m_maxVolume)
            .Subscribe(time =>
            {
                m_mainAudioSource.volume += Time.deltaTime * m_volumeIncreaseSpeed; // Adjust the speed with a multiplier
                m_mainAudioSource.volume = Mathf.Clamp(m_mainAudioSource.volume, 0f, 1f); // Clamp the volume to ensure it doesn't exceed 1
            },
            () => m_mainAudioSource.volume = 1f)
            .AddTo(this);
    }

    private void PopulateAudioDictionary() 
    {
        m_dict = new Dictionary<string, AudioClip>();
        foreach(var audio in m_data) 
        {
            m_dict.Add(audio.name, audio.audioClip);
        }
    }

    private void PrepareAudioSources() => m_sources = m_audioSourceContainer.GetComponents<AudioSource>().ToList(); 
    

    private void BindAudioSignal() 
    { 
        ObservableSignal
            .AsObservable<AudioSignal>()
            .Subscribe(data => PlayAudio(data.ClipName, data.Volume))
            .AddTo(this); 
    }  
    
    private void PlayAudio(string audioName, float volume = 1f) 
    {
        if (string.IsNullOrEmpty(audioName)) 
        {
            Debug.LogWarning("No audio clip name given");
            return;
        }

        if (m_dict.TryGetValue(audioName, out AudioClip clip))
        {
            AudioSource source = GetAudioSource();
            source.clip = clip;
            source.volume = volume;
            source.Play();
        }
        else
        {
            Debug.LogWarning("Could not find the clip");
        }
    }

    private AudioSource GetAudioSource() 
    {
        AudioSource source = m_sources.FirstOrDefault(source => source.isPlaying == false);
        if(source is null) 
        {
            source =  m_audioSourceContainer.AddComponent<AudioSource>();
            m_sources.Add(source);
        }
        return source;
    }

    private void BindPopupSignal() 
    {
        ObservableSignal
            .AsObservable<PopupSignal>()
            .Subscribe(_ => PlayAudio("Swoosh"))
            .AddTo(this);
    }

    [Serializable]
    public class AudioData 
    {
        public string name;
        public AudioClip audioClip;
    }
}

public class AudioSignal : SignalData
{
    public readonly string ClipName;
    public readonly float Volume;

    public AudioSignal(string clipName, float volume = 1f)
    {
        ClipName = clipName;
        Volume = volume;
    }
}

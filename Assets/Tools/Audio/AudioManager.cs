using System;
using System.Collections.Generic;
using System.Linq;
using Tools;
using UniRx;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private GameObject m_audioSourceContainer;
    [Header("SFX")]
    [SerializeField] private List<AudioData> m_data;

    private IDictionary<string, AudioClip> m_dict;
    private List<AudioSource> m_sources;

    private void Start()
    {
        DontDestroyOnLoad(this);
        m_sources = new List<AudioSource>();
        PopulateAudioDictionary();
        BindAudioSignal();
    }

    private void PopulateAudioDictionary() 
    {
        m_dict = new Dictionary<string, AudioClip>();
        foreach(var audio in m_data) 
        {
            m_dict.Add(audio.name, audio.audioClip);
        }
    }

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
}
[Serializable]
public class AudioData
{
    public string name;
    public AudioClip audioClip;
}

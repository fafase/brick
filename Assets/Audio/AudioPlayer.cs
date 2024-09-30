using UniRx;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioClip m_clip;
    [SerializeField] private AudioSource m_source;
    [SerializeField] private float m_volumeIncreaseSpeed = 1f;
    private void Start()
    {
        DontDestroyOnLoad(this);
        m_source.clip = m_clip;
        m_source.Play();
        m_source.volume = 0f;
        m_source.loop = true;
        Observable.EveryUpdate()
            .TakeWhile(_ => m_source.volume < 1f)
            .Subscribe(time => 
            {               
                m_source.volume += Time.deltaTime * m_volumeIncreaseSpeed; // Adjust the speed with a multiplier
                m_source.volume = Mathf.Clamp(m_source.volume, 0f, 1f); // Clamp the volume to ensure it doesn't exceed 1
            }, 
            () =>  m_source.volume = 1f)
            .AddTo(this);
    }
}

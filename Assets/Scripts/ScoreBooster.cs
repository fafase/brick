using System;
using TMPro;
using Tools;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ScoreBooster : MonoBehaviour, IScoreBooster
{
    [SerializeField] private Image m_image;
    [SerializeField] private TextMeshProUGUI m_multiplierTxt;

    private ReactiveProperty<float> m_countdown = new ReactiveProperty<float>(0f);
    private ReactiveProperty<int> m_multiplier = new ReactiveProperty<int>(1);

    private IDisposable m_updateDisposable;
    private float decrementRate = 0.5f;
    private float m_max = 4f;
    public int Multiplier => m_multiplier.Value;    

    private void Start()
    {
        m_image.fillAmount = 0f;

        m_updateDisposable = Observable.EveryUpdate()
                                .Subscribe(_ =>
                                {
                                    float temp = m_countdown.Value - decrementRate * Time.deltaTime;
                                    m_countdown.Value = Mathf.Clamp(temp, 0f, m_max);
                                    Debug.Log(m_countdown.Value);
                                })
                                .AddTo(this);
        m_countdown
            .Subscribe(value =>
            {
                float percentage = value / m_max;
                m_image.fillAmount = percentage;
                int percent = (int)value;
                int extra = Mathf.Clamp(percent, 0, (int)m_max - 1);
                m_multiplier.Value = 1 + extra;
            })
            .AddTo(this);

        m_multiplier
            .Subscribe(value => m_multiplierTxt.text = "x" + value)
            .AddTo(this);
    }
    private void OnDestroy()
    {
        m_updateDisposable.Dispose();
    }

    public int ProcessMultiplier() 
    {
        m_countdown.Value += 1f;
        return m_multiplier.Value;  
    }
}
public interface IScoreBooster 
{
    int ProcessMultiplier();
}

using System;
using TMPro;
using Tools;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class ScoreBooster : MonoBehaviour
{
    [SerializeField] private Image m_image;
    [SerializeField] private TextMeshProUGUI m_multiplierTxt;  

    [Inject] private IScoreBooster m_presenter;

    private void Start()
    {
        m_image.fillAmount = 0f;

        m_presenter.Countdown
            .Subscribe(value => m_image.fillAmount = m_presenter.ProcessCountdown(value))
            .AddTo(this);

        m_presenter.Multiplier
            .Subscribe(value => m_multiplierTxt.text = "x" + value)
            .AddTo(this);
    }
}
public interface IScoreBooster 
{
    int ProcessMultiplier();
    float ProcessCountdown(float value);
    IReactiveProperty<float> Countdown { get; }
    IReactiveProperty<int> Multiplier { get; }
}
public class ScoreBoosterPresenter : Presenter, IScoreBooster
{
    private IDisposable m_updateDisposable;

    private float decrementRate = 0.5f;
    private float m_max = 4f;

    public IReactiveProperty<float> Countdown { get; private set; }
    public IReactiveProperty<int> Multiplier { get; private set; }

    public ScoreBoosterPresenter()
    {
        m_compositeDisposable = new CompositeDisposable();
        Countdown = new ReactiveProperty<float>(0f);
        Multiplier = new ReactiveProperty<int>(1);

        m_updateDisposable = Observable.EveryUpdate()
                        .Subscribe(_ =>
                        {
                            float temp = Countdown.Value - decrementRate * Time.deltaTime;
                            Countdown.Value = Mathf.Clamp(temp, 0f, m_max);
                        })
                        .AddTo(m_compositeDisposable);
    }
    
    public override void Dispose()
    {
        if (m_isDisposed) { return; }
        base.Dispose();
        m_updateDisposable?.Dispose();
        (Countdown as ReactiveProperty<float>)?.Dispose();
        (Multiplier as ReactiveProperty<int>)?.Dispose();
    }

    public float ProcessCountdown(float value) 
    {
        Multiplier.Value = 1 + Mathf.Clamp((int)value, 0, (int)m_max - 1);
        return value / m_max; ;
    }
    public int ProcessMultiplier()
    {
        Countdown.Value += 1f;
        return Multiplier.Value;
    }
}

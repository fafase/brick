using System;
using TMPro;
using Tools;
using UniRx;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TimerView : MonoBehaviour
{
    private TextMeshProUGUI m_timerTxt;

    private int m_sessionDuration = 60;
    private int m_remainingTime;
    private IDisposable m_timer;
    public int RemainingTime => m_remainingTime;

    private void Start()
    {
        m_timerTxt = GetComponent<TextMeshProUGUI>();   
        BindTimer();

        ObservableSignal
            .AsObservable<EndLevelSignal>()
            .Subscribe(_ => m_timer.Dispose())
            .AddTo(this);

        ObservableSignal
            .AsObservable<LoadLevelSignal>()
            .DelayFrame(1)
            .Subscribe(config => SetTimer(config.LvlConfig.time))
            .AddTo(this);
    }

    private void BindTimer()
    {
        m_timerTxt.text = $"{m_sessionDuration}s";
        IDisposable signalDisposable = null;
        void StartTimer()
        {
            signalDisposable?.Dispose();
            m_timer = Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(elapsed => m_remainingTime = m_sessionDuration - (int)elapsed)
                .TakeWhile(remaining => remaining >= 0)
                .DelayFrame(1)
                .Subscribe(remainingTime => m_timerTxt.text = $"{remainingTime}s",
                () =>
                {
                    m_timer.Dispose();
                    m_remainingTime = 0;
                    ObservableSignal.Broadcast(new EndLevelSignal(false, EndLevelSignal.LossReason.TimeUp));
                })
                .AddTo(this);
        }

        signalDisposable = ObservableSignal
            .AsObservable<GameStateData>()
            .Where(data => data.NextState.Equals(GameState.Play))
            .Subscribe(data => StartTimer())
            .AddTo(this);
    }

    public void SetTimer(int s) 
    {
        m_sessionDuration = s;
    }
}

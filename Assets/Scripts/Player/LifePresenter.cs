using System;
using Tools;
using UniRx;
using UnityEngine;

public class LifePresenter : Presenter, ILife
{
    private int m_maxLives = 5;

    private int m_refillInterval = 5;
    private ReactiveProperty<int> m_currentLives;
    private IDisposable m_refillSubscription;
    public IObservable<int> Lives => m_currentLives.AsObservable();
    private Subject<int> m_countdownSubject = new Subject<int>();
    public IObservable<int> CountdownTracker => m_countdownSubject.AsObservable();

    public LifePresenter()
    {
        //m_maxLives = provider.MaxLives;
        m_currentLives = new ReactiveProperty<int>(m_maxLives);
        //m_refillInterval = provider.RefillLifeInSec;
        StartRefill();
    }

    private void StartRefill()
    {
        if (m_currentLives.Value >= m_maxLives) return;

        int initialCountdown = Mathf.RoundToInt(m_refillInterval);
        m_refillSubscription = 
            Observable.Interval(TimeSpan.FromSeconds(1))
            //.Select(secondsElapsed => initialCountdown - (int)(secondsElapsed % m_refillInterval))
            .Do(timeLeft =>
            {
                initialCountdown--;
                m_countdownSubject.OnNext(initialCountdown);

                if (initialCountdown <= 0)
                {
                    if (m_currentLives.Value < m_maxLives)
                    {
                        m_currentLives.Value++;
                        Debug.Log("Life refilled!");
                    }

                    // Reset the countdown for the next interval
                    initialCountdown = Mathf.RoundToInt(m_refillInterval);
                }

                // If the lives are full, stop the refill
                if (m_currentLives.Value >= m_maxLives)
                {
                    StopRefill();
                }
            })
            .Subscribe()
            .AddTo(m_compositeDisposable);
    }
    public void LoseLife()
    {
         if (m_currentLives.Value > 0)
         {
            m_currentLives.Value--;

            if (m_refillSubscription == null && m_currentLives.Value < m_maxLives)
            {
                StartRefill();
            }
        }
    }
    private void StopRefill()
    {
        m_refillSubscription?.Dispose();
        m_refillSubscription = null;
        m_countdownSubject.OnNext(-1);
    }
}
public interface ILifeProvider 
{
    int MaxLives {  get; }
    int RefillLifeInSec {  get; }
}
public interface ILife 
{
    void LoseLife();
    IObservable<int> Lives { get; }
    IObservable<int> CountdownTracker { get; }
}

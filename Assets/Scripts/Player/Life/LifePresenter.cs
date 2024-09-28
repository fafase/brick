using System;
using Tools;
using UniRx;
using UnityEngine;

public class LifePresenter : Presenter, ILife
{
    private int m_maxLives = 5;

    private int m_refillInterval = 300;
    private ReactiveProperty<int> m_currentLives;
    private IDisposable m_refillSubscription;
    public IObservable<int> LivesAsObservable => m_currentLives.AsObservable();
    private Subject<int> m_countdownSubject = new Subject<int>();
    public IObservable<int> CountdownTrackerAsObservable => m_countdownSubject.AsObservable();
    public int Lives => m_currentLives.Value;

    public LifePresenter()
    {
        m_currentLives = new ReactiveProperty<int>(m_maxLives);
        StartRefill();
    }

    private void StartRefill()
    {
        if (m_currentLives.Value >= m_maxLives) return;

        int initialCountdown = Mathf.RoundToInt(m_refillInterval);
        m_refillSubscription = Observable.Interval(TimeSpan.FromSeconds(1))
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
                                        initialCountdown = Mathf.RoundToInt(m_refillInterval);
                                    }
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
            Debug.Log("Using a life");
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
    IObservable<int> LivesAsObservable { get; }
    IObservable<int> CountdownTrackerAsObservable { get; }
    int Lives { get; }
}

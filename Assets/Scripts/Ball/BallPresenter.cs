using System;
using Tools;
using UniRx;
using UnityEngine;

public class BallPresenter : Presenter, IBallPresenter, IDisposable
{
    public float InitialForce { get; private set; }

    public int Power { get; private set; }

    public IReactiveProperty<bool> Active { get; private set; }
    public Subject<int> Score { get; private set; } = new Subject<int>();

    private float m_initialAngle;
    private Rigidbody2D m_rigidbody;
    public bool IsExtraBall { get; set; }
    private float m_minimumVelocity;
    private Vector2 m_velocity;

    public BallPresenter()
    {
        m_compositeDisposable = new CompositeDisposable();
        Active = new ReactiveProperty<bool>(false);

        Active
            .Skip(1)
            .Subscribe(state => 
            {
                if (!IsExtraBall)
                {
                    ObservableSignal.Broadcast(new BallActiveSignal(state));
                }
                m_rigidbody.gameObject.SetActive(state); 
            })
            .AddTo(m_compositeDisposable);

        Score
            .Subscribe(score =>
            {
                ObservableSignal.Broadcast(new BallScoreSignal(score));
            })
            .AddTo(m_compositeDisposable);

        ObservableSignal.AsObservable<GameStateData>()
            .Where(data => data.NextState.Equals(GameState.Pause) 
                || data.NextState.Equals(GameState.Play)
                || data.NextState.Equals(GameState.Loss))
            .Subscribe(data => 
                {
                    switch(data.NextState)
                    {
                        case GameState.Loss:
                        case GameState.Pause: 
                            ProcessPause(); 
                            break;
                        case GameState.Play : 
                            if(data.PreviousState.Equals(GameState.Pause))
                                ProcessPlay();
                            break;
                    }
                })
            .AddTo(m_compositeDisposable);

        ObservableSignal
            .AsObservable<ResetBallSignal>()
            .Subscribe(data => ResetBall(data.Swipe))
            .AddTo(m_compositeDisposable);

        Observable.EveryFixedUpdate()
            .Where(_ => Active.Value)
            .Subscribe(_ => 
            {
                var velocity = m_rigidbody.velocity;
                if (velocity.magnitude < m_minimumVelocity)
                {
                    float boostMultiplier = Mathf.Max(m_minimumVelocity, velocity.magnitude + 0.1f);
                    m_rigidbody.velocity = velocity.normalized * boostMultiplier;
                }
                if (Mathf.Abs(velocity.y) < 0.05f)
                {
                    velocity.y += (velocity.y > 0 ? 0.1f : -0.1f);
                    m_rigidbody.velocity = velocity.normalized * m_rigidbody.velocity.magnitude;
                }
            })
            .AddTo(m_compositeDisposable);

    }

    public void Init(float initialForce, int power, float initialAngle, Rigidbody2D rigidbody, bool isExtraBall)
    {
        InitialForce = initialForce;
        Power = power;
        m_initialAngle = initialAngle;
        m_rigidbody = rigidbody;
        IsExtraBall = isExtraBall;
    }

    public void AddInitialForce(Vector2 swipe)
    {     
        var force = (swipe == Vector2.zero) ? new Vector2(Mathf.Sin(m_initialAngle * Mathf.Deg2Rad) * InitialForce, Mathf.Cos(m_initialAngle * Mathf.Deg2Rad) * InitialForce)
            : swipe.normalized * InitialForce;
        
        m_rigidbody.AddForce(force);
    }

    public void CalculateBounceVelocityPaddle(Collision2D collision, float maxPaddleBounceAngle)
    {
        var localContact = collision.transform.InverseTransformPoint(collision.contacts[0].point);
        var paddleWidth = collision.collider.GetComponent<SpriteRenderer>().bounds.size.x;
        var normalizedLocalContactX = localContact.x / (paddleWidth / 2);
        var bounceAngle = normalizedLocalContactX * maxPaddleBounceAngle;

        var bounceForce = new Vector2
        {
            x = Mathf.Sin(bounceAngle) * InitialForce,
            y = Mathf.Cos(bounceAngle) * InitialForce
        };
        m_rigidbody.velocity = Vector2.zero;  
        m_rigidbody.AddForce(bounceForce);
    }

    private void ProcessPause() 
    {
        m_velocity = m_rigidbody.velocity;
        Active.Value = false;       
        m_rigidbody.velocity = Vector2.zero; 
    }

    private void ProcessPlay() 
    {
        Active.Value = true;
        if (m_velocity == Vector2.zero)
            AddInitialForce(Vector2.zero);
        else
        {
            m_rigidbody.AddForce(m_velocity.normalized * InitialForce);
            m_velocity = Vector2.zero;
        }
    }
    public void UpdateScore(int score) 
    {
        Score.OnNext(score);
    }
    public void ResetBall(Vector2 swipe)
    {
        Active.Value = true;
        AddInitialForce(swipe);
    }
}
public class BallActiveSignal : SignalData 
{
    public readonly bool IsActive;
    public BallActiveSignal(bool isActive)
    {
        IsActive = isActive;
    }
}
public class BallScoreSignal : SignalData 
{
    public readonly int Score;
    public BallScoreSignal(int score)
    {
        Score = score;
    }

}
public interface IBallPresenter
{
    IReactiveProperty<bool> Active { get; }
    int Power { get; }
    Subject<int> Score { get; }
    void AddInitialForce(Vector2 swipe);
    void CalculateBounceVelocityPaddle(Collision2D collider, float maxPaddleBounceAngle);
    void Init(float m_initialForce, int m_power, float m_initialAngle, Rigidbody2D rigidbody2D, bool isExtraBall);
    void UpdateScore(int score);
    bool IsExtraBall { get; set; }
    void ResetBall(Vector2 swipe);
}

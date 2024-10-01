using System;
using Tools;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

public class BallView : MonoBehaviour
{   
    [SerializeField]
    [Range(0f, 90f)]
    private float m_maxPaddleBounceAngle = 75f;
    [SerializeField]
    private float m_initialForce = 50f;
    [SerializeField]
    [Range(0f, 90f)]
    private float m_initialAngle = 45f;
    [SerializeField]
    [Tooltip("How much damage I can inflict on a brick per collision.")]
    private int m_power = 1;

    [Inject] private IPaddlePresenter m_paddle;

    private Transform m_startTr;

    private const string s_brickTag = "Brick";
    private const string s_paddleTag = "Paddle";
    private const string s_deathZone = "Death";

    private float MaxPaddleBounceAngle => m_maxPaddleBounceAngle * Mathf.Deg2Rad;
    private IBallPresenter Ball { get;  set; }

    private void Awake()
    {
        Ball = new BallPresenter();   
    }

    private void Start()
    {       
        var collision = this.OnCollisionEnter2DAsObservable();

        collision
            .Where(collider => collider.gameObject.CompareTag(s_paddleTag))
            .Subscribe(collider =>
            {
                Ball.CalculateBounceVelocityPaddle(collider, MaxPaddleBounceAngle);
                ObservableSignal.Broadcast(new AudioSignal("Bloop"));
            })
            .AddTo(this);

        collision
            .Where(collider => collider.gameObject.CompareTag(s_brickTag))
            .Subscribe(BrickCollision)
            .AddTo(this);

        collision
            .Where(collider => collider.gameObject.CompareTag(s_deathZone))
            .Subscribe(_ =>
            {
                if (Ball.IsExtraBall) 
                {
                    Destroy(gameObject);
                    return;
                }
                ObservableSignal.Broadcast(new BallDeathSignal());
                Ball.Active.Value = false;
            })
            .AddTo(this);

        Ball.Active
            .Subscribe(gameObject.SetActive)
            .AddTo(this);

        ObservableSignal
            .AsObservable<EndLevelSignal>()
            .Subscribe(_ => Ball.Active.Value = false)
            .AddTo(this);

        ObservableSignal
            .AsObservable<ResetBallSignal>()
            .Subscribe(data => ResetBall(data.Swipe))
            .AddTo(this);
    }

    public void Init(bool isExtraBall = false)
    {
        m_startTr = m_paddle.StartTr;
        transform.position = m_startTr.position;
        Ball.Init(m_initialForce, m_power,
            m_initialAngle, GetComponent<Rigidbody2D>(),isExtraBall);
    }

    public void BrickCollision(Collision2D collider) 
    {
        collider.gameObject.GetComponent<IDamage>()?.ApplyDamage(Ball.Power);
        if(collider.gameObject.GetComponent<IScore>() is IScore score)
        {
            Ball.UpdateScore(score.Score);
        }
    }

    public void ResetBall(Vector2 swipe)
    {
        transform.position = m_startTr.position;
        Ball.ResetBall(swipe);
    }

    private void OnDestroy()
    {
        ((IDisposable)Ball)?.Dispose();
    }

    public class Factory : PlaceholderFactory<BallView, BallView> { }
}
public interface IDamage 
{
    void ApplyDamage(int power);
}
public interface IScore 
{
    int Score { get; }
}

public class BallDeathSignal : SignalData 
{ }

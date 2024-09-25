using System;
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
    [SerializeField]
    private Transform m_startPosition;

    private IBallPresenter m_controller;

    private const string s_brickTag = "Brick";
    private const string s_paddleTag = "Paddle";
    private const string s_deathZone = "Death";

    public float MaxPaddleBounceAngle => m_maxPaddleBounceAngle * Mathf.Deg2Rad;
    public IBallPresenter Ball => m_controller;

    private void Start()
    {
        m_controller = new BallPresenter();
        transform.position = m_startPosition.position;
        m_controller.Init(m_initialForce, m_power, m_startPosition.position,
            m_initialAngle, GetComponent<Rigidbody2D>());

        var collision = this.OnCollisionEnter2DAsObservable();

        collision
            .Where(collider => collider.gameObject.CompareTag(s_paddleTag))
            .Subscribe(collider => m_controller.CalculateBounceVelocityPaddle(collider, MaxPaddleBounceAngle))
            .AddTo(this);

        collision
            .Where(collider => collider.gameObject.CompareTag(s_brickTag))
            .Subscribe(BrickCollision)
            .AddTo(this);

        collision
            .Where(collider => collider.gameObject.CompareTag(s_deathZone))
            .Subscribe(_ => m_controller.Active.Value = false)
            .AddTo(this);

        m_controller.Active
            .Subscribe(gameObject.SetActive)
            .AddTo(this);
    }


    public void BrickCollision(Collision2D collider) 
    {
        collider.gameObject.GetComponent<IDamage>()?.ApplyDamage(m_controller.Power);
        if(collider.gameObject.GetComponent<IScore>() is IScore score)
        {
            m_controller.UpdateScore(score.Score);
        }
    }

    public void ResetBall()
    {
        transform.position = m_startPosition.position;
        m_controller.Active.Value = true;
        m_controller.AddInitialForce();
    }
}
public interface IDamage 
{
    void ApplyDamage(int power);
}
public interface IScore 
{
    int Score { get; }
}

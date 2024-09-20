using System;
using UniRx;
using UnityEngine;

public class BallController 
{
    public float InitialForce { get; private set; }

    public int Power { get; private set; }

    public Vector3 StartPosition { get; private set; }

    public IReactiveProperty<bool> Active { get; private set; }

    private float m_initialAngle;
    private Rigidbody2D m_rigidbody;
    public BallController()
    {
        Active = new ReactiveProperty<bool>(true);
    }
    public void Init(float initialForce, int power, Vector3 startPosition, float initialAngle, Rigidbody2D rigidbody)
    {
        InitialForce = initialForce;
        Power = power;
        StartPosition = startPosition;
        m_initialAngle = initialAngle;
        m_rigidbody = rigidbody;
    }

    public void AddInitialForce()
    {
        var force = new Vector2
        {
            x = Mathf.Sin(m_initialAngle * Mathf.Deg2Rad) * InitialForce,
            y = Mathf.Cos(m_initialAngle * Mathf.Deg2Rad) * InitialForce
        };
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
        m_rigidbody.velocity = Vector2.zero;  // Reset velocity before applying bounce
        m_rigidbody.AddForce(bounceForce);
    }
}

using UniRx;
using UnityEngine;

public class BallController 
{
    public float InitialForce { get; }

    public int Power { get; }

    public Vector3 StartPosition { get; }

    public IReactiveProperty<bool> Active { get; }

    private float m_initialAngle;

    public BallController(float initialForce, int power, Vector3 startPosition, float initialAngle)
    {
        InitialForce = initialForce;
        Power = power;
        StartPosition = startPosition;
        Active = new ReactiveProperty<bool>(true);
        m_initialAngle = initialAngle;
    }

    public Vector2 AddInitialForce()
    {
        var force = new Vector2
        {
            x = Mathf.Sin(m_initialAngle * Mathf.Deg2Rad) * InitialForce,
            y = Mathf.Cos(m_initialAngle * Mathf.Deg2Rad) * InitialForce
        };

        return force;
    }

    public Vector2 CalculateBounceVelocityPaddle(Collision2D collision, float maxPaddleBounceAngle)
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
        return bounceForce;
    }
}

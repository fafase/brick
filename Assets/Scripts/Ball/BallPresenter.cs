using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BallPresenter : MonoBehaviour
{
    [SerializeField]
    [Range(0f, 90f)]
    private float m_maxPaddleBounceAngle = 75f;
    [SerializeField]
    private float m_initialForce = 50f;
    [SerializeField]
    [Tooltip("How much damage I can inflict on a brick per collision.")]
    private int m_power = 1;
    [SerializeField]
    private Transform m_startPosition;

    private const string s_collisionTag = "BallBounce";
    private const string s_paddleTag = "Paddle";

    private BallController m_ballController;
    private Rigidbody2D m_rigidbody;
    public float MaxPaddleBounceAngle => m_maxPaddleBounceAngle * Mathf.Deg2Rad;

    private void Start()
    {
        transform.position = m_startPosition.position;
        m_rigidbody = GetComponent<Rigidbody2D>();    
        m_ballController = new(m_initialForce, m_power, m_startPosition.position, m_initialForce);
        SetInitialForce();

        var collision = this.OnCollisionEnter2DAsObservable();

        collision
            .Where(collider => collider.gameObject.CompareTag(s_paddleTag))
            .Subscribe(ProcessBouncePaddle);
    }

    private void ProcessBouncePaddle(Collision2D collider) 
    {
        Vector2 bounceForce = m_ballController.CalculateBounceVelocityPaddle(collider, MaxPaddleBounceAngle);
        m_rigidbody.velocity = Vector2.zero;
        m_rigidbody.AddForce(bounceForce);
    }

    private void SetInitialForce() 
    {
        Vector2 force = m_ballController.AddInitialForce(this);
        m_rigidbody.AddForce(force);
    }
}

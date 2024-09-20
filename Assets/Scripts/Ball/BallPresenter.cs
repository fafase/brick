using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BallPresenter : Presenter<BallController>
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

    private const string s_brickTag = "Brick";
    private const string s_paddleTag = "Paddle";
    private const string s_deathZone = "Death";

    public float MaxPaddleBounceAngle => m_maxPaddleBounceAngle * Mathf.Deg2Rad;

    private void Start()
    {
        transform.position = m_startPosition.position;   
        m_controller.Init(m_initialForce, m_power, m_startPosition.position, 
            m_initialAngle, GetComponent<Rigidbody2D>());

        m_controlelr.AddInitialForce();
        var collision = this.OnCollisionEnter2DAsObservable();

        collision
            .Where(collider => collider.gameObject.CompareTag(s_paddleTag))
            .Subscribe(collider => m_ballController.CalculateBounceVelocityPaddle(collider, MaxPaddleBounceAngle))
            .AddTo(this);

        collision
            .Where(collider => collider.gameObject.CompareTag(s_brickTag))
            .Subscribe(collider => collider.gameObject.GetComponent<IDamage>().ApplyDamage(m_ballController.Power))
            .AddTo(this);
        
        collision
            .Where(collider => collider.gameObject.CompareTag(s_deathZone))
            .Subscribe(_ => m_ballController.Active.Value = false)
            .AddTo(this);

        m_controller.Active
            .Where(value => value == false)
            .Subscribe(gameObject.SetActive)
            .AddTo(this);
    }

}
public interface IDamage 
{
    void ApplyDamage(int power);
}

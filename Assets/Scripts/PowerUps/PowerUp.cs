using Tools;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class PowerUp : MonoBehaviour
{
    [SerializeField] private float m_speed;
    [SerializeField] private GameObject m_fx;
    private PowerUpPresenter m_presenter;

    protected virtual void Start()
    {
        m_presenter = new PowerUpPresenter();
        m_presenter.Init(m_speed, GetComponent<Rigidbody2D>());

        this.OnTriggerEnter2DAsObservable()
            .Where(collider => collider.gameObject.CompareTag(TagContent.PADDLE) || collider.gameObject.CompareTag(TagContent.DEATH))
            .Subscribe(OnCollision) 
            .AddTo(this);
    }

    protected virtual void OnCollision(Collider2D collider) 
    {
        if (collider.gameObject.CompareTag(TagContent.PADDLE))
        {
            ApplyEffect(collider);
        }
        if(m_fx != null) 
        {
            Instantiate(m_fx);
        }
        Destroy(gameObject);
    }

    protected abstract void ApplyEffect(Collider2D collider);

    public class Factory : PlaceholderFactory<Object, PowerUp> { }
}

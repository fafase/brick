using Tools;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class PowerUp : MonoBehaviour
{
    [SerializeField] protected PowerUpType m_powerUpType;
    [SerializeField] private float m_speed;
    [SerializeField] private GameObject m_fx;
    private PowerUpPresenter m_presenter;
    protected bool m_destroyOnCollision = true;

    public PowerUpType PowerUpType => m_powerUpType;
    
    protected virtual void Start()
    {
        m_presenter = new PowerUpPresenter();
        m_presenter.Init(m_speed, GetComponent<Rigidbody2D>());

        this.OnTriggerEnter2DAsObservable()
            .Where(collider => collider.gameObject.CompareTag(TagContent.PADDLE) || collider.gameObject.CompareTag(TagContent.DEATH))
            .Subscribe(OnCollision) 
            .AddTo(this);
    }

    private void OnDestroy()
    {
        m_presenter?.Dispose();
        ObservableSignal.Broadcast(new PowerUpSignal(this, false));
    }

    protected virtual void OnCollision(Collider2D collider) 
    {
        if (collider.gameObject.CompareTag(TagContent.PADDLE))
        {
            ApplyEffect(collider);
            if (m_destroyOnCollision)
            {
                Destroy(gameObject);
            }
            if (m_fx != null)
            {
                Instantiate(m_fx);
            }
            return;
        }
        Destroy(gameObject);

    }

    protected abstract void ApplyEffect(Collider2D collider);

    public class Factory : PlaceholderFactory<PowerUp, PowerUp> { }
}

public enum PowerUpType
{
    ExtraBall, GrowPad, Shoot, ShrinkPad
}

public class PowerUpSignal : SignalData 
{
    public readonly bool IsStarting;
    public readonly PowerUp PowerUp;

    public PowerUpSignal(PowerUp powerUp, bool isStarting)
    {
        PowerUp = powerUp;
        IsStarting = isStarting;
    }
}

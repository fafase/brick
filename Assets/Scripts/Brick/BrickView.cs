using UniRx;
using UnityEngine;
using Tools;
using Zenject;

public class BrickView : View<BrickController>, IDamage, IScore
{
    [SerializeField] private int m_health = 1;
    [SerializeField] private int m_score = 100;
    [SerializeField] private GameObject m_destructionFX;

    public int Score => m_score;

    public IBrick Brick => m_presenter;

    void Start() 
    {
        m_presenter.Init(m_health);
        m_presenter.Health
            .Where(value => value <= 0)
            .Subscribe(_ => DestroyBrick())
            .AddTo(this);
    }

    public void ApplyDamage(int power) 
    {
        if (power <= 0)
        {
            Debug.LogWarning("Invalid power value: Power must be greater than zero.");
            return;
        }
        m_presenter.ApplyDamage(power); 
    }

    private void DestroyBrick() 
    {
        if (m_destructionFX != null)
        {
            Instantiate(m_destructionFX, transform.position, Quaternion.identity);
        }
        Destroy(gameObject); 
    }

    public class Factory:PlaceholderFactory<Object, BrickView> { }
}

public interface IBrick 
{
    IReactiveProperty<int> Health { get; }

    void Init(int health);

    void ApplyDamage(int damage);
}

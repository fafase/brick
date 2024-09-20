using UniRx;
using UnityEngine;

public class BrickPresenter : Presenter<BrickController>, IDamage, IScore
{
    [SerializeField] private int m_health = 1;
    [SerializeField] private int m_score = 100;
    [SerializeField] private GameObject m_destructionFX;

    public int Score => m_score;

    void Start() 
    {
        m_controller.Init(m_health);
        m_controller.HealthProperty
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
        m_controller.ApplyDamage(power); 
    }

    private void DestroyBrick() 
    {
        // Optionally trigger some visual destruction effect before the brick is destroyed
        if (m_destructionFX != null)
        {
            Instantiate(m_destructionFX, transform.position, Quaternion.identity);
        }
        Destroy(gameObject); }
}

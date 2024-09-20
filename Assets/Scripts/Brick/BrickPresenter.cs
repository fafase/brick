using UniRx;
using UnityEngine;

public class BrickPresenter : MonoBehaviour, IDamage
{
    [SerializeField] private int m_health = 1;
    [SerializeField] private GameObject m_destructionFX;

    private BrickController m_brickController;

    void Start() 
    {
        m_brickController = new BrickController(m_health);
        m_brickController.HealthProperty
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
        m_brickController.ApplyDamage(power); 
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

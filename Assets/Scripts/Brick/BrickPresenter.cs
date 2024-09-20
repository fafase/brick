using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

public class BrickPresenter : MonoBehaviour, IDamage
{
    [SerializeField] private int m_health = 1;

    private BrickController m_brickController;

    void Start() 
    {
        m_brickController = new BrickController(m_health);
        m_brickController.HealthProperty
            .Where(value => value <= 0)
            .Subscribe(_ => DestroyBrick())
            .AddTo(this);
    }

    public void ApplyDamage(int power) => m_brickController.ApplyDamage(power);
    private void DestroyBrick() => Destroy(gameObject);
}

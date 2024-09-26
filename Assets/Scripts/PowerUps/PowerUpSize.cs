using System;
using Tools;
using UniRx;
using UnityEngine;

public class PowerUpSize : PowerUp
{
    [SerializeField] private SpriteRenderer m_renderer;
    [SerializeField] private Collider2D m_collider;

    protected override void Start()
    {
        base.Start();
        m_destroyOnCollision = false;
    }
    protected override void ApplyEffect(Collider2D collider)
    {
        m_collider.enabled = false;
        m_renderer.enabled = false;
        ObservableSignal
            .Broadcast(new PowerUpSignal(this, true));

        Observable
            .Timer(TimeSpan.FromSeconds(5), Scheduler.MainThread)
            .Subscribe(_ => Destroy(gameObject))
            .AddTo(this);
    }
}

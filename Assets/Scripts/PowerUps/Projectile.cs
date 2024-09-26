using UniRx;
using UniRx.Triggers;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    int m_damage = 1;
    float m_speed = 10f;

    void Start()
    {
        Rigidbody2D rigidbody2D = GetComponent<Rigidbody2D>();
        Vector2 vel = new Vector2(0f, m_speed);
        rigidbody2D.velocity = vel;

        this.OnCollisionEnter2DAsObservable()
            .Where(collider => collider.gameObject.CompareTag(TagContent.BRICK) || collider.gameObject.CompareTag(TagContent.BOUNCE))
            .Subscribe(OnCollision)
            .AddTo(this);
    }

    private void OnCollision(Collision2D collider)
    {
        if (collider.gameObject.CompareTag(TagContent.BRICK))
        {
            collider.gameObject.GetComponent<IDamage>()?.ApplyDamage(m_damage);
        }
        Destroy(gameObject);
    }
}

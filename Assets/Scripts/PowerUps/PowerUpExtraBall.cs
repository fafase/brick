using UnityEngine;
using Zenject;

public class PowerUpExtraBall : PowerUp
{
    [SerializeField] private BallView m_ballPrefab;
    [Inject] private IPaddlePresenter m_paddle;
    [Inject] private ICore m_core;

    protected override void ApplyEffect(Collider2D collider)
    {
        BallView ball = m_core.CreateBall(m_ballPrefab); 
        ball.Init(m_paddle.StartTr);
        ball.Ball.IsExtraBall = true;
        ball.transform.localScale = ball.transform.localScale * 0.7f;
        ball.GetComponent<SpriteRenderer>().color = Color.blue;
        ball.ResetBall();
    }
}

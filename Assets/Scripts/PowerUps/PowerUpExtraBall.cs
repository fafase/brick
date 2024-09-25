using UnityEngine;
using Zenject;

public class PowerUpExtraBall : PowerUp
{
    [SerializeField] private BallView m_ballPrefab;
    [Inject] private IPaddlePresenter m_paddle;
    [Inject] private ICore m_core;

    protected override void ApplyEffect(Collider2D collider)
    {
        //BallView ball = m_core.CreateBall(m_ballPrefab); //Instantiate(m_ballPrefab, m_paddle.StartPosition, Quaternion.identity);
        //ball.Ball.AddInitialForce();
        //ball.Ball.IsExtraBall = true;
    }
}

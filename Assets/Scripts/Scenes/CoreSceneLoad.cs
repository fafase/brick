using UnityEngine;

public class CoreSceneLoad : MonoBehaviour
{
    [Header("Objects to load")]
    [SerializeField] private BallView m_ballPrefab;

    [Header("Dependencies")]
    [SerializeField] private GameView m_gameView;
    [SerializeField] private PaddleView m_paddle;

    private void Awake()
    {
        
        BallView ball = Instantiate(m_ballPrefab, m_paddle.StartTransform.position, Quaternion.identity);
        ball.Init(m_paddle.StartTransform);

        m_gameView.Ball = ball;
    }
}

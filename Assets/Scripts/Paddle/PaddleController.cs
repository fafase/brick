using UniRx;
using UnityEngine;

public class PaddleController
{
    private float m_speed;
    private Vector3 m_defaultPosition;
    private Vector2 m_limits;

    private readonly float m_defaultSize;
    private readonly IPaddleProvider m_provider;
    private readonly float m_deckLeft, m_deckRight;

    public ReactiveProperty<Vector3> PaddlePos = new ReactiveProperty<Vector3>();
    public ReactiveProperty<float> PaddleScale = new ReactiveProperty<float>();

    public PaddleController(IPaddleProvider provider, Vector3 defaultPos)
    {
        m_provider = provider;
        m_deckLeft = Camera.main.WorldToScreenPoint(m_provider.XLeft).x;
        m_deckRight = Camera.main.WorldToScreenPoint(m_provider.XRight).x;

        m_defaultPosition = defaultPos;
        PaddleScale.Value =  m_provider.Scale;
        PaddleScale
            .DelayFrame(1)
            .Subscribe(SetLimits);
        SetLimits(0f);
    }

    public void ProcessPosition(Vector3 mousePos)
    {
        mousePos.x = Mathf.Clamp(mousePos.x, m_limits.x, m_limits.y);
        var xPos = Camera.main.ScreenToWorldPoint(mousePos);
        PaddlePos.Value = new Vector3(xPos.x, m_defaultPosition.y, m_defaultPosition.z);
    }

    public void SetScale(long obj) => PaddleScale.Value = (PaddleScale.Value > 2.5f) ? 2f : 3f;

    private void SetLimits(float newScale) 
    {
        float paddleHalfWidthWorld = m_provider.Size / 2f;
        Vector3 paddleScreenSpace = Camera.main.WorldToScreenPoint(new Vector3(paddleHalfWidthWorld, 0, 0));
        float paddleHalfWidthScreen = paddleScreenSpace.x - Camera.main.WorldToScreenPoint(Vector3.zero).x;

        float adjustedDeckLeft = m_deckLeft + paddleHalfWidthScreen;
        float adjustedDeckRight = m_deckRight - paddleHalfWidthScreen;
        m_limits = new Vector2(adjustedDeckLeft, adjustedDeckRight);
    }
}

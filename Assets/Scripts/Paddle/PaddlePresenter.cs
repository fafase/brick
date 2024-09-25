using UniRx;
using UnityEngine;

public class PaddlePresenter : IPaddlePresenter
{
    private Vector3 m_defaultPosition;
    private Vector2 m_limits;

    private IPaddleProvider m_provider;
    private float m_deckLeft, m_deckRight;

    public ReactiveProperty<Vector3> PaddlePos { get; private set; } = new ReactiveProperty<Vector3>();
    public ReactiveProperty<float> PaddleScale { get; private set; } = new ReactiveProperty<float>();
    public Vector3 StartPosition => m_provider.StartTransform.position;
    public void Init(IPaddleProvider provider, Vector3 defaultPos, float deckLeftScreen, float deckRightScreen)
    {
        m_provider = provider;
        m_defaultPosition = defaultPos;

        m_deckLeft = deckLeftScreen;
        m_deckRight = deckRightScreen;

        PaddleScale.Value =  m_provider.Scale;
        PaddleScale
            .Skip(1)
            .DelayFrame(1)
            .Subscribe(SetLimits)
            .AddTo(m_provider as Component);

        SetLimits(PaddleScale.Value);
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
public interface IPaddlePresenter 
{
    ReactiveProperty<Vector3> PaddlePos { get; }
    ReactiveProperty<float> PaddleScale { get; }

    void SetScale(long obj);
    void ProcessPosition(Vector3 mousePos);
    void Init(IPaddleProvider provider, Vector3 defaultPos, float deckLeftScreen, float deckRightScreen);
}

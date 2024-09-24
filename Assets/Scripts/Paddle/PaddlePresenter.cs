using UniRx;
using UnityEngine;
using Zenject;

public class PaddleView : MonoBehaviour, IPaddleProvider
{
    [SerializeField] private Transform m_paddleTr;
    [SerializeField] private Transform m_deckLeftLimit, m_deckRightLimit;
    [SerializeField] private Transform m_paddleLeftLimit, m_paddleRightLimit;

    [Inject] private IGamePresenter m_gamePresenter;

    private PaddleController m_paddleController;

    public float Size => GetComponent<SpriteRenderer>().bounds.size.x;
    public float Scale => transform.localScale.x;

    private void Start()
    {
        SetPaddleController();
        BindInput();
    }

    private void SetPaddleController() 
    {
        // Calculate the screen limits in world space
        float deckLeftScreen = Camera.main.WorldToScreenPoint(m_deckLeftLimit.position).x;
        float deckRightScreen = Camera.main.WorldToScreenPoint(m_deckRightLimit.position).x;
        m_paddleController = new(this, m_paddleTr.position, deckLeftScreen, deckRightScreen);
        m_paddleController.PaddlePos
            .Skip(1)
            .Subscribe(newPosition => m_paddleTr.position = newPosition)
            .AddTo(this);

        m_paddleController.PaddleScale
            .Subscribe(newScale =>
            {
                Vector3 scale = m_paddleTr.localScale;
                m_paddleTr.localScale = new Vector3(newScale, scale.y, scale.z);
            });
    }

    private void BindInput() 
    {
        var update = Observable.EveryUpdate();
        update
            .Where(_ => Input.GetMouseButton(0) && m_gamePresenter.CurrentGameState == GameState.Play)
            .Select(_ => Input.mousePosition)
            .Subscribe(m_paddleController.ProcessPosition)
            .AddTo(this);
    }
}

public interface IPaddleProvider 
{
    float Size { get; }
    float Scale { get; }
}

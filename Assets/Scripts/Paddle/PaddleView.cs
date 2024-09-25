using UniRx;
using UnityEngine;
using Zenject;

public class PaddleView : MonoBehaviour, IPaddleProvider
{
    [SerializeField] private Transform m_paddleTr;
    [SerializeField] private Transform m_deckLeftLimit, m_deckRightLimit;
    [SerializeField] private Transform m_paddleLeftLimit, m_paddleRightLimit;
    [SerializeField] private Transform m_startPosition;

    [Inject] private IGamePresenter m_gamePresenter;

    [Inject] private IPaddlePresenter m_presenter;

    public float Size => GetComponent<SpriteRenderer>().bounds.size.x;
    public float Scale => transform.localScale.x;
    public Transform SartPosition => m_startPosition;

    private void Start()
    {
        SetPaddleController();
        BindInput();
    }

    private void SetPaddleController() 
    {
        float deckLeftScreen = Camera.main.WorldToScreenPoint(m_deckLeftLimit.position).x;
        float deckRightScreen = Camera.main.WorldToScreenPoint(m_deckRightLimit.position).x;
        m_presenter.Init(this, m_paddleTr.position, deckLeftScreen, deckRightScreen);
        m_presenter.PaddlePos
            .Skip(1)
            .Subscribe(newPosition => m_paddleTr.position = newPosition)
            .AddTo(this);

        m_presenter.PaddleScale
            .Subscribe(newScale =>
            {
                Vector3 scale = m_paddleTr.localScale;
                m_paddleTr.localScale = new Vector3(newScale, scale.y, scale.z);
            });
    }

    private void BindInput() 
    {
        Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButton(0) && m_gamePresenter.CurrentGameState == GameState.Play)
            .Select(_ => Input.mousePosition)
            .Subscribe(m_presenter.ProcessPosition)
            .AddTo(this);
    }
}

public interface IPaddleProvider 
{
    float Size { get; }
    float Scale { get; }
    Transform StartTransform { get; }
}

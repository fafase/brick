using System;
using UniRx;
using UnityEngine;
using Zenject;

public class PaddleView : MonoBehaviour, IPaddleProvider
{
    [SerializeField] private Transform m_leftDeck;
    [SerializeField] private Transform m_rightDeck;
    [SerializeField] private Transform m_paddleLeftLimit, m_paddleRightLimit;
    [SerializeField] private Transform m_startPosition;

    [Inject] private IGamePresenter m_gamePresenter;
    [Inject] private IPaddlePresenter m_presenter;
    [Inject] private ICore m_core;

    public float Size => GetComponent<SpriteRenderer>().bounds.size.x;
    public float Scale => transform.localScale.x;
    public Transform StartTransform => m_startPosition;

    private void Start()
    {
        SetPaddleController();
        BindInput();
    }

    private void SetPaddleController() 
    {
        float deckLeftScreen = Camera.main.WorldToScreenPoint(m_leftDeck.position).x;
        float deckRightScreen = Camera.main.WorldToScreenPoint(m_rightDeck.position).x;
        m_presenter.Init(this, transform.position, deckLeftScreen, deckRightScreen);
        m_presenter.PaddlePos
            .Skip(1)
            .Subscribe(newPosition => transform.position = newPosition)
            .AddTo(this);

        m_presenter.PaddleScale
            .Subscribe(newScale =>
            {
                Vector3 scale = transform.localScale;
                transform.localScale = new Vector3(newScale, scale.y, scale.z);
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

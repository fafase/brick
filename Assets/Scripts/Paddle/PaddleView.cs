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
    [SerializeField] private Transform m_leftCannon;
    [SerializeField] private Transform m_rightCannon;
    [SerializeField] private GameObject m_shootPrefab;

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
            .Where(_ => Input.GetMouseButton(0) && (m_gamePresenter.CurrentGameState == GameState.Play || m_gamePresenter.CurrentGameState == GameState.Waiting))
            .Select(_ => Input.mousePosition)
            .Subscribe(m_presenter.ProcessPosition)
            .AddTo(this);
    }

    public void SetCanons(float ratio, bool isStarting) 
    {
        Vector3 startL = m_leftCannon.localPosition;
        Vector3 endL = new Vector3(startL.x, isStarting ? 0.3f : 0.0f, 0f);
        m_leftCannon.localPosition = Vector3.Lerp(startL, endL, ratio);

        Vector3 startR = m_rightCannon.localPosition;
        Vector3 endR = new Vector3(startR.x, isStarting ? 0.3f : 0.0f, 0f);
        m_rightCannon.localPosition = Vector3.Lerp(startR, endR, ratio);
    }

    public void Shoot() 
    {
        void GetProjectile(Vector3 position) 
        {
            GameObject projectile = Instantiate(m_shootPrefab);
            projectile.SetActive(true);
            projectile.transform.position = position;
        }
        GetProjectile(m_leftCannon.position);
        GetProjectile(m_rightCannon.position);
    }

    private void OnDestroy()
    {
        (m_presenter as IDisposable)?.Dispose();
    }
}

public interface IPaddleProvider 
{
    float Size { get; }
    float Scale { get; }
    Transform StartTransform { get; }
    void SetCanons(float ratio, bool isStarting);
    void Shoot();
}

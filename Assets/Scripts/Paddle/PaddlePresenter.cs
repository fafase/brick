using Cysharp.Threading.Tasks;
using System;
using Tools;
using UniRx;
using UnityEngine;

public class PaddlePresenter : Presenter, IPaddlePresenter
{
    private Vector3 m_defaultPosition;
    private Vector2 m_limits;

    private IPaddleProvider m_provider;
    private float m_deckLeft, m_deckRight;

    public ReactiveProperty<Vector3> PaddlePos { get; private set; } = new ReactiveProperty<Vector3>();
    public ReactiveProperty<float> PaddleScale { get; private set; } = new ReactiveProperty<float>();
    public Vector3 StartPosition => m_provider.StartTransform.position;
    public Transform StartTr => m_provider.StartTransform;
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

        ObservableSignal
             .AsObservable<PowerUpSignal>()
             .Where(data => !data.PowerUp.PowerUpType.Equals(PowerUpType.ExtraBall))
             .Subscribe(data => 
             {
                 switch (data.PowerUp.PowerUpType) 
                 {
                    case PowerUpType.Shoot:
                         PowerUpShoot();
                         break;
                    case PowerUpType.ShrinkPad:
                         PaddleScale.Value = (data.IsStarting) ? PaddleScale.Value /= 2f : PaddleScale.Value *= 2;
                         break;
                    case PowerUpType.GrowPad:
                         PaddleScale.Value = (data.IsStarting) ? PaddleScale.Value *= 1.5f : PaddleScale.Value /= 1.5f;
                         break;
                    default:
                         break;
                 }
             })
             .AddTo(m_compositeDisposable);
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

    private void PowerUpShoot() 
    {
        CannonAnimSequence(0.25f, true)
            .Concat(CannonShootingSequence(0.2f, 2f))
            .Concat(CannonAnimSequence(0.3f, false))
            .Subscribe()
            .AddTo(m_compositeDisposable);
    }


    private IObservable<Unit> CannonAnimSequence(float duration, bool isStarting) 
    {
        float elapsedTime = 0f;
        return Observable
            .EveryUpdate()
            .TakeWhile(_ => elapsedTime < duration)
            .Do(_ =>
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / duration);
                m_provider.SetCanons(t, isStarting);
            })
            .AsUnitObservable();
    }

    private IObservable<Unit> CannonShootingSequence(float interval, float duration) 
    {
        return Observable
                .Interval(TimeSpan.FromSeconds(interval))
                .TakeUntil(Observable.Timer(TimeSpan.FromSeconds(duration)))
                .Do(_ => Shoot())
                .AsUnitObservable();
    }

    private void Shoot() 
    {
        Debug.Log("Pew");
        m_provider.Shoot();
    }
}
public interface IPaddlePresenter 
{
    ReactiveProperty<Vector3> PaddlePos { get; }
    ReactiveProperty<float> PaddleScale { get; }
    Vector3 StartPosition { get; }
    Transform StartTr { get; }
    void SetScale(long obj);
    void ProcessPosition(Vector3 mousePos);
    void Init(IPaddleProvider provider, Vector3 defaultPos, float deckLeftScreen, float deckRightScreen);
}

using UniRx;
using UniRx.Triggers;
using Tools;
using System;

public class PaddleBlob : BlobEffect
{
    private IDisposable m_collisionDisposable = null;

    void Start()
    {
        ObservableSignal
            .AsObservable<ResetBallSignal>()
            .Subscribe(_ =>RegisterCollision())
            .AddTo(this);

        ObservableSignal
            .AsObservable<BallDeathSignal>()
            .Subscribe(_ => UnregisterCollision())
            .AddTo(this);
    }

    private void RegisterCollision()
    {
        m_collisionDisposable?.Dispose();

        m_collisionDisposable = this.OnCollisionEnter2DAsObservable()
                    .Where(collider => collider.gameObject.CompareTag(Tags.BALL))
                    .Subscribe(_ => Blob().Subscribe().AddTo(this))
                    .AddTo(this);
    }

    private void UnregisterCollision() => m_collisionDisposable?.Dispose();
}

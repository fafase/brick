using System;
using UniRx;
using UnityEngine;

namespace Tools
{
    public class BlobEffect : MonoBehaviour, ILeanFX
    {
        [SerializeField] private Effect m_effect;
        [SerializeField] private float m_scaleAmount = 1f;
        [SerializeField] private float m_duration = 0.5f;
        [SerializeField] private float m_delay = 0.0f;
        [SerializeField] private float m_scaleUpSplash = 1.1f;

        private IDisposable m_animationSubscription;

        protected virtual void OnDestroy()
        {
            m_animationSubscription?.Dispose();
        }

        public virtual IObservable<Unit> PlayAnimation() 
        {
            return Observable.Create<Unit>(observer =>
            {
                m_animationSubscription =  Observable.Timer(TimeSpan.FromSeconds(m_delay))
                .Subscribe(_ =>
                {
                    LeanTween.cancel(gameObject);
                    switch (m_effect)
                    {
                        case Effect.None:
                            Debug.LogWarning("Missing animation effect");
                            observer.OnCompleted();
                            break;
                        case Effect.Blob:
                            Blob()
                                .Subscribe();
                            break;
                        case Effect.Disappear:
                            Disappear()
                                .Subscribe();
                            break;
                        case Effect.Splash:
                            Splash()
                                .Subscribe();
                            break;
                    }
                }).AddTo(this);

                return Disposable.Create(() => m_animationSubscription?.Dispose());
            });
        }

        protected virtual IObservable<Unit> Disappear() 
        {
            return Observable.Create<Unit>(observer => 
            {
                Vector3 scale = gameObject.transform.localScale;
                Vector2 vec = new Vector2(scale.x, scale.y);
                if (Mathf.Approximately(vec.magnitude, 0.0f)) 
                {
                    observer.OnNext(Unit.Default);
                    observer.OnCompleted();
                    return Disposable.Empty;    
                }
                LeanTween.scale(gameObject, Vector3.zero, m_duration)
                    .setOnComplete(() => 
                    {
                        observer.OnNext(Unit.Default);
                        observer.OnCompleted();
                    });
                return Disposable.Empty;
            });
        }

        protected virtual IObservable<Unit> Blob() 
        {
            return Observable.Create<Unit>(observer => 
            {
                Vector3 initialScale = transform.localScale;
                LeanTween.scale(gameObject, new Vector3(m_scaleAmount, m_scaleAmount, 1f), m_duration)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() =>
                 {
                     LeanTween.scale(gameObject, initialScale, m_duration)
                         .setEase(LeanTweenType.easeInOutQuad)
                         .setOnComplete(()=> 
                         {
                             observer.OnNext(Unit.Default);
                             observer.OnCompleted();
                         });
                 });
                return Disposable.Empty;
            });
        }

        protected virtual IObservable<Unit> Splash()
        {
            return Observable.Create<Unit>(observer =>
            {
                Vector3 scaleUpTarget = transform.localScale * m_scaleUpSplash;

                LeanTween.scale(gameObject, scaleUpTarget, m_duration / 2f)
                 .setEase(LeanTweenType.easeInOutQuad)
                 .setOnComplete(() =>
                 {
                     LeanTween.scale(gameObject, Vector3.zero, m_duration)
                         .setEase(LeanTweenType.easeInOutQuad)
                         .setOnComplete(() =>
                         {
                             observer.OnNext(Unit.Default);
                             observer.OnCompleted();
                         });
                 });
                return Disposable.Empty;
            });
        }

        enum Effect { None, Blob, Disappear, Splash }
    }
    public interface ILeanFX
    {
        IObservable<Unit> PlayAnimation();
    }
}

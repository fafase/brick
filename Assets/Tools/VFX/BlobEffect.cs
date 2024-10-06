using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Tools
{
    public class BlobEffect : MonoBehaviour
    {
        [SerializeField] public float m_scaleAmount = 1f;
        [SerializeField] public float m_duration = 0.5f;

        private IDisposable m_blob;
        private Vector3 m_initialScale;

        protected virtual void OnDestroy()
        {
            m_blob?.Dispose();
        }

        public virtual void PlayBlobAnimation() 
        {
            m_initialScale = transform.localScale;
            LeanTween.cancel(gameObject);
            LeanTween.scale(gameObject, new Vector3(m_scaleAmount, m_scaleAmount, 1f), m_duration)
             .setEase(LeanTweenType.easeInOutQuad)
             .setOnComplete(() =>
             {
                 LeanTween.scale(gameObject, m_initialScale, m_duration)
                     .setEase(LeanTweenType.easeInOutQuad);
             });
        }
    }
}

using System;
using UniRx;

namespace Tools
{
    public abstract class Presenter : IDisposable 
    {
        protected CompositeDisposable m_compositeDisposable;
        protected bool m_isDisposed = false;

        public Presenter()
        {
            m_compositeDisposable = new CompositeDisposable();
        }

        public virtual void Dispose()
        {
            if (m_isDisposed) 
            {
                return;
            }
            m_isDisposed = true;  
            m_compositeDisposable?.Dispose();
        }
    }
}

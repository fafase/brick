using UnityEngine;

namespace Tools
{
    public class View<TPresenter> : MonoBehaviour where TPresenter : Presenter, new()
    {
        protected TPresenter m_presenter;

        protected virtual void Awake()
        {
            m_presenter = new TPresenter();
        }

        protected virtual void OnDestroy()
        {
            m_presenter?.Dispose();
        }
    }
}


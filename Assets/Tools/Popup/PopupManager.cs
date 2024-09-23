using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using State = Tools.IPopup.State;
using Zenject;

namespace Tools
{
    public class PopupManager : View<PopupManagerPresenter>, IPopupManager
    {
        [SerializeField] private List<Popup> m_popups = new List<Popup>();
        public IEnumerable<IPopup> Popups => m_popups.Cast<IPopup>();
        public RectTransform Container => transform as RectTransform;
        public IObservable<PopupState> OpenPopups => m_presenter.PopupStateSubject.AsObservable();
        public IReactiveProperty<int> PopupsCountObservable => m_presenter.PopupsCountObservable;
        public int PopupsCount => m_presenter.Count;

        [Inject] private Popup.Factory m_factory;
            
        protected override void Awake()
        {
            base.Awake();
            m_presenter.Init(this);
        }

        public IPopup Show<T>() where T : IPopup
        {
            Result<IPopup> result = m_presenter.Show<T>();
            result.CheckForDebug();
            return result.Obj;
        }

        public void Close(IPopup popup)
        {
            m_presenter.Close(popup)
                .CheckForDebug();
        }
    
        public bool IsOpen<T>() where T : IPopup => m_presenter.IsOpen<T>();

        public IObservable<T> GetPopup<T>() where T : IPopup => m_presenter.GetPopup<T>();

        public IPopup Clone(IPopup popup) 
        {
            if(popup == null) 
            {
                Debug.LogError("Popup is missing");
                return null;
            }
            if (popup is Popup p) 
            {
                return m_factory.Create(p); //Instantiate(p);
            }
            return null;
        }
    }

    public class PopupState
    {
        public IPopup Popup { get; }
        public State CurrentState { get; }

        public PopupState(IPopup popup, State currentState)
        {
            Popup = popup;
            CurrentState = currentState;
        }
    }

}

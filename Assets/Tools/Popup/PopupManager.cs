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

        private const string m_popupSfx = "Swoosh";

        protected override void Awake()
        {
            base.Awake();
            m_presenter.Init(this);
        }

        public T Show<T>() where T : IPopup
        {
            Result<IPopup> result = m_presenter.Show<T>();
            result.CheckForDebug();
            if (result.IsSuccess) 
            {
                ObservableSignal.Broadcast(new AudioSignal(m_popupSfx));
            }
            return (T)result.Obj;
        }

        public void Close(IPopup popup)
        {
            Result result = m_presenter.Close(popup);
            result.CheckForDebug();
            if (result.IsSuccess)
            {
                ObservableSignal.Broadcast(new AudioSignal(m_popupSfx));
            }
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
                return m_factory.Create(p); 
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

    //public class PopupSignal : SignalData 
    //{
    //    public readonly Type PopupType;
    //    public readonly IPopup.State State;

    //    public PopupSignal(Type popupType, State state)
    //    {
    //        PopupType = popupType;
    //        State = state;
    //    }
    //}
}

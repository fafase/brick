using System;
using System.Collections;
using UniRx;
using UnityEngine;
using State = Tools.IPopup.State;

namespace Tools
{
    public class PopupPresenter : Presenter, IDisposable
    {
        private IPopupManager m_popupManager;
        private IPopup m_popupView;

        private AsyncSubject<IPopup> m_onClose = new AsyncSubject<IPopup>();
        private AsyncSubject<IPopup> m_onOpen = new AsyncSubject<IPopup>();
        private AsyncSubject<State> m_state = new AsyncSubject<State>();

        public IReadOnlyReactiveProperty<bool> IsOpen { get; private set; }
        public IReactiveProperty<State> PopupState { get; private set; }
        public IObservable<IPopup> OnCloseAsObservable => m_onClose.AsObservable();
        public IObservable<IPopup> OnOpenAsObservable => m_onOpen.AsObservable();

        public void Init(IPopupManager popupManager, IPopup popupView)
        {
            m_popupManager = popupManager;
            m_popupView = popupView;
            m_compositeDisposable = new CompositeDisposable();

            PopupState = new ReactiveProperty<State>(State.Idle);
            IsOpen = PopupState
                .Select(state => state == State.Opening || state == State.Idle)
                .ToReadOnlyReactiveProperty();

            m_popupView.OnCloseAsObservable
                .Subscribe(_=> OnClose())
                .AddTo(m_compositeDisposable);

            OpenPopup();
        }

        private void OpenPopup()
        {
            PopupState.Value = State.Opening;
            m_popupView.SetButtonsInteractable(false);
            m_onOpen.OnNext(m_popupView);

            m_popupView.Open()
                .DoOnCompleted(() => 
                    {
                        PopupState.Value = State.Idle;
                        m_popupView.SetButtonsInteractable(true);
                        m_onOpen.OnCompleted();
                    })
                .Subscribe()
                .AddTo(m_compositeDisposable);
        }


        public void ClosePopup(IObservable<Unit> obs)
        {
            m_onClose.OnNext(m_popupView);
            if (obs == null)
            {
                OnClose();
                return;
            }

            PopupState.Value = State.Closing;
            m_popupView.SetButtonsInteractable(false);
            obs.DoOnCompleted(OnClose)
                .Subscribe()
                .AddTo(m_compositeDisposable);
        }

        private void OnClose() 
        {
            m_popupManager.Close(m_popupView);
            m_onClose.OnCompleted();
            m_compositeDisposable?.Dispose();
        }

        public override void Dispose()
        {
            if (m_isDisposed) 
            {
                return;
            }
            base.Dispose();
            (PopupState as ReactiveProperty<State>)?.Dispose();
            (IsOpen as ReadOnlyReactiveProperty<bool>)?.Dispose();
        }
    }
}

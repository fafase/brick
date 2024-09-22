using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using State = Tools.IPopup.State;

namespace Tools
{
    public class PopupManagerPresenter : Presenter
    {
        private IDictionary<Type, IPopup> m_prefabs;
        private ReactiveCollection<IPopup> m_openPopups = new ReactiveCollection<IPopup>();
        private IPopupManager m_popupManager;

        public Subject<PopupState> PopupStateSubject = new Subject<PopupState>();
        public IReactiveProperty<int> PopupsCountObservable { get; private set; }
        public int Count => m_openPopups.Count;

        public void Init(IPopupManager popupManager) 
        {
            m_compositeDisposable = new CompositeDisposable();
            m_popupManager = popupManager;
            m_prefabs = m_popupManager.Popups.ToDictionary(p => p.GetType(), p => p);
            PopupsCountObservable = new ReactiveProperty<int>(m_openPopups.Count);

            m_openPopups.ObserveAdd()
                .Subscribe(popup => EmitChange(popup.Value, State.Opening))
                .AddTo(m_compositeDisposable);

            m_openPopups.ObserveRemove()
                .Subscribe(popup => EmitChange(popup.Value, State.Closing))
                .AddTo(m_compositeDisposable);
        }

        private void EmitChange(IPopup popup, State state)
        {
            PopupsCountObservable.Value = m_openPopups.Count;
            PopupStateSubject.OnNext(new PopupState(popup, state));
        }

        public Result<IPopup> Show<T>() where T : IPopup
        {          
            if (m_prefabs.TryGetValue(typeof(T), out IPopup popup))
            {
                IPopup instance = m_popupManager.Clone(popup);
                m_openPopups.Add(instance);
                instance.Init(m_popupManager);
                return Result<IPopup>.Success(instance);
            }
            return Result<IPopup>.Failure("Popup prefab not found");
        }


        public Result Close(IPopup popup)
        {
            if (m_openPopups.Contains(popup))
            {
                m_openPopups.Remove(popup);
                return Result.Success(); 
            }
            return Result.Failure("Popup was not already open");
        }

        public bool IsOpen<T>() where T : IPopup => GetPopup<T>() != null;

        public IObservable<T> GetPopup<T>() where T : IPopup
        {
            Type t = typeof(T);
            int index = m_openPopups.ToList().FindIndex(popup => popup.GetType().Equals(t));
            if (index < 0)
            {
                return null;
            }
            return m_openPopups[index] as IObservable<T>;
        }

        public override void Dispose() 
        {
            if (m_isDisposed) return;
            base.Dispose();
            PopupStateSubject?.OnCompleted();
            PopupStateSubject?.Dispose();
            (PopupsCountObservable as ReactiveProperty<int>)?.Dispose();
            m_openPopups.Dispose();
        }
    }
}

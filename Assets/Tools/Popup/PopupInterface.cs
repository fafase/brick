using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Tools 
{
    public interface IPopup
    {
        IObservable<Unit> Open();
        IObservable<Unit> Close(bool closeImmediate = false);
        void Init(IPopupManager popupManager);
        IReactiveProperty<State> PopupState { get; }
        IReadOnlyReactiveProperty<bool> IsOpen { get; }
        IObservable<IPopup> OnCloseAsObservable { get; }
        IObservable<IPopup> OnOpenAsObservable { get; }
        void SetButtonsInteractable(bool value);
        public enum State
        {
            Idle, Opening, Closing
        }
    }

    public interface IPopupManager
    {
        List<Popup> Popups { get; }
        RectTransform Container { get; }

        void Close(IPopup popup);
        IObservable<IPopup> Show<T>() where T : IPopup;
        bool IsOpen<T>() where T : IPopup;
        IObservable<T> GetPopup<T>() where T : IPopup;
    }
}
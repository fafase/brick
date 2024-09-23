using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace Tools
{
    public interface IPopup
    {
        void Init();
        IObservable<Unit> Open();
        IObservable<Unit> Close(bool closeImmediate = false);

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
        IEnumerable<IPopup> Popups { get; }
        RectTransform Container { get; }

        void Close(IPopup popup);
        T Show<T>() where T : IPopup;
        bool IsOpen<T>() where T : IPopup;
        IObservable<T> GetPopup<T>() where T : IPopup;
        int PopupsCount {  get; }
        //IEnumerable<IPopup> PopupList { get; }
        IPopup Clone(IPopup popup);
    }
}
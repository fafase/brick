using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using State = Tools.IPopup.State;

namespace Tools
{
    [RequireComponent(typeof(Animation))]
    public class Popup : View<PopupPresenter>, IPopup, IObservable<IPopup>
    {
        [SerializeField] protected Button m_closeBtn;
        [SerializeField] private AnimationClip m_openAnimation;
        [SerializeField] private AnimationClip m_closeAnimation;

        private Animation m_animation;

        public IObservable<IPopup> OnCloseAsObservable => m_controller.OnCloseAsObservable;
        public IObservable<IPopup> OnOpenAsObservable => m_controller.OnOpenAsObservable;
        public IReadOnlyReactiveProperty<bool> IsOpen => m_controller.IsOpen;
        public IReactiveProperty<State> PopupState => m_controller.PopupState;  

        public virtual void Init(IPopupManager popupManager)
        {           
            transform.SetParent(popupManager.Container, false);
            SetCloseButton();

            m_animation = GetComponent<Animation>();
            m_animation.wrapMode = WrapMode.Once;
            AddAnimation(m_openAnimation);
            AddAnimation(m_closeAnimation);

            m_controller.Init(popupManager, this);
        }

        private void OnDestroy()
        {
            (m_controller as IDisposable)?.Dispose();    
        }

        private void SetCloseButton()
        {
            if (m_closeBtn != null)
            {
                m_closeBtn.onClick.AddListener(() => Close());
            }
        }

        public IObservable<Unit> Open() => 
            Observable.FromCoroutine(() => AnimationSequence(State.Opening));
        

        public IObservable<Unit> Close(bool closeImmediate = false)
        {
            if (closeImmediate) 
            {
                m_controller.ClosePopup(null); // No observable needed for immediate close.
                Destroy(gameObject);
                return Observable.ReturnUnit();
            }

            IObservable<Unit> observable = Observable.FromCoroutine(() => AnimationSequence(State.Closing));
            observable
                .DoOnCompleted(() => Destroy(gameObject))
                .Subscribe()
                .AddTo(this);

            m_controller.ClosePopup(observable);
            return observable;
        }

        private void AddAnimation(AnimationClip clip)
        {
            if (clip == null)
            {
                Debug.LogWarning("Attempted to add a null AnimationClip.");
                return;       
            }
            m_animation.AddClip(clip, clip.name);
        }

        private IEnumerator AnimationSequence(State state)
        {
            PopupState.Value = state;
            SetButtonsInteractable(false);
            AnimationClip currentAnim = state == State.Opening ? m_openAnimation : m_closeAnimation;
            m_animation.clip = currentAnim;
            m_animation.Play();
            if (currentAnim == null)
            {
                Debug.LogWarning("Animation clip missing for " + state);
                yield break;
            }
            while (m_animation.IsPlaying(currentAnim.name))
            {
                yield return null;
            }
            PopupState.Value = State.Idle;
        }

        public void SetButtonsInteractable(bool value) => Array.ForEach(GetComponentsInChildren<Button>(), (btn => btn.interactable = value));

        public IDisposable Subscribe(IObserver<IPopup> observer)
        {
            return m_controller.OnCloseAsObservable.Subscribe(observer);
        }
    }
}

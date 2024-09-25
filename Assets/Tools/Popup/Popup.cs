using System;
using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using State = Tools.IPopup.State;

namespace Tools
{
    [RequireComponent(typeof(Animation))]
    public class Popup : View<PopupPresenter>, IPopup
    {
        [SerializeField] protected Button m_closeBtn;
        [SerializeField] protected Button m_primaryAction;
        [SerializeField] private AnimationClip m_openAnimation;
        [SerializeField] private AnimationClip m_closeAnimation;
        
        [Inject] protected IPopupManager m_popupManager;

        private Animation m_animation;

        public IObservable<IPopup> OnCloseAsObservable => m_presenter.OnCloseAsObservable;
        public IObservable<IPopup> OnOpenAsObservable => m_presenter.OnOpenAsObservable;
        public IReadOnlyReactiveProperty<bool> IsOpen => m_presenter.IsOpen;
        public IReactiveProperty<State> PopupState => m_presenter.PopupState;
        public IObservable<Unit> OnPrimaryActionObservable => m_primaryAction.OnClickAsObservable();
        public IObservable<Unit> OnQuitAsObservable => m_closeBtn.OnClickAsObservable();

        public virtual void Init()
        {           
            transform.SetParent(m_popupManager.Container, false);
            SetCloseButton();

            m_animation = GetComponent<Animation>();
            m_animation.wrapMode = WrapMode.Once;
            AddAnimation(m_openAnimation);
            AddAnimation(m_closeAnimation);

            m_presenter.Init(m_popupManager, this);
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
                m_presenter.ClosePopup(null); // No observable needed for immediate close.
                Destroy(gameObject);
                return Observable.ReturnUnit();
            }

            IObservable<Unit> observable = Observable.FromCoroutine(() => AnimationSequence(State.Closing));
            observable
                .DoOnCompleted(() => Destroy(gameObject))
                .Subscribe()
                .AddTo(this);

            m_presenter.ClosePopup(observable);
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

        public class Factory : PlaceholderFactory<Popup, Popup> { }
    }
}

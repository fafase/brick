using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using UniRx;
using UnityEngine;

namespace Tools
{
    public class PopupManager : MonoBehaviour, IPopupManager, IObservable<ObservableCollection<IPopup>>
    {
        [SerializeField] private List<Popup> m_popups = new List<Popup>();
        public List<Popup> Popups => m_popups;
        public bool ShouldWaitForCompletion => false;

        private bool m_isInit = false;
        public bool IsInit => m_isInit;
        public RectTransform Container => transform as RectTransform;

        public ObservableCollection<IPopup> OpenPopups => m_openPopups;
        public IReactiveProperty<int> PopupsCount { get; private set; }

        private IDictionary<Type, Popup> m_prefabs = new Dictionary<Type, Popup>();

        private ObservableCollection<IPopup> m_openPopups = new ObservableCollection<IPopup>(); 

        private void Awake()
        {
            m_prefabs = m_popups.ToDictionary(p => p.GetType(), p => p);

            PopupsCount = new ReactiveProperty<int>(m_openPopups.Count);
            m_isInit = true;
        }

        public IObservable<IPopup> Show<T>() where T : IPopup
        {
            if (m_prefabs.TryGetValue(typeof(T), out Popup popup))
            {
                Popup instance = Instantiate(popup);
                m_openPopups.Add(instance);
                instance.Init(this);
                return instance;
            }
            Debug.LogError("[PopupManager] Popup was not found in manager list");
            return null;
        }

        public void Close(IPopup popup)
        {
            m_openPopups.Remove(popup);
        }

        public bool IsOpen<T>() where T : IPopup => GetPopup<T>() != null;

        public IObservable<T> GetPopup<T>() where T : IPopup
        {
            Type t = typeof(T);
            int index = m_openPopups.ToList().FindIndex(popup => popup.GetType().Equals(t));
            if(index <= 0) 
            {
                return null;
            }
            return m_openPopups[index] as IObservable<T>;
        }

        public IDisposable Subscribe(IObserver<ObservableCollection<IPopup>> observer)
        {
           return m_openPopups
                .ObserveEveryValueChanged(popup => popup).
                Subscribe(observer);
        }
    }
}

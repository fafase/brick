using System;
using UniRx;
using UnityEngine;
using Zenject;

namespace Tools
{
    public class Loader : MonoBehaviour, ILoader
    {
        [Inject] private ILevelManager m_levelManager;
        public IObservable<Unit> InitializeAll() 
        {
            var observables = new IObservable<Unit>[]
            {
                m_levelManager.Init<LevelConfig>("level_bundle_1")
            };

            return Observable.WhenAll(observables);
        }
    }
}

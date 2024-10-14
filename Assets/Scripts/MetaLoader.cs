using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using Tools;
using UniRx;
using UnityEngine;

public class MetaLoader : MonoBehaviour
{
    [SerializeField] private List<UnityEngine.Object> m_loaders;

    private async void Start()
    {
        foreach (var loader in m_loaders)
        { 
            if(loader is ILoader l) 
            {
                if (l.WaitForCompletion) 
                {
                    await l.OnMetaLoad();
                }
                else 
                {
                    l.OnMetaLoad().Forget();
                }
            }
        }
    }
}

public interface ILoader 
{
    bool WaitForCompletion { get; }
    UniTask OnMetaLoad();
}

public class MetaLoaderSignal : SignalData{ }

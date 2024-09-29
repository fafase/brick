using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


namespace Tools
{

    public class LevelManager : Presenter, ILevelManager
    {
        private List<ILevelConfig> m_levelConfigs;

        public IObservable<Unit> Init<T>(string label) where T : ILevelConfig
        {
            m_levelConfigs = new List<ILevelConfig>();

            return Observable.Create<Unit>(observer =>
            {
                return LoadLocalLevels(label)
                    .Select(json => ConvertToLevel<T>(json))
                    .Do(levelConfig =>
                    {
                        if (levelConfig != null)
                        {
                            m_levelConfigs.Add(levelConfig);
                        }
                    })
                    .Subscribe(_ => { },
                        observer.OnError,
                        () =>
                        {
                            observer.OnNext(Unit.Default);
                            observer.OnCompleted();
                        }).AddTo(m_compositeDisposable);
            });
        }

        private IObservable<string> LoadLocalLevels(string label)
        {
            return Observable.Create<string>(observer =>
            {
                AsyncOperationHandle<IList<TextAsset>> handle = Addressables.LoadAssetsAsync<TextAsset>(label, null);
                handle.Completed += op =>
                {
                    if (op.Status == AsyncOperationStatus.Succeeded)
                    {
                        foreach (TextAsset jsonData in op.Result)
                        {
                            observer.OnNext(jsonData.text);
                        }
                        observer.OnCompleted();
                    }
                    else
                    {
                        observer.OnError(new System.Exception("Failed to load JSON files."));
                    }
                    Addressables.Release(handle);
                };
                return Disposable.Empty;
            });
        }

        private ILevelConfig ConvertToLevel<T>(string json) where T : ILevelConfig
        {
            if (string.IsNullOrEmpty(json))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<T>(json);
        }
    }

    public interface ILevelManager 
    {
        IObservable<Unit> Init<T>(string label) where T: ILevelConfig;
    }
    public interface ILevelConfig { }
}
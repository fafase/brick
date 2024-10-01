using System;
using System.Collections;
using System.Threading;
using UniRx;
using UnityEngine.SceneManagement;
using UnityEngine;

namespace Tools
{
    public class SceneLoading : IDisposable, ISceneLoading
    {
        private CompositeDisposable m_compositeDisposable;
        private bool m_isDisposed = false;
        private string m_current;
        public SceneLoading()
        {
            m_compositeDisposable = new CompositeDisposable();
            m_current = "Loading";
        }

        public virtual IObservable<float> LoadMeta() => LoadScene("Meta");
  
        public IObservable<float> LoadCore() => LoadScene("Core");
        
        
        public IObservable<float> LoadScene(string scene) 
        {
            var sceneObservable = Load(scene);
            sceneObservable
                .Do(progress => Debug.Log(progress))
                .DoOnCompleted(() => 
                { 
                    string temp = m_current; 
                    m_current = scene;
                    Debug.Log($"Changing scene from {temp} to {scene}");
                    ObservableSignal.Broadcast(new SceneChangeSignal(scene, temp)); 
                })
                .Subscribe()
                .AddTo(m_compositeDisposable);
            return sceneObservable;
        }


        public static IObservable<float>Load(string scene) 
        {
            if (string.IsNullOrEmpty(scene)) 
            {
                throw new ArgumentNullException();
            }
            var asyncOperation = SceneManager.LoadSceneAsync(scene);
            if(asyncOperation == null) 
            {
                throw new Exception();
            }
            return Observable.FromCoroutine<float>((observer, ct) =>
                        RunAsyncOperation(asyncOperation, observer, ct));
        }

        static IEnumerator RunAsyncOperation(UnityEngine.AsyncOperation asyncOperation, IObserver<float> observer, CancellationToken cancellationToken)
        {
            while (!asyncOperation.isDone && !cancellationToken.IsCancellationRequested)
            {
                observer.OnNext(asyncOperation.progress);
                yield return null;
            }
            if (!cancellationToken.IsCancellationRequested)
            {
                observer.OnNext(asyncOperation.progress); 
                observer.OnCompleted();
            }
        }

        public void Dispose()
        {
            if (m_isDisposed) { return; }
            m_compositeDisposable?.Dispose();
        }
    }
    public interface ISceneLoading 
    {
        IObservable<float> LoadCore();
        IObservable<float> LoadMeta();
    }
    public class SceneChangeSignal : SignalData 
    {
        public readonly string NextScene;
        public readonly string PreviousScene;

        public SceneChangeSignal(string nextScene, string previousScene)
        {
            NextScene = nextScene;
            PreviousScene = previousScene;
        }
    }
}

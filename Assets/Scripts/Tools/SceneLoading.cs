using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tools
{
    public class SceneLoading 
    {
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
    }
}

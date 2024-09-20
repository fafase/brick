using Cysharp.Threading.Tasks;
using System;
using Tools;
using UniRx;
using UnityEngine;

public class SplashLoading : MonoBehaviour
{
    void Awake()
    {
        Debug.Log("Awake on Splash");
        ObservableSignal
            .AsObservable<LoginSignalData>()
            .DoOnCompleted(OnCompleted)
            .DoOnError(error => OnError(error))
            .Subscribe()
            .AddTo(this);

    }
    public void OnCompleted()
    {
        var sceneObs = SceneLoading.Load("Meta");
        sceneObs
            .Do(progress => Debug.Log(progress))
            .Subscribe();
    }

    public void OnError(Exception e) { }
}

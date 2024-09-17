using System;
using Tools;
using UniRx;
using UnityEngine;

public class SplashLoading : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        var sceneObs = SceneLoading.Load("Meta");
        sceneObs
            .Do(progress => Debug.Log(progress))
            .Subscribe();
    }
}

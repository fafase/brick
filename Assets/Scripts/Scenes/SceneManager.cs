using Tools;
using UniRx;
using UnityEngine;

public class Scene : MonoBehaviour
{
    public virtual void LoadMeta()
    {
        SceneLoading.Load("Meta")
            .Do(progress => Debug.Log(progress))
            .Subscribe()
            .AddTo(this);
    }
}


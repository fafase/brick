using Tools;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public abstract class CoreBase : MonoBehaviour
{
    [SerializeField] private Button m_backBtn;

    protected virtual void Awake()
    {
        m_backBtn.OnClickAsObservable()
            .Subscribe(_ => LoadMeta())
            .AddTo(this);
    }
    protected virtual void LoadMeta() 
    {
        SceneLoading.Load("Meta")
            .Do(progress => Debug.Log(progress))
            .Subscribe()
            .AddTo(this);
    }
}

using Cysharp.Threading.Tasks;
using Tools;
using UniRx;
using Zenject;

public class LevelStartPopup : Popup
{
    [Inject] private ISceneLoading m_sceneLoading;

    private void Start()
    {
        m_primaryAction.OnClickAsObservable()
            .Subscribe(_ => Load())
            .AddTo(this);    
    }

    private void Load()
    {
        Close()
            .DoOnCompleted(() => m_sceneLoading.LoadCore())
            .Subscribe()
            .AddTo(this);
    }
}

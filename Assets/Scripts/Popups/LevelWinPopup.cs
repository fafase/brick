using Cysharp.Threading.Tasks;
using Tools;
using UniRx;
using Zenject;

public class LevelWinPopup : Popup
{
    [Inject] private ISceneLoading m_coreScene;

    private void Start()
    {
        m_primaryAction.OnClickAsObservable()
            .Subscribe(_ => Load())
            .AddTo(this);
    }
    private void Load()
    {
        Close()
            .DoOnCompleted(()=> m_coreScene.LoadMeta())
            .Subscribe()
            .AddTo(this);
    }
}

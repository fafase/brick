using Tools;
using UniRx;
using Zenject;

public class LevelLossPopup : Popup 
{
    [Inject] private ISceneLoading m_sceneLoading;

    private void Start()
    {
        OnQuitAsObservable
             .SelectMany(_ => OnCloseAsObservable)
             .Subscribe(_ => m_sceneLoading.LoadMeta())
             .AddTo(this);

        OnPrimaryActionObservable
            .SelectMany(_ =>
            {
                Close();
                return OnCloseAsObservable;
            })
            .Subscribe(_ => m_sceneLoading.LoadCore())
            .AddTo(this);
    }
}

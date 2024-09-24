using Tools;
using UniRx;
using Zenject;

public class PlayLevelPopup : Popup
{
    [Inject] private ISceneLoading m_sceneLoading;
    [Inject] private ILife m_life;

    private void Start()
    {
        OnOpenAsObservable
            .Subscribe(_ => 
            {
                if (m_life.Lives <= 0)
                {
                    m_primaryAction.interactable = false;
                    m_life
                        .LivesAsObservable
                        .Where(lives => lives > 0)
                        .Subscribe(_ => m_primaryAction.interactable = true)
                        .AddTo(this);

                    return;
                }
                BindPrimaryButton();
            })
            .AddTo(this);    
    }

    private void BindPrimaryButton() 
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

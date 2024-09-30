using Tools;
using UniRx;


public class Player : Presenter, IPlayer
{
    private int m_level;
    public int Level => m_level;

    public Player()
    {
        m_level = 1;
        ObservableSignal
            .AsObservable<EndLevelSignal>()
            .Where(data => data.IsWinning)
            .Subscribe(_ => IncreaseLevel())
            .AddTo(m_compositeDisposable);
    }

    public void IncreaseLevel () => m_level++;
    public void SetLevel(int level) => m_level = level; 
}


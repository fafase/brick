using UniRx;
using Tools;

public class GamePresenter : Presenter
{
    public IReactiveProperty<int> BallAmount { get; private set; }
    public IReactiveProperty<int> Score = new ReactiveProperty<int>();

    public GamePresenter()
    {
        BallAmount = new ReactiveProperty<int>();
    }
    public void Init(int ballAmount)
    {
        BallAmount.Value = ballAmount > 0 ? ballAmount : 1;
    }

    public void DecreaseBallAmount()
    {
        BallAmount.Value--;
    }

    public void AddScore(int score) => Score.Value += score;

    public override void Dispose()
    {
        if (m_isDisposed) return;
        base.Dispose();
        (BallAmount as ReactiveProperty<int>)?.Dispose();
        (Score as ReactiveProperty<int>)?.Dispose();
    }
}

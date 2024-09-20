using UniRx;

public class GameController
{
    public ReactiveProperty<int> BallAmount { get; private set; }
    public ReactiveProperty<int> Score = new ReactiveProperty<int>();

    public GameController()
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
}

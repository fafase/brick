using UniRx;
using Tools;
using UnityEngine;
using Zenject;
using Cysharp.Threading.Tasks;
using System;

public class GamePresenter : Presenter, IGamePresenter
{
    [Inject] IScoreBooster m_scoreBooster;

    public IReactiveProperty<int> BallAmount { get; private set; }
    public IReactiveProperty<int> Score { get; private set; }

    private int m_defaultScorePerSecond = 10;
    private CoreStateMachine m_coreStateMachine;
    public GameState CurrentGameState => m_coreStateMachine.State.Value;
    public IObservable<GameState> CoreState => m_coreStateMachine.State.AsObservable();

    public GamePresenter()
    {
        BallAmount = new ReactiveProperty<int>();
        Score = new ReactiveProperty<int>();
    }
    public void Init(int ballAmount)
    {
        BallAmount.Value = ballAmount > 0 ? ballAmount : 1;
        m_coreStateMachine = new CoreStateMachine(GameState.Waiting);
    }

    public void DecreaseBallAmount()
    {
        BallAmount.Value--;
    }

    public void AddScore(int score) => Score.Value += score * m_scoreBooster.ProcessMultiplier(); 
    
    public int CalculateEndScore(int timer) => Mathf.Abs(timer) * m_defaultScorePerSecond + Score.Value;

    public void SetGameState(GameState state) 
    {
        m_coreStateMachine.SetNewState(state);
    }

    public override void Dispose()
    {
        if (m_isDisposed) return;
        base.Dispose();
        (BallAmount as ReactiveProperty<int>)?.Dispose();
        (Score as ReactiveProperty<int>)?.Dispose();
    }
}

public interface IGamePresenter 
{
    IReactiveProperty<int> BallAmount { get; }
    IReactiveProperty<int> Score { get; }

    void Init(int ballAmount);
    void AddScore(int score);
    void DecreaseBallAmount();
    int CalculateEndScore(int timer);
    void SetGameState(GameState state);
}

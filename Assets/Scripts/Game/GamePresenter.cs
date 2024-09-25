using System;
using Tools;
using UniRx;
using UnityEngine;
using Zenject;

public class GamePresenter : Presenter, IGamePresenter
{
    [Inject] ILife m_life;
    public IReactiveProperty<int> BallAmount { get; private set; }
    private CoreStateMachine m_coreStateMachine;
    public GameState CurrentGameState => m_coreStateMachine.State.Value;
    public IObservable<GameState> CoreState => m_coreStateMachine.State.AsObservable();

    public GamePresenter()
    {
        BallAmount = new ReactiveProperty<int>();
    }
    public void Init(int ballAmount)
    {
        BallAmount.Value = ballAmount > 0 ? ballAmount : 1;
        m_coreStateMachine = new CoreStateMachine(GameState.Waiting);

        ObservableSignal
            .AsObservable<BallActiveSignal>()
            .Where(data => data.IsActive == false && CurrentGameState == GameState.Play)
            .Subscribe(_ => DecreaseBallAmount())
            .AddTo(m_compositeDisposable);
    }

    public void DecreaseBallAmount()
    {
        BallAmount.Value--;
    }

    public void SetGameState(GameState state) 
    {
        m_coreStateMachine.SetNewState(state);
    }

    public void EndLevel() 
    {
        m_life.LoseLife();
        SetGameState(GameState.Loss);
    }

    public override void Dispose()
    {
        if (m_isDisposed) return;
        base.Dispose();
        (BallAmount as ReactiveProperty<int>)?.Dispose();
    }
}

public interface IGamePresenter 
{
    IReactiveProperty<int> BallAmount { get; }

    void Init(int ballAmount);
    void DecreaseBallAmount();
    void SetGameState(GameState state);
    void EndLevel();

    GameState CurrentGameState {  get; }
}

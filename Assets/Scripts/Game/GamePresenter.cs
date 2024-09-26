using System;
using Tools;
using UniRx;
using Zenject;

public class GamePresenter : Presenter, IGamePresenter
{
    [Inject] ILife m_life;
    private CoreStateMachine m_coreStateMachine;
    public GameState CurrentGameState => m_coreStateMachine.State.Value;
    public IObservable<GameState> CoreState => m_coreStateMachine.State.AsObservable();

    public GamePresenter()
    {
        m_coreStateMachine = new CoreStateMachine(GameState.Start);
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
}

public interface IGamePresenter 
{
    void SetGameState(GameState state);
    void EndLevel();

    GameState CurrentGameState {  get; }
}

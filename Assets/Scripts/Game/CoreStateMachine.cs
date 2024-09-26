using System;
using UnityEngine;
using UniRx;
using Tools;

public class CoreStateMachine : IDisposable
{
    public IReactiveProperty<GameState> State;
    private bool m_isDisposed = false;
    public CoreStateMachine()
    {
        State = new ReactiveProperty<GameState>(GameState.Idle);
    }

    public CoreStateMachine(GameState initialState) 
    {
        State = new ReactiveProperty<GameState>(initialState);
    }

    public void SetNewState(GameState newState) 
    {
        Debug.Log($"Changing state from {State.Value} to {newState}");
        if(newState == State.Value) 
        {
            Debug.LogError($"Attempt to set same GameState {newState}");
            return;
        }

        ObservableSignal.Broadcast(new GameStateData(State.Value, newState));
        State.Value = newState;
    }
    public void Dispose()
    {
        if(m_isDisposed) { return; }
        m_isDisposed = true;
        (State as ReactiveProperty<GameState>)?.Dispose();
    }
}
public class GameStateData : SignalData
{
    public readonly GameState PreviousState;
    public readonly GameState NextState;
    public GameStateData(GameState previous, GameState next)
    {
        PreviousState = previous;
        NextState = next;
    }
}

public enum GameState
{
    Idle, Play, Start, Win, Loss, Pause, Waiting
}

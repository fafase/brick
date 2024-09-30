using System.Collections.Generic;
using System.Linq;
using Tools;
using UniRx;
using UnityEngine;

public class BrickSystem : MonoBehaviour, IBrickSystem
{
    public IReactiveCollection<BrickView> Bricks { get; private set; }
    
    public void InitWithBricks(List<BrickView> list)
    {
        Bricks = list.ToReactiveCollection();
        foreach(var brick in Bricks) 
        {
            brick.Brick.Health
                .Skip(1)
                .Where(health => health <= 0)
                .Subscribe(_ => Bricks.Remove(brick))
                .AddTo(this);  
        }

        Bricks
            .ObserveCountChanged()
            .Where(count => count <= 0)
            .DelayFrame(1)
            .Subscribe(_ => ObservableSignal.Broadcast(new EndLevelSignal(true)))
            .AddTo(this);
    }
}

public interface IBrickSystem 
{
    public IReactiveCollection<BrickView> Bricks { get; }
    void InitWithBricks(List<BrickView> list);
}
public class EndLevelSignal : SignalData 
{
    public readonly bool IsWinning;
    public readonly LossReason Reason;
    public EndLevelSignal(bool isWinning)
    {
        IsWinning = isWinning;
    }

    public EndLevelSignal(bool isWinning, LossReason reason)
    {
        IsWinning = isWinning;
        Reason = reason;    
    }

    public enum LossReason 
    {
        TimeUp, NoBall
    }
}

using UniRx;
using Tools;
using UnityEngine;

public class BrickPresenter : Presenter, IBrick
{
    public IReactiveProperty<int> Health { get; private set; }

    public BrickPresenter()
    {
        Health = new ReactiveProperty<int>();
    }

    public void Init(int health, Vector3 position, BrickType type)
    {
        Health.Value = health > 0 ? health : 1;
        Health
            .Where(x => x <= 0)
            .Subscribe(_ => ObservableSignal.Broadcast(new BrickDestroyedSignal(position, type)))
            .AddTo(m_compositeDisposable);
    }

    public void ApplyDamage(int damage) 
    {
        if(damage <= 0) 
        {
            return;
        }
        Health.Value -= damage;
    }

    public override void Dispose()
    {
        if (m_isDisposed) { return; }
        base.Dispose();
        (Health as ReactiveProperty<int>)?.Dispose();
    }
}
public enum BrickType 
{
    Blue, Orange
}

public class BrickDestroyedSignal : SignalData
{
    public readonly Vector3 Position;
    public readonly BrickType Type;
    public BrickDestroyedSignal(Vector3 pos, BrickType type)
    {
        Position = pos;
        Type = type;
    }
}

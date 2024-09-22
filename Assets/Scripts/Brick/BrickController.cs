using UniRx;
using Tools;

public class BrickController : Presenter, IBrick
{
    public IReactiveProperty<int> Health { get; private set; }

    public BrickController()
    {
        Health = new ReactiveProperty<int>();
    }

    public void Init(int health)
    {
        Health.Value = health > 0 ? health : 1;
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

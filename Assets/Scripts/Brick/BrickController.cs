using UniRx;

public class BrickController :IBrick
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
}

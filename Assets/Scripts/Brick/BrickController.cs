using UniRx;

public class BrickController 
{
    public ReactiveProperty<int> HealthProperty;

    public BrickController()
    {
        HealthProperty = new ReactiveProperty<int>();
    }

    public void Init(int health)
    {
        HealthProperty.Value = health > 0 ? health : 1;
    }

    public void ApplyDamage(int damage) 
    {
        if(damage <= 0) 
        {
            return;
        }
        HealthProperty.Value -= damage;
    }
}

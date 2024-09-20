using UniRx;

public class BrickController 
{
    public ReactiveProperty<int> HealthProperty;
    public void Init(int health)
    {
        HealthProperty = new ReactiveProperty<int>(health > 0 ? health : 1);
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

using System.Linq;
using UniRx;
using UnityEngine;

public class BrickSystem : MonoBehaviour, IBrickSystem
{
    public IReactiveCollection<BrickPresenter> Bricks { get; private set; }
    
    void Start()
    {
        Bricks = GetComponentsInChildren<BrickPresenter>(true).ToReactiveCollection();
        foreach(var brick in Bricks) 
        {
            brick.Brick.Health
                .Skip(1)
                .Where(health => health <= 0)
                .Subscribe(_ => Bricks.Remove(brick))
                .AddTo(this);  
        }
    }
}

public interface IBrickSystem 
{
    public IReactiveCollection<BrickPresenter> Bricks { get; }
}

using System;
using Tools;
using Zenject;

public abstract class CoreBase : Presenter, ICore
{
    [Inject] private BrickView.Factory m_brickFactory;
    [Inject] private BallView.Factory m_ballFactory;

    protected CoreBase()
    {

    }


    public virtual BrickView CreateBrick(BrickView prefab)
    {
        if (prefab == null)
        {
            throw new ArgumentNullException("Null prefab for Brick in CoreBase for Factory");
        }
        return m_brickFactory.Create(prefab);
    }
    public BallView CreateBall(BallView prefab) 
    {
        if (prefab == null)
        {
            throw new ArgumentNullException("Null prefab for Brick in CoreBase for Factory");
        }
        return m_ballFactory.Create(prefab);
    }
}
public interface ICore
{
    BrickView CreateBrick(BrickView prefab);
    BallView CreateBall(BallView prefab);
}


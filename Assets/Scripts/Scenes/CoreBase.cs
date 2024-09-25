using Tools;
using UnityEngine;
using Zenject;

public abstract class CoreBase: ICore
{
    [Inject] private BrickView.Factory m_brickFactory;
    [Inject] private PowerUp.Factory m_powerUpFactory;

    protected CoreBase()
    {
        
    }

    public virtual PowerUp CreatePowerUp(GameObject prefab)
    {
        return m_powerUpFactory.Create(prefab);
    }

    public virtual BrickView CreateBrick(GameObject prefab)
    {
        return m_brickFactory.Create(prefab);
    }
}
public interface ICore
{
    PowerUp CreatePowerUp(GameObject prefab);
    BrickView CreateBrick(GameObject prefab);
}

public class PowerUpFactory : IFactory<Object, PowerUp>
{
    readonly DiContainer m_container;

    public PowerUpFactory(DiContainer container)
    {
        m_container = container;
    }
    public PowerUp Create(Object prefab)
    {
        return m_container.InstantiatePrefabForComponent<PowerUp>(prefab);
    }
}

public class BrickFactory : IFactory<Object, BrickView>
{
    readonly DiContainer m_container;

    public BrickFactory(DiContainer container)
    {
        m_container = container;
    }
    public BrickView Create(Object prefab)
    {
        return m_container.InstantiatePrefabForComponent<BrickView>(prefab);
    }
}

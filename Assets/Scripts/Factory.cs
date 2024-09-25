using Tools;
using UnityEngine;
using Zenject;

public class PopupFactory : IFactory<Popup, Popup>
{
    readonly DiContainer m_container;

    public PopupFactory(DiContainer container)
    {
        m_container = container;
    }
    public Popup Create(Popup prefab)
    {
        return m_container.InstantiatePrefabForComponent<Popup>(prefab);
    }
}
public class PowerUpFactory : IFactory<PowerUp, PowerUp>
{
    readonly DiContainer m_container;

    public PowerUpFactory(DiContainer container)
    {
        m_container = container;
    }
    public PowerUp Create(PowerUp prefab)
    {
        return m_container.InstantiatePrefabForComponent<PowerUp>(prefab);
    }
}

public class BrickFactory : IFactory<BrickView, BrickView>
{
    readonly DiContainer m_container;

    public BrickFactory(DiContainer container)
    {
        m_container = container;
    }
    public BrickView Create(BrickView prefab)
    {
        return m_container.InstantiatePrefabForComponent<BrickView>(prefab);
    }
}
public class BallFactory : IFactory<BallView, BallView>
{
    readonly DiContainer m_container;

    public BallFactory(DiContainer container)
    {
        m_container = container;
    }
    public BallView Create(BallView prefab)
    {
        return m_container.InstantiatePrefabForComponent<BallView>(prefab);
    }
}
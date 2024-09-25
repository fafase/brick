using System;
using Tools;
using UniRx;
using UnityEngine;
using Zenject;

public abstract class CoreBase: Presenter, ICore
{
    [Inject] private BrickView.Factory m_brickFactory;
    [Inject] private PowerUp.Factory m_powerUpFactory;
    [Inject] private IPrefabContainer m_container;

    protected CoreBase()
    {
        ObservableSignal
            .AsObservable<BrickDestroyedSignal>()
            .Subscribe(ProcessBrickDestruction)
            .AddTo(m_compositeDisposable);
    }

    public virtual PowerUp CreatePowerUp(PowerUp prefab)
    {
        if(prefab == null) 
        {
            throw new ArgumentNullException("Null prefab for PowerUp in CoreBase for Factory");
        }
        return m_powerUpFactory.Create(prefab);
    }

    public virtual BrickView CreateBrick(GameObject prefab)
    {
        if (prefab == null)
        {
            throw new ArgumentNullException("Null prefab for Brick in CoreBase for Factory");
        }
        return m_brickFactory.Create(prefab);
    }

    private void ProcessBrickDestruction(BrickDestroyedSignal data) 
    {
        // Basic probability logic,
        // should be reviewed to avoid streak of creation and forced creation if too long without new power-up
        int prob = data.Type switch
        {
            BrickType.Blue => 50,
            BrickType.Orange => 70,
            _ => 0
        };

        if(UnityEngine.Random.Range(0,99) < prob) 
        {
            try
            {
                PowerUp prefab = m_container.GetPowerUp(PowerUpType.ExtraBall);
                PowerUp powerUp = CreatePowerUp(prefab);
                powerUp.transform.position = data.Position;
            }
            catch(System.Exception e) 
            {
                Debug.LogError(e.Message);
            }
        }
    }
}
public interface ICore
{
    PowerUp CreatePowerUp(PowerUp prefab);
    BrickView CreateBrick(GameObject prefab);
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

public class BrickFactory : IFactory<UnityEngine.Object, BrickView>
{
    readonly DiContainer m_container;

    public BrickFactory(DiContainer container)
    {
        m_container = container;
    }
    public BrickView Create(UnityEngine.Object prefab)
    {
        return m_container.InstantiatePrefabForComponent<BrickView>(prefab);
    }
}

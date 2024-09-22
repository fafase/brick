using UnityEngine;
using Zenject;

public class ZenjectCoreContext : MonoInstaller
{
    [SerializeField] private BrickSystem m_brickSystem;
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<BallController>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<BrickSystem>().FromInstance(m_brickSystem);
    }
}

using Tools;
using UnityEngine;
using Zenject;

public class ZenjectCoreContext : MonoInstaller
{
    [SerializeField] private BrickSystem m_brickSystem;
    [SerializeField] private CoreScene m_coreScene;
    public override void InstallBindings()
    {
        Container.BindInterfacesTo<BallController>().FromNew().AsSingle().NonLazy();

        Container.BindInterfacesTo<BrickSystem>().FromInstance(m_brickSystem);
        Container.BindInterfacesTo<CoreScene>().FromInstance(m_coreScene);
    }
}

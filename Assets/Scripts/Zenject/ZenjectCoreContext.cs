using Tools;
using UnityEngine;
using Zenject;

public class ZenjectCoreContext : MonoInstaller
{
    [SerializeField] private BrickSystem m_brickSystem;
    [SerializeField] private CoreScene m_coreScene;

    public override void InstallBindings()
    {
        BindPresenters();
        Container.BindInterfacesTo<BrickSystem>().FromInstance(m_brickSystem);
        Container.BindInterfacesTo<CoreScene>().FromInstance(m_coreScene);
    }

    private void BindPresenters() 
    {
        Container.BindInterfacesAndSelfTo<GamePresenter>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<PaddlePresenter>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ScoreBoosterPresenter>().FromNew().AsSingle().NonLazy();//.FromInstance(m_booster);
    }
}

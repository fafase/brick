using UnityEngine;
using Zenject;

public class ZenjectCoreContext : MonoInstaller
{
    [SerializeField] private BrickSystem m_brickSystem;

    public override void InstallBindings()
    {
        BindPresenters();

        Container.BindInterfacesTo<BrickSystem>().FromInstance(m_brickSystem);
        Container.BindInterfacesTo<CoreScene>().FromNew().AsSingle().NonLazy();

        Container.BindFactory<Object, PowerUp, PowerUp.Factory>().FromFactory<PowerUpFactory>();
        Container.BindFactory<Object, BrickView, BrickView.Factory>().FromFactory<BrickFactory>();
    }

    private void BindPresenters() 
    {
        Container.BindInterfacesAndSelfTo<GamePresenter>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<PaddlePresenter>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ScoreBoosterPresenter>().FromNew().AsSingle().NonLazy();
    }
}

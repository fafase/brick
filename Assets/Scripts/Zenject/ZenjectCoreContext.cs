using Tools;
using UnityEngine;
using Zenject;

public class ZenjectCoreContext : MonoInstaller
{
    [SerializeField] private BrickSystem m_brickSystem;
    [SerializeField] private PrefabContainer m_prefabContainer;

    public override void InstallBindings()
    {
        BindPresenters();

        Container.BindInterfacesTo<BrickSystem>().FromInstance(m_brickSystem);
        Container.BindInterfacesTo<PrefabContainer>().FromScriptableObject(m_prefabContainer).AsSingle().NonLazy();
        Container.BindInterfacesTo<CoreScene>().FromNew().AsSingle().NonLazy();

        Container.BindFactory<PowerUp, PowerUp, PowerUp.Factory>().FromFactory<PowerUpFactory>();
        Container.BindFactory<BrickView, BrickView, BrickView.Factory>().FromFactory<BrickFactory>();
        Container.BindFactory<BallView, BallView, BallView.Factory>().FromFactory<BallFactory>();
    }

    private void BindPresenters() 
    {
        Container.BindInterfacesAndSelfTo<GamePresenter>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesTo<PaddlePresenter>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<ScoreBoosterPresenter>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<PowerUpController>().FromNew().AsSingle().NonLazy();
    }
}

using Tools;
using UnityEngine;
using Zenject;

public class ZenjectProjectContext : MonoInstaller
{

    [SerializeField] private PopupManager m_popupManager;

    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<LifePresenter>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<UserPrefs>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<Player>().FromNew().AsSingle().NonLazy();
        Container.BindInterfacesAndSelfTo<LevelManager>().FromNew().AsSingle().NonLazy();

        Container.BindInterfacesTo<PopupManager>().FromComponentInNewPrefab(m_popupManager).AsSingle().NonLazy();
        Container.BindInterfacesTo<SceneLoading>().AsSingle();

        Container.BindFactory<Popup, Popup, Popup.Factory>().FromFactory<PopupFactory>();
    }
}
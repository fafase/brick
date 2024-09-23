using Tools;
using UnityEngine;
using Zenject;

public class ZenjectProjectContext : MonoInstaller
{

    [SerializeField] private PopupManager m_popupManager;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<PopupManager>().FromComponentInNewPrefab(m_popupManager).AsSingle().NonLazy();
        Container.BindInterfacesTo<SceneLoading>().AsSingle();
        Container.BindFactory< Object, Popup, Popup.Factory>().FromFactory<PopupFactory>();
    }
}
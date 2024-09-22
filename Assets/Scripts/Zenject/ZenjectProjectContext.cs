using Tools;
using UnityEditor.Localization.Editor;
using UnityEngine;
using Zenject;

public class ZenjectProjectContext : MonoInstaller
{

    [SerializeField] private PopupManager m_popupManager;

    public override void InstallBindings()
    {
        Container.BindInterfacesTo<PopupManager>().FromComponentInNewPrefab(m_popupManager).AsSingle().NonLazy();
    }
}
using UnityEngine;
using Zenject;

public class ZenjectMetaContext : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<CoinPresenter>().FromNew().AsSingle().NonLazy();
    }
}

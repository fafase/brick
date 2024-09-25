using Tools;
using UnityEngine;
using Zenject;

public class PopupFactory : IFactory<Object, Popup>
{
    readonly DiContainer m_container;

    public PopupFactory(DiContainer container)
    {
        m_container = container;
    }
    public Popup Create(Object prefab)
    {
        return m_container.InstantiatePrefabForComponent<Popup>(prefab);
    }
}

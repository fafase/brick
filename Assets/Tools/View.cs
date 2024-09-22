using UnityEngine;

namespace Tools
{
    public abstract class View<T> : MonoBehaviour where T : new()
    {
        protected T m_controller;
        protected virtual void Awake()
        {
            m_controller = new T();
        }
    }
}


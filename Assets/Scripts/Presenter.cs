using UnityEngine;

public abstract class Presenter <T> :MonoBehaviour where T: new
{
    protected T m_controller;
    protected virtual void Awake() 
    {
        m_controller = new T();
    }
}


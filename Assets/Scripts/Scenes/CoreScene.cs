using UnityEngine;
using Zenject;

public class CoreScene : CoreBase, ICoreScene
{
    [Inject] private IBrickSystem m_brickSystem;

    [ContextMenu("End Level")]
    public void EndGame()
    {     
        foreach(var brick in m_brickSystem.Bricks)
        {
            GameObject go = brick.gameObject;
            Destroy(go);
        }
        m_brickSystem.Bricks.Clear();
    }
}
public interface ICoreScene 
{
    void LoadMeta();
}

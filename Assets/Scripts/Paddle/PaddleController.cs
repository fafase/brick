using UniRx;
using UnityEngine;

public class PaddleController
{
    private float m_speed;
    private Vector3 m_defaultPosition;

    public ReactiveProperty<Vector3> PaddlePos = new ReactiveProperty<Vector3>();
    public PaddleController(Vector3 defaultPos)
    {
        m_defaultPosition = defaultPos;
    }

    public void ProcessPosition(Vector3 mousePos)
    {
        Debug.Log($"Start {mousePos}");
        mousePos.x = Mathf.Clamp(mousePos.x, 0, Screen.width);
        var xPos = Camera.main.ScreenToWorldPoint(mousePos);
        PaddlePos.Value = new Vector3(xPos.x, m_defaultPosition.y, m_defaultPosition.z);
    }
}

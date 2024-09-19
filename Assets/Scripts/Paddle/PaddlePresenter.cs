using UniRx;
using UnityEngine;

public class PaddleView : MonoBehaviour
{
    [SerializeField] private Transform m_paddleTr;
    private PaddleController m_paddleController;

    private void Start()
    {
        m_paddleController = new(m_paddleTr.position);
        m_paddleController.PaddlePos
            .Skip(1)
            .Subscribe(newPosition =>
            {
                m_paddleTr.position = newPosition;
            })
            .AddTo(this);

         Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButton(0))
            .Select(_ => Input.mousePosition)
            .Subscribe(m_paddleController.ProcessPosition)
            .AddTo(this);
    }
}

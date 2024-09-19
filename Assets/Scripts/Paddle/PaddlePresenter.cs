using UniRx;
using UnityEngine;

public class PaddleView : MonoBehaviour, IPaddleProvider
{
    [SerializeField] private Transform m_paddleTr;
    [SerializeField] private Transform m_deckLeftLimit, m_deckRightLimit;
    [SerializeField] private Transform m_paddleLeftLimit, m_paddleRightLimit;

    private PaddleController m_paddleController;

    public Vector3 XLeft => m_deckLeftLimit.position;
    public Vector3 XRight => m_deckRightLimit.position;
    public float Size => GetComponent<SpriteRenderer>().bounds.size.x;
    public float Scale => transform.localScale.x;

    private void Start()
    {
        SetPaddleController();
        SetUpdate();
    }

    private void SetPaddleController() 
    {
        SpriteRenderer sp = m_paddleTr.GetComponent<SpriteRenderer>();
        m_paddleController = new(this, m_paddleTr.position);
        m_paddleController.PaddlePos
            .Skip(1)
            .Subscribe(newPosition =>
            {
                m_paddleTr.position = newPosition;
            })
            .AddTo(this);
        m_paddleController.PaddleScale
            .Subscribe(newScale =>
            {
                Vector3 scale = m_paddleTr.localScale;
                m_paddleTr.localScale = new Vector3(newScale, scale.y, scale.z);
            });
    }

    private void SetUpdate() 
    {
        var update = Observable.EveryUpdate();
        update
            .Where(_ => Input.GetMouseButton(0))
            .Select(_ => Input.mousePosition)
            .Subscribe(m_paddleController.ProcessPosition)
            .AddTo(this);

        update
            .Where(_ => Input.GetKeyDown(KeyCode.Space))
            .Subscribe(m_paddleController.SetScale);
    }
}

public interface IPaddleProvider 
{
    Vector3 XLeft { get; }
    Vector3 XRight { get; }
    float Size { get; }
    float Scale { get; }
    
}

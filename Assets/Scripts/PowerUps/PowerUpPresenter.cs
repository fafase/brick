using Tools;
using UniRx;
using UnityEngine;

public class PowerUpPresenter : Presenter
{
    private float m_speed;
    private Rigidbody2D m_rigidbody;

    public void Init(float speed, Rigidbody2D rig) 
    {
        m_speed = speed;
        m_rigidbody = rig;

        ObservableSignal.AsObservable<GameStateData>()
            .Where(data => data.NextState.Equals(GameState.Pause) 
                || data.NextState.Equals(GameState.Play)
                || data.NextState.Equals(GameState.Loss))
            .Subscribe(data =>
            {
                switch (data.NextState)
                {
                    case GameState.Loss:
                        ProcessLoss();
                        break;
                    case GameState.Pause:
                        ProcessPause();
                        break;
                    case GameState.Play:
                        ProcessPlay();
                        break;
                }
            })
            .AddTo(m_compositeDisposable);

        ProcessPlay();
    }

    public virtual void ProcessPlay()
    {
        Vector2 downwardVelocity = new Vector2(0, -m_speed);
        m_rigidbody.velocity = downwardVelocity;
    }
    public void ProcessPause() 
    {
        m_rigidbody.velocity = Vector2.zero;
    }

    public void ProcessLoss() 
    {
        Object.Destroy(m_rigidbody.gameObject);
    }
}

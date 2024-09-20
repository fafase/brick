using System;
using TMPro;
using UniRx;
using UnityEngine;

public class GamePresenter : Presenter<GameController>
{
    [SerializeField] private int m_amountBalls = 3;
    [SerializeField] private TextMeshProUGUI m_ballAmount;
    [SerializeField] private TextMeshProUGUI m_scoreTxt;
    [SerializeField] private BallPresenter m_ballPresenter;

    private IDisposable m_restartUpdate;
    
    private void Start()
    {
        BindController();
        BindBall();

        m_restartUpdate = Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ => ResetBall());
    }

    private void BindController() 
    {
        m_controller.Init(m_amountBalls);
        m_controller.BallAmount
               .Subscribe(i => m_ballAmount.text = $"Ball : {i}")
               .AddTo(this);

        m_controller.BallAmount
            .Where(value => value == 0)
            .Subscribe(_ => EndLevel())
            .AddTo(this);

        m_controller.Score
            .Subscribe(score => m_scoreTxt.text = $"Score : {score}");
    }

    private void BindBall() 
    {
        m_ballPresenter.Ball
            .Active
            .Where(value => value == false)
            .Subscribe(_ => m_controller.DecreaseBallAmount())
            .AddTo(this);

        m_ballPresenter.Score
            .Subscribe(m_controller.AddScore)
            .AddTo(this);
    }

    private void ResetBall() 
    {
        m_ballPresenter.ResetBall();
    }

    private void EndLevel() 
    {
        m_restartUpdate.Dispose();
    }
}

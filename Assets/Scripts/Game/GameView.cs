using System;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;
using Tools;

public class GameView : MonoBehaviour
{
    [SerializeField] private int m_amountBalls = 3;
    [SerializeField] private TextMeshProUGUI m_ballAmount;
    [SerializeField] private TextMeshProUGUI m_scoreTxt;
    [SerializeField] private TextMeshProUGUI m_timerTxt;
    [SerializeField] private BallPresenter m_ballPresenter;

    [Inject] private IBrickSystem m_brickSystem;
    [Inject] private IPopupManager m_popupManager;
    [Inject] private IGamePresenter m_presenter;

    private IDisposable m_restartUpdate;
    private IDisposable m_timer;
    private int m_sessionDuration = 30;
    private int m_remainingTime;
    public IReactiveProperty<GameState> GameState { get; private set; }

    private void Start()
    {
        BindController();
        BindBall();
        BindTimer();
        BindBrickSystem();

        GameState = new ReactiveProperty<GameState>(global::GameState.Idle);

        m_restartUpdate = Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ => ResetBall());
    }
    private void BindBrickSystem() 
    {
        m_brickSystem.Bricks
            .ObserveCountChanged()
            .Where(count => count <= 0)
            .DelayFrame(1)
            .Subscribe(_ => WinLevel());
    }
    private void BindTimer() 
    {
        m_timerTxt.text = $"{m_sessionDuration}s";

        m_timer = Observable.Interval(TimeSpan.FromSeconds(1))
            .Select(elapsed => m_remainingTime = m_sessionDuration - (int)elapsed)
            .TakeWhile(remaining => remaining >= 0)
            .DelayFrame(1)
            .Subscribe(remainingTime => m_timerTxt.text = $"{remainingTime}s",
                () =>
                {
                    m_timer.Dispose();
                    m_remainingTime = 0;
                    WinLevel();
                })
            .AddTo(this);
    }

    private void BindController() 
    {
        m_presenter.Init(m_amountBalls);
        m_presenter.BallAmount
               .Subscribe(i => m_ballAmount.text = $"BALL\n{i}")
               .AddTo(this);

        m_presenter.BallAmount
            .Where(value => value == 0)
            .Subscribe(_ => EndLevel())
            .AddTo(this);

        m_presenter.Score
            .Subscribe(score => m_scoreTxt.text = $"SCORE\n{score}");
    }

    private void BindBall() 
    {
        m_ballPresenter.Ball
            .Active
            .Where(value => value == false)
            .Subscribe(_ => m_presenter.DecreaseBallAmount())
            .AddTo(this);

        m_ballPresenter.Score
            .Subscribe(score => 
                {
                    m_presenter.AddScore(score);
                    
                })
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

    private void WinLevel() 
    {
        m_ballPresenter.Ball.Active.Value = false;    
        m_restartUpdate.Dispose();
        m_timer.Dispose ();
        Debug.Log("WINNING");
        int score = m_presenter.CalculateEndScore(m_remainingTime);
        LevelWinPopup popup = m_popupManager.Show<LevelWinPopup>();
        popup.Init(score);
    }
}



using System;
using System.Linq;
using TMPro;
using UniRx;
using UnityEngine;
using Zenject;
using Tools;
using UnityEngine.UI;

public class GameView : MonoBehaviour
{
    [SerializeField] private int m_amountBalls = 3;
    [SerializeField] private TextMeshProUGUI m_ballAmount;
    [SerializeField] private TextMeshProUGUI m_scoreTxt;
    [SerializeField] private TextMeshProUGUI m_timerTxt;
    [SerializeField] private BallView m_ballPresenter;
    [SerializeField] private Button m_quitBtn;

    [Inject] private IBrickSystem m_brickSystem;
    [Inject] private IPopupManager m_popupManager;
    [Inject] private IGamePresenter m_presenter;
    [Inject] private ISceneLoading m_sceneLoading;

    private IDisposable m_timer;
    private int m_sessionDuration = 30;
    private int m_remainingTime;

    private void Start()
    {
        BindController();
        BindTimer();
        BindBrickSystem();

        Observable.EveryUpdate()
            .Where(_ => Input.GetKeyDown(KeyCode.R))
            .Subscribe(_ => ResetBall())
            .AddTo(this);

        Popup startPopup = m_popupManager.Show<LevelStartPopup>();
        startPopup
            .OnCloseAsObservable
            .Subscribe(_ => m_presenter.SetGameState(GameState.Play))
            .AddTo(this);

        m_quitBtn
            .OnClickAsObservable()
            .Subscribe(_ => SetRetry())
            .AddTo(this);
    }

    private void SetRetry() 
    {
        m_presenter.SetGameState(GameState.Pause);
        LevelRetryPopup retryPopup = m_popupManager.Show<LevelRetryPopup>();

        retryPopup
            .OnPrimaryActionObservable
            .SelectMany(_ => 
            {
                retryPopup.Close();
                return retryPopup.OnCloseAsObservable;
            })
            .Subscribe(_ => m_sceneLoading.LoadCore())
            .AddTo(this);

        retryPopup
            .OnContinueAsObservable
            .SelectMany(_ =>  retryPopup.OnCloseAsObservable)
            .Subscribe(_=> m_presenter.SetGameState(GameState.Play))
            .AddTo(this);

        retryPopup
            .OnQuitAsObservable
            .SelectMany (_ => retryPopup.OnCloseAsObservable)
            .Subscribe(_=> m_sceneLoading.LoadMeta())
            .AddTo(this);
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
        IDisposable signalDisposable = null;
        void StartTimer()
        {
            signalDisposable?.Dispose();
            m_timer = Observable.Interval(TimeSpan.FromSeconds(1))
                .Select(elapsed => m_remainingTime = m_sessionDuration - (int)elapsed)
                .TakeWhile(remaining => remaining >= 0)
                .DelayFrame(1)
                .Subscribe(remainingTime => m_timerTxt.text = $"{remainingTime}s",
                () =>
                {
                    m_timer.Dispose();
                    m_remainingTime = 0;
                    EndLevel();
                    //WinLevel();
                })
                .AddTo(this);
        }

        signalDisposable = ObservableSignal
            .AsObservable<GameStateData>()
            .Where(data => data.NextState.Equals(GameState.Play))
            .Subscribe(data => StartTimer())
            .AddTo(this);
    }

    private void BindController() 
    {
        m_presenter.Init(m_amountBalls);
        m_presenter.BallAmount
            .Where(value => value >= 0)
            .Subscribe(i => m_ballAmount.text = $"BALL\n{i}")
            .AddTo(this);

        m_presenter.BallAmount
            .Where(value => value < 0)
            .Subscribe(_ => EndLevel())
            .AddTo(this);

        m_presenter.Score
            .Subscribe(score => m_scoreTxt.text = $"SCORE\n{score}");
    }

    private void ResetBall() 
    {
        m_ballPresenter.ResetBall();
    }

    private void EndLevel()
    {
        m_presenter.EndLevel();
        m_popupManager.Show<LevelLossPopup>();
    }

    private void WinLevel() 
    {
        m_ballPresenter.Ball.Active.Value = false;    
        
        m_timer.Dispose ();
        Debug.Log("WINNING");
        int score = m_presenter.CalculateEndScore(m_remainingTime);
        LevelWinPopup popup = m_popupManager.Show<LevelWinPopup>();
        popup.Init(score);
    }
}



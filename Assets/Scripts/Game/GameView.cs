using System;
using System.Linq;
using TMPro;
using Tools;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_scoreTxt;
    [SerializeField] private Button m_quitBtn;

    [SerializeField] private TimerView m_timerView;
    [SerializeField] private ScoreView m_scoreView;
    [SerializeField] private SwipeDetector m_swipeDetector;

    [Inject] private IPopupManager m_popupManager;
    [Inject] private IGamePresenter m_presenter;
    [Inject] private ISceneLoading m_sceneLoading;

    public BallView Ball { get; set; }

    private void Start()
    {
        BindEndLevel(); 

        Popup startPopup = m_popupManager.Show<LevelStartPopup>();
        startPopup
            .OnCloseAsObservable
            .Subscribe(_ => m_presenter.SetGameState(GameState.Waiting))
            .AddTo(this);

        m_quitBtn
            .OnClickAsObservable()
            .Subscribe(_ => SetRetry())
            .AddTo(this);

        ObservableSignal
            .AsObservable<GameStateData>()
            .Where(data => data.NextState == GameState.Waiting)
            .Subscribe(_=> BindInput())
            .AddTo(this);

        ObservableSignal
            .AsObservable<BallDeathSignal>()
            .Subscribe(_ => BindInput())
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
            .SelectMany(_ => retryPopup.OnCloseAsObservable)
            .Subscribe(_ => m_presenter.SetGameState(GameState.Play))
            .AddTo(this);

        retryPopup
            .OnQuitAsObservable
            .SelectMany(_ => retryPopup.OnCloseAsObservable)
            .Subscribe(_ => m_sceneLoading.LoadMeta())
            .AddTo(this);
    }

    private void BindInput() 
    {
        IDisposable dispose = null;
        dispose = m_swipeDetector
            .SwipeAsObservable
            .Subscribe(swipe => 
            {
                dispose.Dispose();
                ResetBall(swipe); 
            })
            .AddTo(this);
    }

    private void BindEndLevel()
    {
        var observable = ObservableSignal
            .AsObservable<EndLevelSignal>()
            .Subscribe(data =>
            {
                if (data.IsWinning)
                {
                    WinLevel();
                }
                else 
                {
                    LossLevel();
                }
            })
            .AddTo(this);
    }


    private void ResetBall(Vector2 swipe)
    {
        Ball.ResetBall(swipe);
    }

    private void LossLevel()
    {
        m_presenter.EndLevel();
        m_popupManager.Show<LevelLossPopup>();
    }

    private void WinLevel()
    {
        Debug.Log("WINNING");
        int score = m_scoreView.CalculateEndScore(m_timerView.RemainingTime); 
        LevelWinPopup popup = m_popupManager.Show<LevelWinPopup>();
        popup.Init(score);
    }
}



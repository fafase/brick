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

    [Inject] private IPopupManager m_popupManager;
    [Inject] private IGamePresenter m_presenter;
    [Inject] private ISceneLoading m_sceneLoading;

    public BallView Ball { get; set; }

    private void Start()
    {
        BindEndLevel();

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
            .SelectMany(_ => retryPopup.OnCloseAsObservable)
            .Subscribe(_ => m_presenter.SetGameState(GameState.Play))
            .AddTo(this);

        retryPopup
            .OnQuitAsObservable
            .SelectMany(_ => retryPopup.OnCloseAsObservable)
            .Subscribe(_ => m_sceneLoading.LoadMeta())
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


    private void ResetBall()
    {
        Ball.ResetBall();
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



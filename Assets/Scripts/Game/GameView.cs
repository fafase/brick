using System.Linq;
using TMPro;
using Tools;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class GameView : MonoBehaviour
{
    [SerializeField] private int m_amountBalls = 3;
    [SerializeField] private TextMeshProUGUI m_ballAmount;
    [SerializeField] private TextMeshProUGUI m_scoreTxt;
    [SerializeField] private Button m_quitBtn;

    [SerializeField] private TimerView m_timerView;

    [Inject] private IBrickSystem m_brickSystem;
    [Inject] private IPopupManager m_popupManager;
    [Inject] private IGamePresenter m_presenter;
    [Inject] private ISceneLoading m_sceneLoading;


    public BallView Ball { get; set; }

    private void Start()
    {
        BindController();
        BindBrickSystem();

        ObservableSignal
            .AsObservable<EndTimerSignal>()
            .Subscribe(_=>EndLevel());

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
        ObservableSignal
            .AsObservable<WinLevelSignal>()
            .Subscribe(_ => WinLevel())
            .AddTo(this);
        //m_brickSystem.Bricks
        //    .ObserveCountChanged()
        //    .Where(count => count <= 0)
        //    .DelayFrame(1)
        //    .Subscribe(_ => WinLevel());
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
        Ball.ResetBall();
    }

    private void EndLevel()
    {
        m_presenter.EndLevel();
        m_popupManager.Show<LevelLossPopup>();
    }

    private void WinLevel() 
    {
        //Ball.Ball.Active.Value = false;          
        Debug.Log("WINNING");
        int score = m_presenter.CalculateEndScore(m_timerView.RemainingTime);
        LevelWinPopup popup = m_popupManager.Show<LevelWinPopup>();
        popup.Init(score);
    }
}



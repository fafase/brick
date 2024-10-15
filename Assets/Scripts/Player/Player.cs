using Cysharp.Threading.Tasks;
using Tools;
using UniRx;
using Zenject;


public class Player : Presenter, IPlayer, IInitializable
{
    [Inject] private IUserPrefs m_userPrefs;
    [Inject] private ICoin m_coins;


    private int m_level;
    public int Level => m_level;
    private bool m_dirtySave;
    private const string m_levelKey = "level";
    private const string m_coinKey = "coins";

    public void Initialize()
    {
        ObservableSignal
            .AsObservable<EndLevelSignal>()
            .Where(data => data.IsWinning)
            .Subscribe(_ => IncreaseLevel())
            .AddTo(m_compositeDisposable);

        m_userPrefs
            .AsObservable
            .Subscribe(_ => m_dirtySave = true)
            .AddTo(m_compositeDisposable);

        ObservableSignal
            .AsObservable<SceneChangeSignal>()
            .Where(_ => m_dirtySave)
            .Subscribe(_ => SendCloudSave())
            .AddTo(m_compositeDisposable);

        ObservableSignal
            .AsObservable<LoginSignalData>()
            .Subscribe(_ => SetPlayerData())
            .AddTo(m_compositeDisposable);
    }

    public void IncreaseLevel()
    {
        m_level++;
        m_userPrefs.SetValue(m_levelKey, m_level);
    }

    public void SetLevel(int level)
    {
        m_level = level;
        m_userPrefs.SetValue(m_levelKey, m_level);
    }

    private void SendCloudSave()
    {
        CloudSave.SendCloudSave(m_userPrefs.Json)
            .ContinueWith(_=> 
            {
                m_dirtySave = false;
            });
    }

    private void SetPlayerData()
    {
        if (!m_userPrefs.TryGetInt(m_levelKey, out m_level))
        {
            m_level = 1;
        }
    }

    public void AddCoinInventory(int amount) 
    {
        if (amount <= 0) { throw new System.Exception("Attempt to add negative or 0 coin"); }
        m_coins.AddCoins(amount);
        m_userPrefs.SetValue(m_coinKey, m_coins.CoinAmount);
    }
}


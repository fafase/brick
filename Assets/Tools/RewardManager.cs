using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;
using UnityEngine;
using Zenject;
using UniRx;
using System;

[CreateAssetMenu(fileName = "RewardManager", menuName = "Tools/RewardManager")]
public class RewardManager : ScriptableObject, IReward, IMetaLoader, IInitializable
{
    [Inject] private IUserPrefs m_userPrefs;
    [Inject] private IPlayer m_player;

    private CompositeDisposable m_compositeDisposable = new CompositeDisposable();
    private ILevelConfig m_currentLevel;
    private const string s_storageKey = "reward_bundles";

    public bool WaitForCompletion
    {
        get
        {
            if (m_userPrefs.TryGetObject<List<Reward>>(s_storageKey, out var rewards))
            {
                return rewards.Count > 0;
            }
            return false;
        }
    }

    private void OnDestroy()
    {
        m_compositeDisposable?.Dispose();
    }

    public async UniTask OnMetaLoad()
    {
        if (m_userPrefs.TryGetObject(s_storageKey, out List<RewardBundle> rewards))
        {
            if(rewards.Count > 0) 
            {
                foreach(RewardBundle bundle in rewards) 
                {
                    switch (bundle.rewardType)
                    { 
                        case RewardType.Coins:
                            m_player.AddCoinInventory(bundle.amount);
                            break;
                    }
                }
            }
            rewards.Clear();
        }
        await Task.Delay(1);
    }

    public void Initialize()
    {
        Debug.Log("Initialize RewardManager");
        ObservableSignal
            .AsObservable<EndLevelSignal>()
            .Subscribe(OnEndLevel)
            .AddTo(m_compositeDisposable);

        ObservableSignal.AsObservable<LoadLevelSignal>()
            .Subscribe(data => 
            {
                Debug.Log("Get current level config");
                m_currentLevel = data.LvlConfig;
            })
            .AddTo(m_compositeDisposable);
    }

    private void OnEndLevel(EndLevelSignal data) 
    {
        if(m_currentLevel == null) 
        {
            Debug.LogError("Level config was empty in RewardManager");
            return;
        }
        if (data.IsWinning) 
        {
            Debug.Log("Registering rewards");
            m_userPrefs.TryGetObject(s_storageKey, out var rewardBundles, new List<RewardBundle>());
            m_currentLevel.reward.ForEach(r =>
            {
                RewardBundle bundle = new RewardBundle();
                bundle.amount = r.amount;
                bundle.rewardType = r.rewardType;
                rewardBundles.Add(bundle);
            });           
            m_userPrefs.SetValue(s_storageKey, rewardBundles);
        }
        m_currentLevel = null;
    }

    private void RetrieveStorageBundle() 
    {
    }
}

public static class RewardType 
{
    public const string Coins = "coins";
}

[Serializable]
public class Reward
{
    public string rewardType { get; set; }
    public int amount { get; set; }

    public Reward() { }
    public Reward(Reward reward)
    {
        rewardType = reward.rewardType;
        amount = reward.amount;
    }
}
[Serializable]
public class RewardBundle : Reward
{
    public long timestamp;
    public RewardBundle() 
    {
        timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
    }
    public RewardBundle(Reward reward) : base(reward)
    {
        timestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
    }
}

public interface IReward 
{
    
}

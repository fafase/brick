using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading.Tasks;
using Tools;
using UnityEngine;
using Zenject;
using UniRx;

[CreateAssetMenu(fileName = "RewardManager", menuName = "Tools/RewardManager")]
public class RewardManager : ScriptableObject, IReward, ILoader, IInitializable
{   
    public bool WaitForCompletion=> m_rewards.Count > 0; 

    private List<Reward> m_rewards = new List<Reward>();
    private CompositeDisposable m_compositeDisposable = new CompositeDisposable();
    private ILevelConfig m_currentLevel;

    private void OnDestroy()
    {
        m_compositeDisposable?.Dispose();
        OnQuitSavePendingRewards();
        m_rewards.Clear();
    }

    public async UniTask OnMetaLoad()
    {
        m_rewards.ForEach(reward => Debug.Log($"{reward.rewardType} - {reward.amount}"));
        await Task.Delay(1);
        m_rewards.Clear();
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
            m_rewards.AddRange(m_currentLevel.reward);
        }
        m_currentLevel = null;
    }

    private void OnQuitSavePendingRewards() 
    {
        Debug.Log("Saving rewards");
    }
}

public interface IReward 
{
    
}

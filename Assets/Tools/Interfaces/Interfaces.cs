using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UniRx;

namespace Tools
{
    public interface ILoader
    {
        IObservable<Unit> InitializeAll();
    }
    public interface IPlayer
    {
        int Level { get; }

        void IncreaseLevel();
        void SetLevel(int level);
    }

    public interface IMetaLoader
    {
        bool WaitForCompletion { get; }
        UniTask OnMetaLoad();
    }

    public interface ILevelManager
    {
        IObservable<Unit> Init<T>(string label) where T : ILevelConfig;
        ILevelConfig CurrentLevelConfig(int level);
    }

    public interface ILevelConfig
    {
        List<Reward> reward { get; set; }
    }
}
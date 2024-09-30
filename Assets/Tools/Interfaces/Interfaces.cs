using System;
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
}

namespace Tools
{
    public class EndLevelSignal : SignalData
    {
        public readonly bool IsWinning;
        public readonly LossReason Reason;
        public EndLevelSignal(bool isWinning)
        {
            IsWinning = isWinning;
        }

        public EndLevelSignal(bool isWinning, LossReason reason)
        {
            IsWinning = isWinning;
            Reason = reason;
        }

        public enum LossReason
        {
            TimeUp, NoBall
        }
    }
    public class LoadLevelSignal : SignalData
    {
        public readonly LevelConfig LvlConfig;

        public LoadLevelSignal(LevelConfig lvlConfig)
        {
            LvlConfig = lvlConfig;
        }
    }

    public class MetaLoaderSignal : SignalData { }
}
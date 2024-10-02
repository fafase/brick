
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
}
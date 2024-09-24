
namespace Tools
{
    public class TimeConverter
    {
        public static string ConvertSeconds(int totalSeconds)
        {
            if (totalSeconds < 60)
            {
                return $"{totalSeconds}s";
            }
            else if (totalSeconds < 3600)
            {
                int minutes = totalSeconds / 60;
                int seconds = totalSeconds % 60;
                return $"{minutes:D2}:{seconds:D2}";
            }
            else
            {
                int hours = totalSeconds / 3600;
                int minutes = (totalSeconds % 3600) / 60;
                return $"{hours:D2}:{minutes:D2}";
            }
        }
    }
}

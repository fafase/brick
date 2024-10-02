using Tools;

public class AudioSignal : SignalData
{
    public readonly string ClipName;
    public readonly float Volume;

    public AudioSignal(string clipName, float volume = 1f)
    {
        ClipName = clipName;
        Volume = volume;
    }
}
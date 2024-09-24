using Cysharp.Threading.Tasks;
using Tools;
using UniRx;

public class LevelStartPopup : Popup
{
    private void Start()
    {
        m_primaryAction.OnClickAsObservable()
            .Subscribe(_ => Close())
            .AddTo(this);
    }
}

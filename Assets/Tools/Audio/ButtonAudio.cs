using Tools;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonAudio : MonoBehaviour
{
    [SerializeField]
    private ButtonSfx m_audioClip;

    private void Start()
    {
        Button button = GetComponent<Button>();
        button.OnClickAsObservable()
            .Subscribe(_ => ObservableSignal.Broadcast(new AudioSignal(m_audioClip.ToString())))
            .AddTo(this);
    }

    enum ButtonSfx { Button_Click }
}

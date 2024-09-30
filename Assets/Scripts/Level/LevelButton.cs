using TMPro;
using Tools;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

public class LevelButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_levelTxt;
    [SerializeField] private Button m_levelBtn;

    [Inject] private IPlayer m_player;
    [Inject] private IPopupManager m_popupManager;

    private void Start()
    {
        m_levelTxt.text = $"Level {m_player.Level.ToString()}";
        m_levelBtn
            .OnClickAsObservable()
            .Subscribe(_ => OnBtnClicked())
            .AddTo(this);
    }

    private void OnBtnClicked()
    {
        m_levelBtn.interactable = false;
        IPopup startPopup = m_popupManager.Show<PlayLevelPopup>();
        startPopup
            .OnCloseAsObservable
            .Subscribe(_ => m_levelBtn.interactable = true)
            .AddTo(this);
    }
}

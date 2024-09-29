using Tools;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class MetaManager : MonoBehaviour
{
    [SerializeField] private Button m_levelBtn;
    [SerializeField] private Button m_multiBtn;

    [Inject] private IPopupManager m_popupManager;
    
    private void Start()
    {
        RegisterButton(m_levelBtn, "Core");
        RegisterButton(m_multiBtn, "CoreMulti");
    }

    private void RegisterButton(Button button, string scene) 
    {
            button.OnClickAsObservable()
               .Subscribe(_ => OnBtnClicked(scene, button))
               .AddTo(this);
    }

    private void OnBtnClicked(string scene, Button button) 
    {
        button.interactable = false;
        IPopup startPopup = m_popupManager.Show<PlayLevelPopup>();
        startPopup
            .OnCloseAsObservable
            .Subscribe(_ => m_levelBtn.interactable = true)
            .AddTo(this);
    }
}

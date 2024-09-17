using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using Tools;

public class MetaManager : MonoBehaviour
{
    [SerializeField] private Button m_levelBtn;
    [SerializeField] private Button m_multiBtn;

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

    private void OnLevelBtnClicked() 
    {
        m_levelBtn.interactable = false;
        SceneLoading.Load("Core");
    }

    private void OnBtnClicked(string scene, Button button) 
    {
        button.interactable = false;
        SceneLoading.Load(scene);
    }
}

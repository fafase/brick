using System;
using Tools;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
public class LevelRetryPopup : Popup
{
    [SerializeField] private Button m_continueBtn;

    public IObservable<Unit> OnContinueAsObservable => m_continueBtn.OnClickAsObservable();

    void Start()
    {
        m_continueBtn
            .OnClickAsObservable()
            .Subscribe(_ => OnClick())
            .AddTo(this);
    }

    private void OnClick() 
    {
        Close();
    }
}

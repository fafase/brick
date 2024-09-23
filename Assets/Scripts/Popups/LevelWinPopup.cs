using Cysharp.Threading.Tasks;
using Tools;
using UniRx;
using UnityEngine;
using Zenject;
using TMPro;


public class LevelWinPopup : Popup
{
    [SerializeField] private TextMeshProUGUI m_scoreTxt; 
    [Inject] private ISceneLoading m_coreScene;

    private void Start()
    {
        m_primaryAction.OnClickAsObservable()
            .Subscribe(_ => Load())
            .AddTo(this);
    }

    public void Init(int score)
    {
        m_scoreTxt.text = $"You Won!\nYour score : {score}";
    }

    private void Load()
    {
        Close()
            .DoOnCompleted(()=> m_coreScene.LoadMeta())
            .Subscribe()
            .AddTo(this);
    }
}

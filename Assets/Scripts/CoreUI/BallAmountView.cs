using System.Collections;
using System.Collections.Generic;
using Tools;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BallAmountView : MonoBehaviour
{
    [SerializeField] private int m_amountBalls = 3;
    [SerializeField] private GameObject m_ballPrefab;
    [SerializeField] private RectTransform m_container;

    private List<GameObject> m_ballImages;

    private ReactiveProperty<int> m_ballCount = new ReactiveProperty<int>();
    private GameState m_currentGameState;

    private void Start()
    {
        m_ballImages = new List<GameObject>();
        for(int i = 0; i < m_amountBalls; i++) 
        {
            GameObject obj = Instantiate(m_ballPrefab);
            obj.SetActive(true);
            obj.transform.SetParent(m_container, false);
            m_ballImages.Add(obj);
        }

        m_ballCount.Value = m_amountBalls > 0 ? m_amountBalls : 1;
        m_ballCount
            .Subscribe(x => 
            {
                m_ballImages.ForEach(im => im.SetActive(false));
                for(int i = 0; i < x; i++) 
                {
                    m_ballImages[i].SetActive(true);
                }
                if(x == 0) 
                {
                    ObservableSignal.Broadcast(new EndLevelSignal(false, EndLevelSignal.LossReason.NoBall));
                }
            })
            .AddTo(this);

        ObservableSignal
            .AsObservable<GameStateData>()
            .Subscribe(data => m_currentGameState = data.NextState)
            .AddTo(this);

        ObservableSignal
            .AsObservable<BallDeathSignal>()
            .Subscribe(_ => DecreaseBallAmount())
            .AddTo(this);
    }

    public void DecreaseBallAmount()
    {
        m_ballCount.Value--;
    }
}

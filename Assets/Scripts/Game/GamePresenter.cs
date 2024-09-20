using TMPro;
using UniRx;
using UnityEngine;

public class GamePresenter : Presenter<GameController>
{
    [SerializeField] private int m_amountBalls = 3;
    [SerializeField] private TextMeshProUGUI m_ballAmount;

    private void Start()
    {
        m_controller.Init(m_amountBalls);

        m_controller.BallAmount
            .Subscribe(i => m_ballAmount.text = i.ToString())
            .AddTo(this);

        m_controller.BallAmount
            .Where(value => value == 0)
            .Subscribe(_ => EndLevel())
            .AddTo(this);
    }

    private void EndLevel() 
    {
        
    }
}

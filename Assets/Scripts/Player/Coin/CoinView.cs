using TMPro;
using UnityEngine;
using Zenject;
using UniRx;

public class CoinView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_coinTxt;

    [Inject] private ICoin m_coin;

    void Start()
    {
        m_coin
            .CoinsAsObservable
            .Subscribe(value => m_coinTxt.text = value.ToString())
            .AddTo(this);
    }
}

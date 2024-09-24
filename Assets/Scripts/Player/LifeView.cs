using TMPro;
using UniRx;
using UnityEngine;
using Zenject;

public class LifeView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_lifeTxt;
    [SerializeField] private TextMeshProUGUI m_refillTimeTxt;

    [Inject] private ILife m_life;

    private const string FULL = "Full";

    private void Start()
    {
        m_life.CountdownTrackerAsObservable
            .Do(time => 
            {
                if (time < 0) m_refillTimeTxt.text = FULL;
                else
                    m_refillTimeTxt.text = time.ToString(); 
            })
            .Subscribe()
            .AddTo(this);

        m_refillTimeTxt.text = FULL;
        m_life.LivesAsObservable
            .Subscribe(value => m_lifeTxt.text = value.ToString())
            .AddTo(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L)) 
        {
            m_life.LoseLife();
        }
    }
}

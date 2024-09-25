using System.Collections;
using System.Collections.Generic;
using TMPro;
using Tools;
using UniRx;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ScoreView : MonoBehaviour
{
    [Inject] IScoreBooster m_scoreBooster;

    private TextMeshProUGUI m_scoreTxt;
    private int m_defaultScorePerSecond = 10;
    private ReactiveProperty<int> Score;

    private void Start()
    {
        m_scoreTxt = GetComponent<TextMeshProUGUI>();
        Score = new ReactiveProperty<int>();
        Score
            .Subscribe(score => m_scoreTxt.text = $"SCORE\n{score}")
            .AddTo(this);

        ObservableSignal.AsObservable<BallScoreSignal>()
            .Where(data => data.Score > 0)
            .Subscribe(data =>
            {
                AddScore(data.Score);
            })
            .AddTo(this);
    }

    public void AddScore(int score) => Score.Value += score * m_scoreBooster.ProcessMultiplier();
    public int CalculateEndScore(int timer) => Mathf.Abs(timer) * m_defaultScorePerSecond + Score.Value;
}

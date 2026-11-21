using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text totalScoreText;
    [SerializeField] private TMP_Text scorePoolText;

    private ScoreData scoreData;

    void Awake()
    {
        scoreData = Resources.Load<ScoreData>(nameof(ScoreData));

        scoreData.TotalScoreChanged.AddListener(UpdateTotalScoreText);
        UpdateTotalScoreText(scoreData.CurrentScore);

        scoreData.ScorePoolChanged.AddListener(UpdateScorePoolText);
        UpdateScorePoolText(scoreData.ScorePool);
    }

    void UpdateTotalScoreText(int score)
    {
        if (totalScoreText == null) return;

        totalScoreText.text = "Score: " + score.ToString();
    }

    void UpdateScorePoolText(int score)
    {
        if (scorePoolText == null) return;

        if (score <= 0)
        {
            scorePoolText.gameObject.SetActive(false);
        }
        else
        {
            scorePoolText.gameObject.SetActive(true);
            scorePoolText.text = score.ToString();
        }
        
    }
}

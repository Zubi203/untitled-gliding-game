using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text text;

    private ScoreData scoreData;

    void Awake()
    {
        scoreData = Resources.Load<ScoreData>(nameof(ScoreData));
        GetTextMeshProComponent();

        if (text == null) return;

        scoreData.ScoreChanged.AddListener(UpdateScoreText);
        UpdateScoreText(scoreData.CurrentScore);
    }

    void GetTextMeshProComponent()
    {
        if (text != null) return;
        
        if (TryGetComponent<TMP_Text>(out TMP_Text txt))
        {
            text = txt;
        }
    }

    void UpdateScoreText(int score)
    {
        if (text == null) return;

        text.text = "Score: " + score.ToString();
    }
}

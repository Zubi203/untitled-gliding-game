using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Scriptable Objects/ScoreData")]
public class ScoreData : ScriptableObject
{
    public UnityEvent<int> ScoreChanged;
    private int _currentScore;
    public int baseScoreGain = 10;
    public int CurrentScore => _currentScore;


    public void ChangeScore(int amount)
    {
        _currentScore += amount;
        ScoreChanged?.Invoke(_currentScore);
    } 

    public void ResetScore()
    {
        _currentScore = 0;
    } 

}

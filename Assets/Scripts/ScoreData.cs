using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Scriptable Objects/ScoreData")]
public class ScoreData : ScriptableObject
{
    public UnityEvent<int> TotalScoreChanged;
    public UnityEvent<int> ScorePoolChanged;
    private int _currentScore;
    private int _scorePool;
    public int BaseScoreGain = 10;
    public int CurrentScore => _currentScore;
    public int ScorePool => _scorePool;


    public void ChangeTotalScore(int amount)
    {
        _currentScore += amount;
        TotalScoreChanged?.Invoke(_currentScore);
    } 

    public void ResetTotalScore()
    {
        ChangeTotalScore(-_currentScore);
    } 

    public void ChangeScorePool(int amount)
    {
        _scorePool += amount;
        ScorePoolChanged?.Invoke(_scorePool);
    } 

    public void ResetScorePool()
    {
        ChangeScorePool(-_scorePool);
    } 

    public void AddPoolToTotal()
    {
        ChangeTotalScore(_scorePool);
        ResetScorePool();
    }

}

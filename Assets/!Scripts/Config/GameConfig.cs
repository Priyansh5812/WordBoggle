using UnityEngine;

[CreateAssetMenu(fileName = "GameConfig", menuName = "Scriptable Objects/GameConfig")]
public class GameConfig : ScriptableObject
{
    [Header("Timer")]
    public int minutes;
    public int seconds;

    [Header("Score")]
    public int scorePerLetter;
    public int scorePerBonus;
}

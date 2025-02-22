using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int _lives = 3;
    private int _score = 0;
    public static GameManager instance;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;

    public int Lives {  get { return _lives; } set { _lives = value; } }

    private void Awake()
    {
        instance = this;
    }
    
    public void IncrementScore(int amount)
    {
        _score += amount;
        scoreText.text = "Score: " + _score.ToString();
    }

    public void DecreaseLives() {
        Debug.Log("Decrease lives");
        _lives--;
        livesText.text = "Lives: 0" + _lives.ToString();
    }


}

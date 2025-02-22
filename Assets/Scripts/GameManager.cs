using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        _lives--;
        livesText.text = "Lives: 0" + _lives.ToString();
    }

    private void Update()
    {
        // Check if the user is on a non-main scene and presses the Escape key
        if (SceneManager.GetActiveScene().buildIndex != 0 && Input.GetKeyDown(KeyCode.Escape))
        {
            // Load the main scene (assuming the main scene is at build index 0)
            LoadScene(0);
        }
    }


    // General method to load scenes based on build index
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }
}

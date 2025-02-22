using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int _lives = 3;
    private int _score = 0;
    public AudioSource src;
    public static GameManager instance;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;

    // Background music clips
    public AudioClip mainMenuMusic;
    public AudioClip gameplayMusic;

    public int Lives { get { return _lives; } set { _lives = value; } }

    public int Score { get { return _score; } set { _score = value; } }
    public AudioSource AudioSrc { get { return src; } set { src = value; } }

    private void Awake()
    {
        instance = this;
    }

    public void IncrementScore(int amount)
    {
        _score += amount;
        scoreText.text = "Score: " + _score.ToString();
    }

    public void DecreaseLives()
    {
        _lives--;
        livesText.text = "Lives: 0" + _lives.ToString();
    }

    private void Update()
    {
        // Check if the user is on a non-main scene and presses the Escape key
        if (SceneManager.GetActiveScene().buildIndex != 0 && Input.GetKeyDown(KeyCode.Escape))
        {
            LoadScene(0);
        }
    }

    // General method to load scenes based on build index
    public void LoadScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void ExitGame(){
        Application.Quit();
    }

}

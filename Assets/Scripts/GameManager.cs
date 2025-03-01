using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public AudioSource src;
    public static GameManager instance;
    public Sprite spaceBackground;
    public Sprite dragonsBackground;

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI livesText;

    public AudioSource AudioSrc { get { return src; } set { src = value; } }

    private void Awake()
    {
        instance = this;
        if (spaceBackground != null && dragonsBackground != null)
        {
            GetComponentInParent<SpriteRenderer>().sprite = GameState.instance.World == 3 ? dragonsBackground : spaceBackground;
        }
    }

    private void Start()
    {
        scoreText.text = "Score: " + GameState.instance.Score.ToString();
        livesText.text = "Lives: 0" + GameState.instance.Lives.ToString();
        UpdateLivesDisplay();
    }

    public void IncrementScore(int amount)
    {
        GameState.instance.Score += amount;
        scoreText.text = "Score: " + GameState.instance.Score.ToString();
    }

    public void DecreaseLives()
    {
        GameState.instance.Lives--;
        livesText.text = "Lives: 0" + GameState.instance.Lives.ToString();

        UpdateLivesDisplay();
    }

    public void IncreaseLives()
    {
        GameState.instance.Lives++;
        Debug.Log("Increasing lives " + GameState.instance.Lives);
        livesText.text = "Lives: 0" + GameState.instance.Lives.ToString();

        UpdateLivesDisplay();
    }

    private Coroutine flashCoroutine;

    public void UpdateLivesDisplay()
    {
        if (GameState.instance.Lives == 1)
        {
            if (flashCoroutine == null)
            {
                flashCoroutine = StartCoroutine(FlashLivesText());
            }
        }
        else
        {
            if (flashCoroutine != null)
            {
                StopCoroutine(flashCoroutine);
                flashCoroutine = null;
            }

            if (GameState.instance.Lives > 3)
            {
                livesText.color = Color.green;
            }
            else // Covers 2 and 3
            {
                livesText.color = Color.white;
            }
        }
    }

    private IEnumerator FlashLivesText()
    {
        while (GameState.instance.Lives == 1)
        {
            livesText.color = Color.red;
            yield return new WaitForSeconds(0.5f);
            livesText.color = Color.white;
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(GameState.instance.World);
    }

    public void ChangeWorld()
    {
        GameState.instance.ChangeWorld();
        GetComponentInParent<SpriteRenderer>().sprite = GameState.instance.World == 3 ? dragonsBackground : spaceBackground;
    }

    public void ExitGame()
    {
        Application.Quit();
    }

}

using UnityEngine;

public class GameState : MonoBehaviour
{
    public int _lives = 3;
    private int _score = 0;

    public static GameState instance;

    public int Lives { get { return _lives; } set { _lives = value; } }
    public int Score { get { return _score; } set { _score = value; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
using UnityEngine;

public class GameState : MonoBehaviour
{
    public int lives = 3;
    private int _score = 0;
    private int _gameTime = 0;
    private int _world = 1;
    public int _bulletLevel = 0;
    public int _maxBulletLevel = 4;
    private int _powerupCount = 0;
    private float _currentFuel;

    public float _speedUpgradeIncrement = 0.75f;
    public float _damageUpgradeIncrement = 0.5f;

    public static GameState instance;

    public int Lives { get { return lives; } set { lives = value; } }
    public int Score { get { return _score; } set { _score = value; } }
    public int GameTime { get { return _gameTime; } set { _gameTime = value; } }
    public int World { get { return _world; } set { _world = value; } }
    public int BulletLevel { get { return _bulletLevel; } set { _bulletLevel = value; } }
    public int MaxBulletLevel { get { return _maxBulletLevel; } set { _maxBulletLevel = value; } }
    public int PowerupCount { get { return _powerupCount; } set { _powerupCount = value; } }
    public float CurrentFuel { get { return _currentFuel; } set { _currentFuel = value; } }


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

    public void ChangeWorld()
    {
        World = World == 1 ? 3 : 1;
    }

    public void Reset()
    {
        Score = 0;
        Lives = 3;
        _gameTime = 0;
        _bulletLevel = 0;
        _maxBulletLevel = 4;
        _powerupCount = 0;
    }
}
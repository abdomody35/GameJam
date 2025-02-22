using UnityEngine;
using System.Collections;

public class Algorithm : MonoBehaviour
{
    public GameObject[] obstacle; // List of obstacles
    public GameObject meteorWarning;
    private readonly int _initialDelay = 3; // Initial delay before start of the game
    private readonly int _maxObstacles = 6; // Maximum number of different obstacles to unlock
    private readonly int _spawnRangeX = 8; // Range of x values obstacles can spawn at
    private readonly int _spawnRangeY = 4; // Range of y values obstacles can spawn at
    private int _availableObstacles = 1; // Starts with only the first obstacle available
    private float _spawnInterval = 2.5f; // Initial spawn interval
    private int _gameTime = 0; // Tracks elapsed time

    void Start()
    {
        StartCoroutine(Spawner());
        StartCoroutine(DifficultyScaler());
    }

    IEnumerator Spawner()
    {
        yield return new WaitForSeconds(_initialDelay);

        while (true)
        {
            int randomIndex = Random.Range(0, _availableObstacles * 100) % _availableObstacles; // Spawn available obstacles
            Vector2 randomPosition;
            if (randomIndex == 3)
            {
                randomPosition = new(15, Random.Range(-_spawnRangeY, _spawnRangeY));
                GameObject warning = Instantiate(meteorWarning, new(8.25f, randomPosition.y), transform.rotation);
                yield return new WaitForSeconds(2);
                Destroy(warning);
            }
            else
            {
                randomPosition = new(transform.position.x + Random.Range(-_spawnRangeX, _spawnRangeX), transform.position.y);
            }
            Instantiate(obstacle[randomIndex], randomPosition, transform.rotation);
            yield return new WaitForSeconds(_spawnInterval); // Wait before spawning next obstacle
        }
    }

    IEnumerator DifficultyScaler()
    {
        while (true)
        {
            _gameTime++;
            yield return new WaitForSeconds(1);

            float _gameStage = _gameTime / 60 * GameManager.instance.Score * 1.2f;

            if (_gameTime % 10 == 0 && _availableObstacles < _maxObstacles && _gameStage >= Mathf.Pow(2, _availableObstacles - 1) * 60)
            {
                _availableObstacles++; // Unlock a new obstacle
            }

            if (_gameStage >= Mathf.Pow(2, _availableObstacles - 1) * 60)
            {
                _spawnInterval = Mathf.Max(0.3f, _spawnInterval - 0.1f); // Decrease spawn interval (faster spawning, min 0.3s)
            }
        }
    }
}

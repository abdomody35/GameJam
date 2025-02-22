using UnityEngine;
using System.Collections;

public class Algorithm : MonoBehaviour
{
    public GameObject[] obstacle; // List of obstacles

    private readonly int _initialDelay = 5; // Initial delay before start of the game
    private readonly int _maxObstacles = 2; // Maximum number of different obstacles to unlock
    private readonly int _spawnRange = 8; // Range of x values obstacles can spawn at
    private int _availableObstacles = 2; // Starts with only the first obstacle available
    private float _spawnInterval = 3.0f; // Initial spawn interval
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
            Vector2 randomPosition = new(transform.position.x + Random.Range(-_spawnRange, _spawnRange), transform.position.y);
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

            float _gameStage = _gameTime / 60 * PlayerController.playerLevel * 1.2f;

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

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Algorithm : MonoBehaviour
{
    public GameObject[] obstacle; // List of obstacles
    public GameObject meteorWarning;
    private readonly int _initialDelay = 3; // Initial delay before start of the game
    private readonly int _maxObstacles = 7; // Maximum number of different obstacles to unlock
    private readonly int _spawnRangeX = 8; // Range of x values obstacles can spawn at
    private readonly int _spawnRangeY = 4; // Range of y values obstacles can spawn at
    private int _availableObstacles = 1; // Starts with only the first obstacle available
    private float _spawnInterval = 3f; // Initial spawn interval

    void Start()
    {
        StartCoroutine(Spawner());
        StartCoroutine(DifficultyScaler());
    }

    IEnumerator Spawner()
    {
        yield return new WaitForSeconds(_initialDelay);

        List<GameObject> tempObstacles = new();
        GameObject portal = null;
        List<int> previousTimes = new();
        int portalTime = 0;
        List<Vector2> previousPositions = new();
        List<int> previousMetoers = new();

        while (true)
        {
            if (portal != null && GameState.instance.GameTime - portalTime > 3)
            {
                Destroy(portal);
                portal = null;
            }
            while (tempObstacles.Count > 0 && tempObstacles[0] != null && GameState.instance.GameTime - previousTimes[0] > 3)
            {
                Destroy(tempObstacles[0]);
                tempObstacles.RemoveAt(0);
                previousTimes.RemoveAt(0);
            }
            while (previousMetoers.Count > 0 && GameState.instance.GameTime - previousMetoers[0] > 5)
            {
                previousMetoers.RemoveAt(0);
            }
            int randomIndex = Random.Range(0, 100) % _availableObstacles; // Spawn available obstacles
            Vector2 randomPosition;
            if (randomIndex == 3)
            {
                if (previousMetoers.Count == (GameState.instance.World == 3 ? 2 : 3))
                {
                    continue;
                }
                previousMetoers.Add(GameState.instance.GameTime);
                StartCoroutine(Warning(obstacle[3]));
                yield return new WaitForSeconds(_spawnInterval);
                continue;
            }
            else if (randomIndex == 6)
            {
                previousTimes.Add(GameState.instance.GameTime);
                do
                {
                    randomPosition = new(Random.Range(-_spawnRangeX, _spawnRangeX), Random.Range(-_spawnRangeY + 3, _spawnRangeY));
                } while (SameAsPreviousPositions(previousPositions, randomPosition) || NearToPlayer(randomPosition));
                GameObject newObstacle = Instantiate(obstacle[randomIndex], randomPosition, transform.rotation);
                tempObstacles.Add(newObstacle);
            }
            else
            {
                do
                {
                    randomPosition = new(transform.position.x + Random.Range(-_spawnRangeX, _spawnRangeX), transform.position.y);
                } while (SameAsPreviousPositions(previousPositions, randomPosition));
                Instantiate(obstacle[randomIndex], randomPosition, transform.rotation);
                if (previousPositions.Count == 3)
                {
                    previousPositions.RemoveAt(0);
                }
                previousPositions.Add(randomPosition);
            }

            if (Random.Range(1, 100) < 3)
            {
                if (portal != null || GameState.instance.GameTime - portalTime < 10 || GameState.instance.GameTime < 20 || GameState.instance.Score < 300)
                {
                    continue;
                }

                portalTime = GameState.instance.GameTime;
                randomPosition = new(Random.Range(-_spawnRangeX, _spawnRangeX), Random.Range(-_spawnRangeY + 3, _spawnRangeY));
                portal = Instantiate(obstacle[7], randomPosition, transform.rotation);
            }

            yield return new WaitForSeconds(_spawnInterval); // Wait before spawning next obstacle
        }
    }

    IEnumerator DifficultyScaler()
    {
        while (true)
        {
            GameState.instance.GameTime++;
            yield return new WaitForSeconds(1);

            float _gameStage;

            if (GameState.instance.GameTime < 20 && _availableObstacles == 1)
            {
                _gameStage = GameState.instance.GameTime * GameState.instance.Score / 25;
            }
            else
            {
                _gameStage = GameState.instance.GameTime * GameState.instance.Score / 100;
            }

            if ((GameState.instance.GameTime < 30 || GameState.instance.GameTime % 10 == 0) && _availableObstacles < _maxObstacles && _gameStage >= Mathf.Pow(2, _availableObstacles - 1) * 60)
            {
                _availableObstacles++; // Unlock a new obstacle
            }

            if (_gameStage % 10 == 0)
            {
                _spawnInterval = Mathf.Max(0.5f, _spawnInterval - GameState.instance.GameTime < 60 ? 0.05f : 0.075f); // Decrease spawn interval (faster spawning, min 0.5s)
            }
        }
    }

    IEnumerator Warning(GameObject meteor)
    {
        Vector2 randomPosition = new(GameState.instance.World == 1 ? 15 : 28, Random.Range(-_spawnRangeY, _spawnRangeY));
        GameObject warning = Instantiate(meteorWarning, new(8.25f, randomPosition.y), transform.rotation);
        yield return new WaitForSeconds(2);
        Destroy(warning);
        Instantiate(meteor, randomPosition, transform.rotation);
    }

    bool SameAsPreviousPositions(List<Vector2> previousPositions, Vector2 position)
    {
        for (int i = (int)(position.x - 1), range = (int)(position.x + 1); i < range; i++)
        {
            if (previousPositions.Contains(new(i, position.y)))
            {
                return true;
            }
        }

        return false;
    }

    bool NearToPlayer(Vector2 position)
    {
        return false;
    }
}

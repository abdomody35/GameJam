using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public static bool toSpace = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameState.instance.ChangeWorld();
            SceneManager.LoadScene(GameState.instance.World);
        }
    }
}
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public static bool toSpace = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameState.instance.InitialDelay--;
            GameState.instance.InitialDelay = (int)MathF.Max(GameState.instance.InitialDelay, 0);
            GameState.instance.ChangeWorld();
            SceneManager.LoadScene(GameState.instance.World);
        }
    }
}
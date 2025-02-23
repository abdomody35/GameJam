using UnityEngine;

public class Blackhole : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerController>(out var player))
            {
                player.TakeDamage(); 
            }
        }
        else if (other.CompareTag("Bullet"))
        {
            Destroy(other.gameObject);
        }
    }
}
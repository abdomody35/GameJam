using UnityEngine;

public class MagicSpell : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<PlayerController>(out var player))
            {
                player.TakeDamage(); // Assumes your PlayerController has a TakeDamage method
            }
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
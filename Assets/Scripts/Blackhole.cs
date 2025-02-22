using UnityEngine;

public class Blackhole : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.TryGetComponent<EnemyHealth>(out var enemyHealth))
            {
                enemyHealth.TakeDamage(1);
            }
        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
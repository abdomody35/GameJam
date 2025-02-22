using UnityEngine;

public class LaserScript : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;

    void Start()
    {
        // Add a Rigidbody2D component and set collision detection to Continuous
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.linearVelocity = transform.up * speed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroy the bullet if it hits an enemy or goes out of bounds
        //if (collision.CompareTag("Enemy") || collision.CompareTag("Bounds"))
        if (collision.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Destroy the bullet after hitting the enemy
            Destroy(gameObject);
        }

        if (collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage); // Assumes your PlayerController has a TakeDamage method
            }
            Destroy(gameObject);
        }
    }
}

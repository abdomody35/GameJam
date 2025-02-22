using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 10f;
    public int damage = 1;

    void Start()
    {
        // Add a Rigidbody2D component and set collision detection to Continuous
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        if (gameObject.CompareTag("Enemy_Bullet"))
        {
            rb.linearVelocity = new Vector2(0, -speed);
        }
        else
        {
            rb.linearVelocity = transform.up * speed;
        }
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        // Destroy the bullet if it hits an enemy or goes out of bounds
        if ((gameObject.CompareTag("Bullet") && collision.CompareTag("Enemy")))
        {
            EnemyHealth enemyHealth = collision.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
            }

            // Destroy the bullet after hitting the enemy
            Destroy(gameObject);
        }


        if (gameObject.CompareTag("Enemy_Bullet") && collision.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damage); // Assumes your PlayerController has a TakeDamage method
            }
            Destroy(gameObject);
        }

        if (collision.CompareTag("Bottom_Wall") || collision.CompareTag("Bounds"))
        {
            Destroy(gameObject);
        }

    }
}

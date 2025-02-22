using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public GameObject collectible;
    private int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy took damage. Current Health: " + currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bottom_Wall"))
        {
            Destroy(gameObject);
        }
    }

    void Die()
    {
        // Play explosion effects or sound here if needed.
        Destroy(gameObject);

        // there is a chance it spawns a power up that will increase the player level
        int random = Random.Range(1, 100);
        if (random < 101)
        {
            Instantiate(collectible, transform.position, transform.rotation);
        }
    }
}
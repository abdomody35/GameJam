using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
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

    void Die()
    {
        // Play explosion effects or sound here if needed.
        Destroy(gameObject);
    }
}
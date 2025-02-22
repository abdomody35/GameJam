using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public AudioSource src;
    public AudioClip die_sfx;
    public int score = 100;
    public int maxHealth = 3;
    public GameObject collectible;

    public GameObject bullet;
    private int currentHealth;

    private float _spawnInterval = 2f; // Initial spawn interval

    void Start()
    {
        currentHealth = maxHealth;

        StartCoroutine(Attack());
    }

    IEnumerator Attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnInterval);

            Instantiate(bullet, transform.position, transform.rotation);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Enemy took damage. Current Health: " + currentHealth);
        
        if (currentHealth <= 0)
        {
            Die();
            GameManager.instance.IncrementScore(score);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Bottom_Wall"))
        {
            Destroy(gameObject);
        }

        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(1); // Assumes your PlayerController has a TakeDamage method
            }
            Destroy(gameObject);
        }
    }

    void Die()
    {
        // Play explosion effects or sound here if needed.
        src.clip = die_sfx;
        src.Play();
        Destroy(gameObject);

        // there is a chance it spawns a power up that will increase the player level
        int random = Random.Range(1, 100);
        if (random < 6)
        {
            Instantiate(collectible, transform.position, transform.rotation);
        }
    }
}
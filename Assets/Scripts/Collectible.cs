using UnityEngine;

public class Collectible2D : MonoBehaviour
{
    public float rotationSpeed = 0.5f;

    public GameObject onCollectEffect;
    public GameObject onDamageIncrease;
    public GameObject onFireRateIncrease;
    public AudioClip onCollectAudio;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, 0, rotationSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.ApplyPowerup();
            }

            Instantiate(onCollectEffect, transform.position, transform.rotation);
            GameManager.instance.AudioSrc.clip = onCollectAudio;
            GameManager.instance.AudioSrc.Play();
        }
        if (other.CompareTag("Bottom_Wall"))
        {
            Destroy(gameObject);
        }
    }
}

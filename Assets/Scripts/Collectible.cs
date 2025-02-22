using UnityEngine;

public class Collectible2D : MonoBehaviour
{
    public float rotationSpeed = 0.5f;

    public GameObject onCollectEffect;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed, 0);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayerController.playerLevel += 0.1f;
            Instantiate(onCollectEffect, transform.position, transform.rotation);
        }
    }
}

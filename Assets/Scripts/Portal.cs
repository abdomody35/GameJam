using UnityEngine;

public class Portal : MonoBehaviour
{
    public static bool toSpace = false;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (other.GetComponent<Dragon>() != null)
            {
                GameManager.instance.LoadScene(1);
            }
            else
            {
                GameManager.instance.LoadScene(3);
            }
        }
    }
}
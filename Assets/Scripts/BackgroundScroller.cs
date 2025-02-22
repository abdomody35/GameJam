using TMPro;
using UnityEngine;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private new Renderer renderer;
    private float speed = 0.2f;

    // Update is called once per frame
    void Update()
    {
        renderer.material.mainTextureOffset += new Vector2(0,speed * Time.deltaTime);
    }
}

using UnityEngine;
using UnityEngine.UI;

public class Scroller : MonoBehaviour
{
    [SerializeField] private Renderer renderer;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        renderer.material.mainTextureOffset += new Vector2(0, speed * Time.deltaTime);
    }
}

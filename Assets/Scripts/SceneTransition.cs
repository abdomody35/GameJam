using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition instance;  // Singleton instance
    public Animator fadeAnimator;            // Assign your fade animator in the Inspector
    public float fadeDuration = 1f;          // Duration of each fade animation

    void Awake()
    {
        // Singleton pattern with persistence
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Public method to trigger the scene transition
    public void TransitionToScene(int sceneIndex)
    {
        StartCoroutine(DoTransition(sceneIndex));
    }

    private IEnumerator DoTransition(int sceneIndex)
    {
        if (fadeAnimator != null)
        {
            // Trigger the fade-out animation
            fadeAnimator.SetTrigger("FadeOut");
            // Wait for fade-out to complete
            yield return new WaitForSeconds(fadeDuration);
        }

        // Load the target scene
        SceneManager.LoadScene(sceneIndex);

        // Wait one frame to ensure the scene is loaded (or use LoadSceneAsync for better control)
        yield return null;

        if (fadeAnimator != null)
        {
            // Trigger the fade-in animation
            fadeAnimator.SetTrigger("FadeIn");
            // Optionally wait for fade-in to finish
            yield return new WaitForSeconds(fadeDuration);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ImageFlicker : MonoBehaviour
{
    [Header("Setup")]
    public float flickerInterval; // Time per sprite
    public Image targetImage;         // The UI image to flicker
    public Sprite[] flickerSprites;   // Array of sprites to cycle through

    private int currentIndex = 0;
    private Coroutine flickerRoutine;
    void Start()
    {
        StartFlicker();
    }
    // Call this to start flickering
    public void StartFlicker()
    {
        if (targetImage == null || flickerSprites.Length == 0) return;

        if (flickerRoutine != null)
            StopCoroutine(flickerRoutine);

        flickerRoutine = StartCoroutine(Flicker());
    }

    // Call this to stop flickering
    public void StopFlicker()
    {
        if (flickerRoutine != null)
            StopCoroutine(flickerRoutine);

        flickerRoutine = null;
    }

    private IEnumerator Flicker()
    {
        while (true)
        {
            targetImage.sprite = flickerSprites[currentIndex];
            currentIndex = (currentIndex + 1) % flickerSprites.Length; // loop back
            yield return new WaitForSeconds(flickerInterval);
        }
    }
}

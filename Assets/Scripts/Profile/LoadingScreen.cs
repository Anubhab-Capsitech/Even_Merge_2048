using UnityEngine;
using System.Collections;
using TMPro;

/// <summary>
/// Manages the loading screen that appears while checking Firebase for user profile.
/// Hides menu elements and shows a loading indicator.
/// </summary>
public class LoadingScreen : MonoBehaviour
{
    [Header("Loading Screen UI")]
    [SerializeField] private GameObject loadingScreenPanel; // The main loading screen panel
    [SerializeField] private GameObject loadingIndicator; // Optional: Spinner or animation
    [SerializeField] private GameObject messagePanel; // Panel to show when internet is missing
    //[SerializeField] private UnityEngine.UI.Text loadingText; // The "Loading..." text element
    [SerializeField] private TextMeshProUGUI loadingText; // The "Loading..." text element
    //[SerializeField] private UnityEngine.UI.Text messageText; // Text element for the message
    [SerializeField] private TextMeshProUGUI messageText; // Text element for the message

    [SerializeField] private GameObject loadingCubeRoot;

    private string originalLoadingText;
    
    [Header("Menu Elements to Hide")]
    [Tooltip("All menu UI elements that should be hidden during loading")]
    [SerializeField] private GameObject[] menuElementsToHide;

    [Header("Settings")]
    [SerializeField] private float fadeInDuration = 0.3f;
    [SerializeField] private float fadeOutDuration = 0.3f;

    private CanvasGroup loadingScreenCanvasGroup;
    private bool isShowing = false;

    private void Awake()
    {
        // Get or add CanvasGroup for fade effects
        loadingScreenCanvasGroup = loadingScreenPanel.GetComponent<CanvasGroup>();
        if (loadingScreenCanvasGroup == null)
        {
            loadingScreenCanvasGroup = loadingScreenPanel.AddComponent<CanvasGroup>();
        }

        // Store original loading text if available
        if (loadingText != null)
        {
            originalLoadingText = loadingText.text;
        }

        // Initially hide message panel
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

   
        if (loadingScreenCanvasGroup == null)
            loadingScreenCanvasGroup = loadingScreenPanel.AddComponent<CanvasGroup>();
    }

    private void Start()
    {
        // Initially hide the loading screen only if it Is not already showing (e.g. triggered by another script)
        if (loadingScreenPanel != null && !isShowing)
        {
            loadingScreenPanel.SetActive(false);
        }

        if (loadingCubeRoot != null)
            loadingCubeRoot.SetActive(isShowing);
    }

    /// <summary>
    /// Shows the loading screen and hides menu elements.
    /// </summary>
    public void Show()
    {
        if (isShowing) return;
        isShowing = true;

        // Hide menu elements
        HideMenuElements();

        // Show loading screen
        if (loadingScreenPanel != null)
        {
            loadingScreenPanel.SetActive(true);
            
            // Fade in
            if (fadeInDuration > 0)
            {
                StartCoroutine(FadeIn());
            }
            else
            {
                loadingScreenCanvasGroup.alpha = 1f;
            }
        }

        // Show loading indicator if available
        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(true);
        }

        if (loadingCubeRoot != null)
            loadingCubeRoot.SetActive(true);

        Debug.Log("LoadingScreen: Showing loading screen");
    }

    /// <summary>
    /// Hides the loading screen and shows menu elements.
    /// </summary>
    public void Hide()
    {
        if (!isShowing) return;
        isShowing = false;

        // Fade out and hide
        if (fadeOutDuration > 0 && loadingScreenPanel != null && loadingScreenPanel.activeSelf)
        {
            StartCoroutine(FadeOut());
        }
        else
        {
            if (loadingScreenPanel != null)
            {
                loadingScreenPanel.SetActive(false);
            }
            ShowMenuElements();
        }

        // Hide loading indicator
        if (loadingIndicator != null)
        {
            loadingIndicator.SetActive(false);
        }

        // Hide message panel
        if (messagePanel != null)
        {
            messagePanel.SetActive(false);
        }

        if (loadingCubeRoot != null)
            loadingCubeRoot.SetActive(false);

        // Reset loading text
        ResetLoadingText();

        Debug.Log("LoadingScreen: Hiding loading screen");
    }

    /// <summary>
    /// Shows a message on the loading screen.
    /// </summary>
    public void ShowMessage(string message)
    {
        if (messageText != null)
        {
            messageText.text = message;
        }

        if (messagePanel != null)
        {
            messagePanel.SetActive(true);
        }
    }

    /// <summary>
    /// Sets the text of the main loading label.
    /// </summary>
    public void SetLoadingText(string text)
    {
        if (loadingText != null)
        {
            loadingText.text = text;
        }
    }

    /// <summary>
    /// Resets the loading text to its original value.
    /// </summary>
    public void ResetLoadingText()
    {
        if (loadingText != null && !string.IsNullOrEmpty(originalLoadingText))
        {
            loadingText.text = originalLoadingText;
        }
    }

    private void HideMenuElements()
    {
        foreach (GameObject element in menuElementsToHide)
        {
            if (element != null)
            {
                element.SetActive(false);
            }
        }
    }

    private void ShowMenuElements()
    {
        foreach (GameObject element in menuElementsToHide)
        {
            if (element != null)
            {
                element.SetActive(true);
            }
        }
    }

    private IEnumerator FadeIn()
    {
        loadingScreenCanvasGroup.alpha = 0f;
        float elapsed = 0f;

        while (elapsed < fadeInDuration)
        {
            elapsed += Time.deltaTime;
            loadingScreenCanvasGroup.alpha = Mathf.Clamp01(elapsed / fadeInDuration);
            yield return null;
        }

        loadingScreenCanvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;
        float startAlpha = loadingScreenCanvasGroup.alpha;

        while (elapsed < fadeOutDuration)
        {
            elapsed += Time.deltaTime;
            loadingScreenCanvasGroup.alpha = Mathf.Lerp(startAlpha, 0f, elapsed / fadeOutDuration);
            yield return null;
        }

        loadingScreenCanvasGroup.alpha = 0f;
        loadingScreenPanel.SetActive(false);
        ShowMenuElements();
    }

    /// <summary>
    /// Checks if loading screen is currently showing.
    /// </summary>
    public bool IsShowing()
    {
        return isShowing;
    }
}


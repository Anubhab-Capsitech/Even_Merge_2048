using UnityEngine;

public class MainMenuProfileChecker : MonoBehaviour
{
    [Header("Language Selection")]
    [SerializeField] private GameObject languageSelectionPrefab;

    [Header("Profile Setup")]
    [SerializeField] private GameObject profileSetupPrefab;
    [SerializeField] private Transform parentObject;

    [Header("Loading Screen")]
    [SerializeField] private LoadingScreen loadingScreen;

    void Start()
    {
        // Show loading screen immediately when scene starts
        if (loadingScreen != null)
        {
            loadingScreen.Show();
        }

        // Start checking for internet before proceeding to profile/language checks
        StartCoroutine(WaitForInternetAndProceed());
    }

    private System.Collections.IEnumerator WaitForInternetAndProceed()
    {
        Debug.Log("[MainMenuProfileChecker] Checking for internet connection...");
        
        bool messageShown = false;

        // Wait until internet connectivity is established
        // NetworkReachability.NotReachable means no network connection
        while (Application.internetReachability == NetworkReachability.NotReachable)
        {
            if (!messageShown && loadingScreen != null)
            {
                // Show localized message if possible, or fallback to English
                string message = "Please turn on your internet connection to play.";
                
                // Try to get localized text if the ID exists in Language.json
                // For now, using a clear English message as requested.
                loadingScreen.ShowMessage(message);
                
                // Also update the main loading text to something like "Offline"
                loadingScreen.SetLoadingText("Offline");
                
                messageShown = true;
            }

            // Just stay on the loading screen
            yield return new WaitForSeconds(1.0f);
        }

        Debug.Log("[MainMenuProfileChecker] Internet connection detected. Proceeding with startup checks.");
        
        // Ensure Firebase is initialized (it might have failed at Start if there was no internet)
        if (FirebaseManager.Instance != null && !FirebaseManager.Instance.IsFirebaseReady())
        {
            Debug.Log("[MainMenuProfileChecker] Re-initializing Firebase...");
            FirebaseManager.Instance.InitializeFirebase();
        }

        // Wait a few seconds for Firebase to become ready if needed
        float firebaseTimeout = 10f; // Give it 10 seconds to connect
        while (FirebaseManager.Instance != null && !FirebaseManager.Instance.IsFirebaseReady() && firebaseTimeout > 0)
        {
            firebaseTimeout -= 0.5f;
            yield return new WaitForSeconds(0.5f);
        }

        // Reset loading screen UI if we showed a message
        if (messageShown && loadingScreen != null)
        {
            loadingScreen.ResetLoadingText();
        }

        // Start the sequential check (Language -> Profile -> Menu)
        CheckNextStep();
    }

    private void CheckNextStep()
    {
        // STEP 1: Check Language Configuration
        if (PlayerPrefs.GetInt("LanguageConfigured", 0) == 0 && languageSelectionPrefab != null)
        {
            Debug.Log("[MainMenuProfileChecker] Language not configured. Showing language selection.");
            
            // Hide loading screen so user can see the English/Portuguese/Indonesian buttons
            if (loadingScreen != null) loadingScreen.Hide();

            GameObject langPopup = Instantiate(languageSelectionPrefab, parentObject);
            StartingLanguageSelectionController controller = langPopup.GetComponent<StartingLanguageSelectionController>();
            
            if (controller != null)
            {
                controller.OnComplete = () => {
                    // Re-show loading screen while initializing next step (Profile check)
                    if (loadingScreen != null) loadingScreen.Show();
                    
                    // When language is done, re-check (will move to profile step)
                    CheckNextStep();
                };
            }
            return;
        }

        // STEP 2: Check Profile Creation
        if (PlayerPrefs.GetInt("ProfileCreated", 0) == 0 && profileSetupPrefab != null)
        {
            Debug.Log("[MainMenuProfileChecker] Profile not found. Showing profile setup.");
            
            if (FindObjectOfType<ProfileSetupPopup>() == null)
            {
                GameObject profilePopup = Instantiate(profileSetupPrefab, parentObject);
                ProfileSetupPopup popupScript = profilePopup.GetComponent<ProfileSetupPopup>();
                
                if (popupScript != null && loadingScreen != null)
                {
                    popupScript.SetLoadingScreen(loadingScreen);
                }
            }
        }
        else
        {
            // STEP 3: Fallback / Profile already exists
            Debug.Log("[MainMenuProfileChecker] Startup checks complete.");
            StartCoroutine(HideLoadingScreenAfterDelay());
        }
    }

    private System.Collections.IEnumerator HideLoadingScreenAfterDelay()
    {
        // Wait a moment to ensure everything is initialized
        yield return new WaitForSeconds(0.8f);
        
        if (loadingScreen != null)
        {
            loadingScreen.Hide();
        }
    }
}

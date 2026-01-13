using UnityEngine;

/// <summary>
/// Ensures FirebaseManager is initialized early in the game.
/// Attach this to a GameObject in the StartingLanguageSelection scene (or first scene).
/// </summary>
public class FirebaseInitializer : MonoBehaviour
{
    [Header("Firebase Manager Prefab")]
    [Tooltip("Drag the FirebaseManager prefab here. If null, will try to find existing instance.")]
    [SerializeField] private GameObject firebaseManagerPrefab;

    private void Awake()
    {
        // If FirebaseManager already exists, don't create another one
        if (FirebaseManager.Instance != null)
        {
            Debug.Log("FirebaseInitializer: FirebaseManager already exists.");
            return;
        }

        // Try to instantiate from prefab if provided
        if (firebaseManagerPrefab != null)
        {
            Instantiate(firebaseManagerPrefab);
            Debug.Log("FirebaseInitializer: Created FirebaseManager from prefab.");
        }
        else
        {
            // Create a GameObject with FirebaseManager component
            GameObject firebaseGO = new GameObject("FirebaseManager");
            firebaseGO.AddComponent<FirebaseManager>();
            Debug.Log("FirebaseInitializer: Created FirebaseManager GameObject.");
        }
    }
}


using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts.GameManager;

/// <summary>
/// Button component that opens the profile stats popup when clicked.
/// Attach this to a button in the main menu.
/// </summary>
public class ProfileButton : MonoBehaviour
{
    [Header("Profile Popup")]
    [Tooltip("Prefab of the ProfileStatsPopup. Should be in Resources/Prefabs/")]
    [SerializeField] private string profilePopupPrefabPath = "Prefabs/ProfileStatsPopup";

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
    }

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnProfileButtonClicked);
        }
    }

    private void OnProfileButtonClicked()
    {
        if (PersistentAudioManager.Instance != null)
        {
            PersistentAudioManager.Instance.PlayClickSound();
        }

        // Load and show profile popup
        GameObject popupPrefab = Resources.Load<GameObject>(profilePopupPrefabPath);
        
        if (popupPrefab != null)
        {
            GameObject popup = Instantiate(popupPrefab);
            DialogManager.GetInstance().show(popup, false);
            Debug.Log("ProfileButton: Opened profile stats popup");
        }
        else
        {
            Debug.LogError($"ProfileButton: Could not find prefab at path: {profilePopupPrefabPath}");
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnProfileButtonClicked);
        }
    }
}


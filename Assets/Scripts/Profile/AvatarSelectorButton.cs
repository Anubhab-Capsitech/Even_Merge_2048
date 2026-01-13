using Assets.Scripts.GameManager;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component to attach to avatar selection buttons.
/// When clicked, it calls ProfileSetupPopup.OnAvatarSelected with the assigned avatar ID.
/// </summary>
public class AvatarSelectorButton : MonoBehaviour
{
    [Header("Avatar Settings")]
    [SerializeField] private int avatarId = 0;
    [SerializeField] private Image avatarImage;
    [SerializeField] private GameObject selectedIndicator; // Optional highlight when selected
    
    private Button button;
    private IAvatarSelectedHandler handler;
    private bool isSelected = false;

    private void Awake()
    {
        button = GetComponent<Button>();
        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
        }
        
        // Find an IAvatarSelectedHandler in parent hierarchy
        handler = GetComponentInParent<IAvatarSelectedHandler>();
    }

    private void Start()
    {
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
        
        // Set initial state
        UpdateVisualState();
    }

    private void OnButtonClicked()
    {
        PersistentAudioManager.Instance?.PlayClickSound();
        if (handler != null)
        {
            handler.OnAvatarSelected(avatarId);
            SetSelected(true);
        }
        else
        {
            Debug.LogWarning("AvatarSelectorButton: IAvatarSelectedHandler not found in parent hierarchy!");
        }
    }

    /// <summary>
    /// Sets whether this avatar is selected and updates visual state.
    /// Called by ProfileSetupPopup when managing selection state.
    /// </summary>
    public void SetSelected(bool selected)
    {
        isSelected = selected;
        UpdateVisualState();
    }

    /// <summary>
    /// Gets the avatar ID assigned to this button.
    /// </summary>
    public int GetAvatarId()
    {
        return avatarId;
    }

    private void UpdateVisualState()
    {
        if (selectedIndicator != null)
        {
            selectedIndicator.SetActive(isSelected);
        }
        
        // Optional: Change button color or scale when selected
        if (button != null)
        {
            var colors = button.colors;
            colors.normalColor = isSelected ? new Color(0.8f, 0.9f, 1f, 1f) : Color.white;
            button.colors = colors;
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }
}


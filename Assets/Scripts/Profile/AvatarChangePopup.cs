using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Assets.Scripts.GameManager;

/// <summary>
/// Popup for changing the user's avatar icon.
/// </summary>
public class AvatarChangePopup : MonoBehaviour, IAvatarSelectedHandler
{
    [Header("UI References")]
    [SerializeField] private Button saveButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private Transform avatarContainer;
    [SerializeField] private GameObject loadingIndicator;

    private int selectedAvatarId = 0;
    private List<AvatarSelectorButton> avatarButtons = new List<AvatarSelectorButton>();
    private System.Action<int> onAvatarChanged;

    public void Initialize(int currentAvatarId, System.Action<int> callback)
    {
        selectedAvatarId = currentAvatarId;
        onAvatarChanged = callback;

        if (saveButton != null) saveButton.onClick.AddListener(OnSaveClicked);
        if (closeButton != null) closeButton.onClick.AddListener(OnCloseClicked);

        InitializeAvatarSelection();
    }

    private void InitializeAvatarSelection()
    {
        if (avatarContainer == null) return;

        avatarButtons.Clear();
        avatarButtons.AddRange(avatarContainer.GetComponentsInChildren<AvatarSelectorButton>());

        foreach (var btn in avatarButtons)
        {
            btn.SetSelected(btn.GetAvatarId() == selectedAvatarId);
        }
    }

    public void OnAvatarSelected(int avatarId)
    {
        selectedAvatarId = avatarId;
        foreach (var btn in avatarButtons)
        {
            btn.SetSelected(btn.GetAvatarId() == avatarId);
        }
    }

    private void OnSaveClicked()
    {
        if (loadingIndicator != null) loadingIndicator.SetActive(true);
        if (saveButton != null) saveButton.interactable = false;

        FirebaseManager.Instance.UpdateUserAvatarId(selectedAvatarId, (success) =>
        {
            if (loadingIndicator != null) loadingIndicator.SetActive(false);
            if (saveButton != null) saveButton.interactable = true;

            if (success)
            {
                // Update local storage
                PlayerPrefs.SetInt("UserAvatarId", selectedAvatarId);
                PlayerPrefs.Save();

                // Notify UI listeners
                ProfileButtonAvatarUpdater.NotifyAvatarChanged(selectedAvatarId);
                onAvatarChanged?.Invoke(selectedAvatarId);

                ClosePopup();
            }
            else
            {
                Debug.LogError("AvatarChangePopup: Failed to update avatar in Firebase.");
            }
        });
    }

    private void OnCloseClicked()
    {
        ClosePopup();
    }

    private void ClosePopup()
    {
        if (PersistentAudioManager.Instance != null)
        {
            PersistentAudioManager.Instance.PlayClickSound();
        }
        DialogManager.GetInstance().Close(null);
    }
}

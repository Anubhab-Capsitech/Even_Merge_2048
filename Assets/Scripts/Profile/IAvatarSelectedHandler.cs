using UnityEngine;

/// <summary>
/// Interface for objects that handle avatar selection.
/// This allows AvatarSelectorButton to notify any handler (like ProfileSetupPopup or AvatarChangePopup).
/// </summary>
public interface IAvatarSelectedHandler
{
    void OnAvatarSelected(int avatarId);
}

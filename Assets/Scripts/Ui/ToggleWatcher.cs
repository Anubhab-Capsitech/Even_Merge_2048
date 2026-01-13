using UnityEngine;

public class ToggleWatcher : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log($"[ToggleWatcher] Toggle2D3D enabled at {Time.frameCount}. Stack:\n{System.Environment.StackTrace}", this);
    }

    void OnDisable()
    {
        Debug.Log($"[ToggleWatcher] Toggle2D3D disabled at {Time.frameCount}.", this);
    }
}

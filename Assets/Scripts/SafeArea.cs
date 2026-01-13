using UnityEngine;

namespace Capsitech.GameeUtilities
{
    [RequireComponent(typeof(RectTransform))]
    public class SafeArea : MonoBehaviour
    {
        private RectTransform _panel;
        private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
        private Vector2Int _lastScreenSize = new Vector2Int(0, 0);
        private ScreenOrientation _lastOrientation = ScreenOrientation.AutoRotation;

        void Awake()
        {
            _panel = GetComponent<RectTransform>();
            ApplySafeArea();
        }

        void Update()
        {
            // Re-apply if screen size, orientation, or safe area changes
            if (_lastSafeArea != Screen.safeArea
                || _lastScreenSize.x != Screen.width
                || _lastScreenSize.y != Screen.height
                || _lastOrientation != Screen.orientation)
            {
                ApplySafeArea();
            }
        }

        private void ApplySafeArea()
        {
            Rect safeArea = Screen.safeArea;

            // Save state for change detection
            _lastSafeArea = safeArea;
            _lastScreenSize.x = Screen.width;
            _lastScreenSize.y = Screen.height;
            _lastOrientation = Screen.orientation;

            // Normalized anchor values
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;

            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            _panel.anchorMin = anchorMin;
            _panel.anchorMax = anchorMax;

         //   Debug.Log($"[SafeArea] Applied safe area: {safeArea}");
        }
    }
}

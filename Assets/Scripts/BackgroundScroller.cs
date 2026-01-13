using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// A simple script to scroll a UI image infinitely in one direction.
/// Works best when the image is tileable or wide enough to cover the screen twice.
/// </summary>
public class BackgroundScroller : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollSpeed = 50f;      // Speed of the scroll
    public Vector2 scrollDirection = Vector2.left; // Direction to move

    private RectTransform rectTransform;
    private float imageWidth;
    private Vector2 startAnchoredPos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            startAnchoredPos = rectTransform.anchoredPosition;
            imageWidth = rectTransform.rect.width;
        }
    }

    void Update()
    {
        if (rectTransform == null) return;

        // Move the image
        rectTransform.anchoredPosition += scrollDirection * scrollSpeed * Time.deltaTime;

        // Loop logic: if it has moved its full width, reset it
        // This assumes the anchor is set correctly for wrapping
        if (Mathf.Abs(rectTransform.anchoredPosition.x - startAnchoredPos.x) >= imageWidth)
        {
            rectTransform.anchoredPosition = startAnchoredPos;
        }
    }
}

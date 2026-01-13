using UnityEngine;
using System.Collections;

public class RectTransformAutoMove : MonoBehaviour
{
    public RectTransform target;
    public float moveDistance = 5f;   // how far to move (on X)
    public float interval = 0.2f;     // delay before toggling

    private Vector2 originalPos;
    private bool toggled = false;

    private void Awake()
    {
        if (target == null)
            target = GetComponent<RectTransform>();

        originalPos = target.anchoredPosition;
    }

    private void OnEnable()
    {
        StartCoroutine(FlickerLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
        if (target != null)
            target.anchoredPosition = originalPos; // reset when disabled
    }

    private IEnumerator FlickerLoop()
    {
        while (true)
        {
            if (toggled)
                target.anchoredPosition = originalPos;
            else
                target.anchoredPosition = originalPos + Vector2.left * moveDistance;

            toggled = !toggled;

            yield return new WaitForSeconds(interval);
        }
    }
}
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class ToggleSwitch : MonoBehaviour
{
    [Header("Visuals")]
    public RectTransform knob;               // assign: Knob RectTransform
    public Image backgroundImage;            // assign: BG Image
    public Color color2D = new Color32(77, 166, 255, 255);   // Blue (2D)
    public Color color3D = new Color32(240, 90, 126, 255);  // Pink (3D)


    [Header("Motion")]
    public float animDuration = 0.14f;
    public float knobPadding = 10f; // internal padding from background edge

    Toggle t;
    Vector2 leftPos;
    Vector2 rightPos;
    float bgWidth;
    float knobWidth;

    void Awake()
    {
        t = GetComponent<Toggle>();
        if (t == null) return;

        // compute positions based on sizes
        if (backgroundImage != null && knob != null)
        {
            RectTransform bgRect = backgroundImage.rectTransform;
            bgWidth = bgRect.rect.width;
            knobWidth = knob.rect.width;

            float halfBg = bgWidth * 0.5f;
            float halfKnob = knobWidth * 0.5f;
            float xLeft = -halfBg + halfKnob + knobPadding;
            float xRight = halfBg - halfKnob - knobPadding;

            leftPos = new Vector2(xLeft, 0f);
            rightPos = new Vector2(xRight, 0f);
        }

        // initial visual
        ApplyImmediate(t.isOn);

        // subscribe
        t.onValueChanged.AddListener(SetState);
    }

    void OnDestroy()
    {
        if (t != null) t.onValueChanged.RemoveListener(SetState);
    }

    public void SetState(bool on)
    {
        Debug.Log($"[ToggleSwitch] SetState called. on={on}, current BG color={backgroundImage.color}");
        StopAllCoroutines();
        StartCoroutine(AnimateTo(on));
    }

    IEnumerator AnimateTo(bool on)
    {
        // background color
        Color from = backgroundImage != null ? backgroundImage.color : Color.white;
        Color to = on ? color2D  : color3D;

        // knob positions
        Vector2 fromPos = knob != null ? knob.anchoredPosition : Vector2.zero;
        Vector2 toPos = on ? rightPos : leftPos;

        float elapsed = 0f;
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float tAlpha = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);
            if (backgroundImage != null) backgroundImage.color = Color.Lerp(from, to, tAlpha);
            if (knob != null) knob.anchoredPosition = Vector2.Lerp(fromPos, toPos, tAlpha);
            yield return null;
        }

        // final snap
        if (backgroundImage != null) backgroundImage.color = to;
        if (knob != null) knob.anchoredPosition = toPos;
    }

    void ApplyImmediate(bool on)
    {
        if (backgroundImage != null) backgroundImage.color = on ? color2D  : color3D;
        if (knob != null) knob.anchoredPosition = on ? rightPos : leftPos;
    }
}

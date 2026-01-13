using UnityEngine;
using TMPro;
using System.Collections;

public class TMPUnderlaySwitcher : MonoBehaviour
{
    public TMP_Text tmpText;
    private Material tmpMaterial;

    private float offsetA = 0.22f;
    private float offsetB = -0.22f;
    private bool toggle = false;

    private void Awake()
    {
        if (tmpText == null)
            tmpText = GetComponent<TMP_Text>();

        // Create an instance so this text flickers independently
        tmpMaterial = tmpText.fontMaterial;
    }

    private void OnEnable()
    {
        StartCoroutine(SwitchUnderlayOffsetRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator SwitchUnderlayOffsetRoutine()
    {
        while (true)
        {
            float value = toggle ? offsetA : offsetB;
            tmpMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetX, value);

            toggle = !toggle; // flip for next time

            yield return new WaitForSeconds(0.2f); // wait half a second
        }
    }
}

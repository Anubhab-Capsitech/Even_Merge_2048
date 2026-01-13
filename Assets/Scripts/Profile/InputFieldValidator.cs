using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Editor helper script to validate TextMeshPro Input Field setup.
/// Attach this to your UsernameInput GameObject to check if it's configured correctly.
/// </summary>
[RequireComponent(typeof(TMP_InputField))]
public class InputFieldValidator : MonoBehaviour
{
#if UNITY_EDITOR
    [ContextMenu("Validate Input Field Setup")]
    private void ValidateSetup()
    {
        TMP_InputField inputField = GetComponent<TMP_InputField>();
        if (inputField == null)
        {
            Debug.LogError($"[InputFieldValidator] {gameObject.name}: TMP_InputField component not found!");
            return;
        }

        bool isValid = true;
        string issues = "";

        // Text Component
        if (inputField.textComponent == null)
        {
            isValid = false;
            issues += "\n❌ Text Component is not assigned!";
        }

        // Placeholder
        if (inputField.placeholder == null)
        {
            isValid = false;
            issues += "\n❌ Placeholder is not assigned!";
        }

        // Text Area
        Transform textArea = transform.Find("Text Area");
        if (textArea == null)
        {
            isValid = false;
            issues += "\n❌ 'Text Area' child not found!";
        }
        else
        {
            if (textArea.GetComponent<RectMask2D>() == null)
            {
                isValid = false;
                issues += "\n❌ RectMask2D missing on Text Area!";
            }

            Transform textChild = textArea.Find("Text");
            if (textChild == null || textChild.GetComponent<TextMeshProUGUI>() == null)
            {
                isValid = false;
                issues += "\n❌ TextMeshProUGUI missing on Text!";
            }
        }

        // Placeholder object
        Transform placeholder = transform.Find("Placeholder");
        if (placeholder == null || placeholder.GetComponent<TextMeshProUGUI>() == null)
        {
            isValid = false;
            issues += "\n❌ Placeholder TextMeshProUGUI missing!";
        }

        // Image (raycast)
        if (GetComponent<Image>() == null)
        {
            Debug.LogWarning("[InputFieldValidator] ⚠ Image component missing. Input may not be clickable.");
        }

        if (isValid)
        {
            Debug.Log($"[InputFieldValidator] ✅ {gameObject.name} is properly configured!");
        }
        else
        {
            Debug.LogError($"[InputFieldValidator] ❌ Issues found:{issues}");
            EditorUtility.DisplayDialog(
                "Input Field Validation",
                $"Found issues:\n{issues}",
                "OK"
            );
        }
    }

    [ContextMenu("Auto-Fix Common Issues")]
    private void AutoFix()
    {
        TMP_InputField inputField = GetComponent<TMP_InputField>();
        if (inputField == null) return;

        Transform textArea = transform.Find("Text Area");

        // Assign Text Component
        if (inputField.textComponent == null && textArea != null)
        {
            Transform textChild = textArea.Find("Text");
            if (textChild != null)
            {
                TextMeshProUGUI tmp = textChild.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    inputField.textComponent = tmp;
                    tmp.raycastTarget = false;
                }
            }
        }

        // Assign Placeholder
        if (inputField.placeholder == null)
        {
            Transform placeholder = transform.Find("Placeholder");
            if (placeholder != null)
            {
                TextMeshProUGUI tmp = placeholder.GetComponent<TextMeshProUGUI>();
                if (tmp != null)
                {
                    inputField.placeholder = tmp;
                }
            }
        }

        // Add RectMask2D
        if (textArea != null && textArea.GetComponent<RectMask2D>() == null)
        {
            textArea.gameObject.AddComponent<RectMask2D>();
        }

        Debug.Log("[InputFieldValidator] ✅ Auto-fix complete!");
    }
#endif
}

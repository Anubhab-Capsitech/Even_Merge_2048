using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PopapScore : MonoBehaviour
{
    public static PopapScore Instance;

    [Header("Popup Settings")]
    [SerializeField] private TextMeshProUGUI popupPrefab;
    [SerializeField] private Canvas canvas;
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private float moveUpDistance = 50f;
    [SerializeField] private float scaleUp = 1.3f;
    [SerializeField] private bool enablePopupAnimation = true;

    private List<PopupData> activePopups = new List<PopupData>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        UpdateAllPopups();
    }

    public void ShowPopup(int number, Vector3 worldPosition)
    {
        if (!popupPrefab || !canvas)
        {
            Debug.LogWarning("PopapScore: Missing prefab or canvas reference!");
            return;
        }

        if (!gameObject.activeInHierarchy) return;

        Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPosition + Vector3.up * 0.2f);
        TextMeshProUGUI popup = Instantiate(popupPrefab, canvas.transform);
        popup.text = "+" + number.ToString();
        popup.transform.position = screenPos;
        popup.transform.localScale = Vector3.one;
        popup.color = Color.white;
        popup.gameObject.SetActive(true);


        PopupData newPopup = new PopupData
        {
            popupText = popup,
            startPosition = screenPos,
            endPosition = screenPos + Vector3.up * moveUpDistance,
            startScale = Vector3.one,
            targetScale = Vector3.one * scaleUp,
            startColor = Color.white,
            elapsedTime = 0f,
            duration = duration,
            useAnimation = enablePopupAnimation
        };

        activePopups.Add(newPopup);
    }

    private void UpdateAllPopups()
    {
        for (int i = activePopups.Count - 1; i >= 0; i--)
        {
            PopupData popupData = activePopups[i];


            if (popupData.popupText == null || !popupData.popupText.gameObject.activeInHierarchy)
            {
                activePopups.RemoveAt(i);
                continue;
            }

            popupData.elapsedTime += Time.deltaTime;
            float t = popupData.elapsedTime / popupData.duration;

            if (t >= 1f)
            {
            
                if (popupData.popupText != null && popupData.popupText.gameObject != null)
                {
                    Destroy(popupData.popupText.gameObject);
                }
                activePopups.RemoveAt(i);
                continue;
            }

            
            UpdatePopupAnimation(popupData, t);
        }
    }

    private void UpdatePopupAnimation(PopupData popupData, float t)
    {
        if (popupData.useAnimation)
        {
     
            float smoothT = Mathf.SmoothStep(0f, 1f, t);


            popupData.popupText.transform.position = Vector3.Lerp(
                popupData.startPosition,
                popupData.endPosition,
                smoothT
            );

            popupData.popupText.transform.localScale = Vector3.Lerp(
                popupData.startScale,
                popupData.targetScale,
                smoothT
            );


            Color newColor = popupData.startColor;
            newColor.a = Mathf.Lerp(1f, 0f, smoothT);
            popupData.popupText.color = newColor;
        }
        else
        {
     
            Color newColor = popupData.startColor;
            newColor.a = Mathf.Lerp(1f, 0f, t);
            popupData.popupText.color = newColor;
        }
    }

    public void ToggleAnimation(bool enable)
    {
        enablePopupAnimation = enable;


        foreach (var popupData in activePopups)
        {
            if (popupData.popupText != null)
            {
                popupData.useAnimation = enable;
            }
        }
    }

    public void CleanupAllPopups()
    {
     
        for (int i = activePopups.Count - 1; i >= 0; i--)
        {
            if (activePopups[i].popupText != null && activePopups[i].popupText.gameObject != null)
            {
                Destroy(activePopups[i].popupText.gameObject);
            }
        }

   
        activePopups.Clear();

        if (canvas != null)
        {
            TextMeshProUGUI[] allPopups = canvas.GetComponentsInChildren<TextMeshProUGUI>();
            foreach (TextMeshProUGUI popup in allPopups)
            {
                if (popup != null && popup != popupPrefab)
                {
                    Destroy(popup.gameObject);
                }
            }
        }
    }

    private void OnDestroy()
    {
        CleanupAllPopups();
    }


    private class PopupData
    {
        public TextMeshProUGUI popupText;
        public Vector3 startPosition;
        public Vector3 endPosition;
        public Vector3 startScale;
        public Vector3 targetScale;
        public Color startColor;
        public float elapsedTime;
        public float duration;
        public bool useAnimation;
    }
}
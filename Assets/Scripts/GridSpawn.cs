using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridSpawn : MonoBehaviour
{
    [Header("Prefab and Parent Settings")]
    [SerializeField] private GameObject prefab;
    [SerializeField] private Transform parent;
    [SerializeField] private int spawnCount = 5;

    [Header("Scene Objects")]
    public GameObject wallObject;
    public GameObject groundObject;

    [Header("UI Elements")]
    public Image[] implementRealscorePanal;

    [Header("Theme Data")]
    [SerializeField] private ContanerData contanerData;
    [SerializeField] private Image background;

    [SerializeField] private int forcedThemeIndex = 0;

    [Header("Fixed Theme (No Theme System)")]
    [SerializeField] private Sprite fixedBackground;
    [SerializeField] private Sprite fixedContainerSprite;
    [SerializeField] private Sprite fixedSettingsIcon;
    //[SerializeField] private Sprite fixedScoreIcon;



    private const string SelectedIndexKey = "SelectedThemeIndex";

    private List<GameObject> spawnedButtons = new List<GameObject>();
    private int currentSelectedIndex = -1;

    private MeshRenderer wallMat;
    private MeshRenderer groundMat;

    private void Start()
    {
        if (wallObject != null)
        {
            wallMat = wallObject.GetComponent<MeshRenderer>();
        }


        if (groundObject != null)
        {
            groundMat = groundObject.GetComponent<MeshRenderer>();
        }

        SpawnItems();
        ApplyFixedTheme();
        ApplyFixedVisuals();
    }

    private void Update()
    {
        if (currentSelectedIndex >= 0 && contanerData != null && contanerData.colorList != null && currentSelectedIndex < contanerData.colorList.Count)
        {
            if (wallMat != null)
                wallMat.material.color = contanerData.colorList[currentSelectedIndex].wallColor;

            if (groundMat != null)
                groundMat.material.color = contanerData.colorList[currentSelectedIndex].groundColor;
        }
    }

    private void SpawnItems()
    {
        if (prefab == null || parent == null || contanerData == null || contanerData.colorList == null)
        {
            return;
        }

        foreach (Transform child in parent)
            Destroy(child.gameObject);

        spawnedButtons.Clear();

        int actualSpawnCount = Mathf.Min(spawnCount, contanerData.colorList.Count);

        for (int i = 0; i < actualSpawnCount; i++)
        {
            GameObject obj = Instantiate(prefab, parent);
            obj.SetActive(true);
            spawnedButtons.Add(obj);

            Image img = obj.GetComponent<Image>();

            if (img != null && contanerData.colorList[i].buttonimg != null)
            {
                img.sprite = contanerData.colorList[i].buttonimg;
            }

            int buttonIndex = i;
            Button btn = obj.GetComponent<Button>();
            if (btn != null)
            {
                btn.onClick.AddListener(() => OnClickApplyButton(buttonIndex));
            }
        }
    }

    private void OnClickApplyButton(int index)
    {
        Debug.Log("[GridSpawn] Theme switching disabled");
        return;

        // if (contanerData == null || contanerData.colorList == null || index < 0 || index >= contanerData.colorList.Count)
        // {
        //     Debug.LogWarning("Color data missing or index out of range.");
        //     return;
        // }

        // Debug.Log("Button Clicked: " + index);

        // if (currentSelectedIndex != -1 && currentSelectedIndex < spawnedButtons.Count)
        //     ResetButtonSprite(currentSelectedIndex);

        // ApplyTheme(index);
        // ChangeButtonSprite(index);
        // currentSelectedIndex = index;


        // PlayerPrefs.SetInt(SelectedIndexKey, index);
        // PlayerPrefs.Save();


        // UpdateScorePanels(index);

        // GameObject clickedSoundObj = GameObject.Find("ClickedSound");
        // if (clickedSoundObj != null)
        // {
        //     clickedSoundObj.GetComponent<AudioSource>()?.Play();
        // }

        // if (UiManager.Instance != null)
        // {
        //     UiManager.Instance.CloseSettingPanel();
        // }
    }

    private void UpdateScorePanels(int index)
    {
        var themeData = contanerData.colorList[index];
        if (themeData.scorePanal == null || themeData.scorePanal.Length == 0)
            return;

        for (int i = 0; i < implementRealscorePanal.Length; i++)
        {
            if (i < themeData.scorePanal.Length && implementRealscorePanal[i] != null)
            {
                implementRealscorePanal[i].sprite = themeData.scorePanal[i];
                Debug.Log($"✅ Updated ScorePanel {i} for theme index {index}");
            }
        }
    }

    private void ApplyTheme(int index)
    {
        if (contanerData == null || contanerData.colorList == null) return;

        if (wallMat != null)
        {
            wallMat.material.color = contanerData.colorList[index].wallColor;
        }

        if (groundMat != null)
        {
            groundMat.material.color = contanerData.colorList[index].groundColor;
        }

        if (background != null && contanerData.colorList[index].newSprite != null)
        {
            background.sprite = contanerData.colorList[index].newSprite;
        }
    }

    private void ApplyFixedVisuals()
    {
        // Background
        if (background != null && fixedBackground != null)
        {
            background.sprite = fixedBackground;
        }

        // Top UI / score panels
        if (implementRealscorePanal != null)
        {
            foreach (var img in implementRealscorePanal)
            {
                if (img != null && fixedContainerSprite != null)
                {
                    img.sprite = fixedContainerSprite;
                }
            }
        }

        Debug.Log("[GridSpawn] Fixed visuals applied (single theme)");
    }



    private void ChangeButtonSprite(int index)
    {
        if (index < spawnedButtons.Count)
        {
            Image img = spawnedButtons[index].GetComponent<Image>();
            if (img != null && contanerData.colorList[index].changebuttonimg != null)
            {
                img.sprite = contanerData.colorList[index].changebuttonimg;
            }
        }
    }

    private void ResetButtonSprite(int index)
    {
        if (index < spawnedButtons.Count)
        {
            Image img = spawnedButtons[index].GetComponent<Image>();
            if (img != null && contanerData.colorList[index].buttonimg != null)
            {
                img.sprite = contanerData.colorList[index].buttonimg;
            }
        }
    }

    private void LoadSavedTheme()
    {
        // int savedIndex = PlayerPrefs.GetInt(SelectedIndexKey, -1);
        // if (savedIndex != -1 && contanerData != null && contanerData.colorList != null && savedIndex < contanerData.colorList.Count)
        // {
        //     ApplyTheme(savedIndex);
        //     ChangeButtonSprite(savedIndex);
        //     currentSelectedIndex = savedIndex;


        //     UpdateScorePanels(savedIndex);

        //     Debug.Log("Loaded saved theme index: " + savedIndex);
        // }
    }

    private void ApplyFixedTheme()
    {
        if (contanerData == null || contanerData.colorList == null)
            return;

        int index = Mathf.Clamp(forcedThemeIndex, 0, contanerData.colorList.Count - 1);

        ApplyTheme(index);
        UpdateScorePanels(index);
        currentSelectedIndex = index;

        Debug.Log($"[GridSpawn] Forced theme index = {index}");
    }


    public void ResetAllButtonSprites()
    {
        for (int i = 0; i < spawnedButtons.Count; i++)
            ResetButtonSprite(i);

        currentSelectedIndex = -1;
    }

    public int GetCurrentSelectedIndex() => currentSelectedIndex;

    public void SetTheme(int index)
    {
        if (index >= 0 && index < contanerData.colorList.Count)
            OnClickApplyButton(index);
    }

    private void OnDestroy()
    {
        if (parent != null)
        {
            Button[] buttons = parent.GetComponentsInChildren<Button>();
            foreach (Button btn in buttons)
                btn.onClick.RemoveAllListeners();
        }

        spawnedButtons.Clear();
    }
}

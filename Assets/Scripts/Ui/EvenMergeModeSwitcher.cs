using Assets.Scripts.GameManager;
using UnityEngine;
using UnityEngine.UI;

public class EvenMergeModeSwitcher : MonoBehaviour
{
    // ⭐ GLOBAL INSTANCE
    public static EvenMergeModeSwitcher Instance { get; private set; }

    [Header("Toggle Setup")]
    public Toggle toggle2D3D;      // assign the Toggle component
    public GameObject toggleRoot;  // assign the root GameObject (Toggle2D3D)

    const string PREF_KEY = "GameMode";  // stores 2 or 3

    void Awake()
    {
        // Register global instance
        Instance = this;
    }

    void Start()
    {
        // Load saved mode or default to 2D
        int saved = PlayerPrefs.GetInt(PREF_KEY, 2);
        bool is2D = saved == 2;

        if (toggle2D3D != null)
        {
            toggle2D3D.isOn = is2D;
            toggle2D3D.onValueChanged.AddListener(OnToggleChanged);
        }
    }

    void OnToggleChanged(bool is2D)
    {
        PlayerPrefs.SetInt(PREF_KEY, is2D ? 2 : 3);
        PersistentAudioManager.Instance.PlayClickSound();
    }

    public bool Is2D()
    {
        return toggle2D3D == null ? true : toggle2D3D.isOn;
    }

    // ⭐ THIS METHOD controls visibility of the toggle
    public void ShowToggle(bool show)
    {
        if (toggleRoot != null)
            toggleRoot.SetActive(show);
    }
}

using Assets.Scripts.Configs;
using Assets.Scripts.GameManager;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Lightweight EXP/Level HUD that mirrors the 2D panel_top behaviour for the 3D scene.
/// Drop this on the shared panel prefab and wire the level label + fill image.
/// </summary>
public class ExpProgressUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Image expFill;

		[Header("Level Up Panel (3D)")]
		// Optional reference to an existing LevelUp panel in the scene (e.g. under EvenMerge3D_Root/UI/panel_top/LevelUp).
		// If assigned, we will simply activate this panel instead of instantiating a prefab.
		[SerializeField] private LevelUp levelUpPanel;

		[Header("Optional")]
		// Enabled by default so 3D mode gets the same LevelUp popup behaviour as 2D.
		[SerializeField] private bool playLevelUpPopup = true;

    private void OnEnable()
    {
        GlobalEventHandle.AddExpHandle += OnExpChanged;
        RefreshUI();
    }

    private void OnDisable()
    {
        GlobalEventHandle.AddExpHandle -= OnExpChanged;
    }

    private void Start()
    {
        RefreshUI();
    }

    private void OnExpChanged(bool isLevelUp)
    {
        RefreshUI();

        if (playLevelUpPopup && isLevelUp)
        {
            ShowLevelUpPopup();
        }
    }

    private void RefreshUI()
    {
        GM gm = GM.GetInstance();

        if (levelText == null)
        {
            levelText = FindTextFallback("txt_lv", "Level");
        }

        if (expFill == null)
        {
            expFill = FindImageFallback("img_exp", "img_lv_progress");
        }

        if (levelText != null)
        {
            // Update to Tier Name
            levelText.text = gm.GetTierName(gm.Exp);
        }

        if (expFill != null)
        {
            // Update to Tier Progress
            expFill.fillAmount = gm.GetTierProgress(gm.Exp);
        }
    }

    private TextMeshProUGUI FindTextFallback(params string[] names)
    {
        foreach (string n in names)
        {
            Transform child = transform.Find(n);
            if (child != null && child.TryGetComponent(out TextMeshProUGUI tmp))
            {
                return tmp;
            }
        }

        return GetComponentInChildren<TextMeshProUGUI>(true);
    }

    private Image FindImageFallback(params string[] names)
    {
        foreach (string n in names)
        {
            Transform child = transform.Find(n);
            if (child != null && child.TryGetComponent(out Image img))
            {
                return img;
            }
        }

        return GetComponentInChildren<Image>(true);
    }

    private void ShowLevelUpPopup()
    {
		// Prefer using an existing LevelUp panel in the scene (3D UI) if provided.
		if (levelUpPanel == null)
		{
			levelUpPanel = FindObjectOfType<LevelUp>(true);
		}

		if (levelUpPanel != null)
		{
			// Activate the panel GameObject.
			levelUpPanel.gameObject.SetActive(true);
		}
		else
		{
			// Fallback: reuse existing assets so the behaviour matches the 2D mode (MainScene.ShowLevelUp).
			GameObject levelUpPrefab = Resources.Load<GameObject>("Prefabs/LevelUp");
			if (levelUpPrefab != null)
			{
				DialogManager dm = DialogManager.GetInstance();

				if (dm != null)
				{
					GameObject dialog = Instantiate(levelUpPrefab);
					dm.show(dialog, false);
				}
				else
				{
					Transform parent = transform.root != null ? transform.root : transform;
					Instantiate(levelUpPrefab, parent, false);
				}
			}
		}

		// Level-up VFX.
		GameObject fxPrefab = Resources.Load<GameObject>("Prefabs/effect/eff_levelup");
		if (fxPrefab != null)
		{
			GameObject fx = Instantiate(fxPrefab, transform, false);
			Destroy(fx, 5f);
		}

		// Match 2D behaviour: play level up sound.
		if (PersistentAudioManager.Instance != null)
		{
			PersistentAudioManager.Instance.PlayEffect("sound_eff_levelUp");
		}

		// Freeze gameplay while the level-up panel is visible.
		Time.timeScale = 0f;

		// Also pause Game 2 logic (used by the original 2D implementation).
		if (GM.GetInstance().GameId == 2 && G2BoardGenerator.GetInstance() != null)
		{
			G2BoardGenerator.GetInstance().IsPuase = true;
		}
    }
}


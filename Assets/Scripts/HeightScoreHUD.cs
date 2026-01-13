using TMPro;
using UnityEngine;

/// <summary>
/// Simple HUD driver for the 3D scene that keeps CURRENT and HIGH score texts
/// updated while gameplay is running, independent of where HeightScore lives.
/// </summary>
public class HeightScoreHUD : MonoBehaviour
{
    [Header("UI References (3D HUD)")]
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;

    private void Awake()
    {
        // We want this HUD to stay active during gameplay.
        if (currentScoreText != null)
            currentScoreText.gameObject.SetActive(true);

        if (highScoreText != null)
            highScoreText.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (HeightScore.Instance == null)
            return;

        // Current score displayed live during gameplay.
        if (currentScoreText != null)
        {
            //currentScoreText.text = "Current Score:" + HeightScore.Instance.GetCurrentScore().ToString();
            currentScoreText.text = HeightScore.Instance.GetCurrentScore().ToString();
        }

        // High score displayed live during gameplay.
        if (highScoreText != null)
        {
            //highScoreText.text = "High Score:" + HeightScore.highScore.ToString();
            highScoreText.text = HeightScore.highScore.ToString();
        }
    }
}



using Assets.Scripts.Configs;
using Assets.Scripts.GameManager;
using DG.Tweening;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelUp : MonoBehaviour
{
	public GameObject m_img_cirlce;

	//public Text m_txt_lv_value;
	public TextMeshProUGUI m_txt_lv_value;

	public Text m_txt_awards;

	[Header("Optional FX")]
	// Optional reference to a particle effect that should only play when the
	// level-up panel is shown (e.g. a burst around the level circle).
	[SerializeField] private ParticleSystem levelUpEffect;

	private void Start()
	{
		this.InitUI();
	}

	private void OnEnable()
	{
		this.InitUI();
	}

	private void Update()
	{
	}

	public void OnClickOK()
	{
		if (Configs.TPlayers.ContainsKey(GM.GetInstance().Lv.ToString()))
		{
			TPlayer tPlayer = Configs.TPlayers[GM.GetInstance().Lv.ToString()];
			GM.GetInstance().AddDiamond(tPlayer.Item, false);
		}
		if (GM.GetInstance().GameId == 2 && G2BoardGenerator.GetInstance() != null)
		{
			G2BoardGenerator.GetInstance().IsPuase = false;
		}

		// Stop the optional level-up particle effect when closing the panel.
		if (levelUpEffect != null)
		{
			levelUpEffect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
		}

		// Resume normal time scale (may have been set to 0f when the panel was shown).
		if (Mathf.Approximately(Time.timeScale, 0f))
		{
			Time.timeScale = 1f;
		}

		// If this LevelUp is shown via DialogManager (2D flow), close the dialog.
		DialogManager dm = DialogManager.GetInstance();
		if (dm != null)
		{
			dm.Close(null);
		}
		else
		{
			// Otherwise, simply hide this panel (3D in-scene panel).
			gameObject.SetActive(false);
		}

		// Play sound_eff_button as requested
		PersistentAudioManager.Instance.PlayEffect("sound_eff_button");
	}

	private void InitUI()
	{
        // Show Tier Name instead of Level Number
		this.m_txt_lv_value.text = GM.GetInstance().GetTierName(GM.GetInstance().Exp);
		if (Configs.TPlayers.ContainsKey(GM.GetInstance().Lv.ToString()))
		{
			TPlayer tPlayer = Configs.TPlayers[GM.GetInstance().Lv.ToString()];
			if (this.m_txt_awards != null)
			{
				this.m_txt_awards.text = string.Format("+{0}", tPlayer.Item);
			}
		}
		DOTween.Kill(this.m_img_cirlce, false);
		Sequence _sequence = DOTween.Sequence();
		Image component = this.m_img_cirlce.GetComponent<Image>();
		_sequence.Append(component.transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0f));
		_sequence.Append(component.DOFade(1f, 0f));
		_sequence.Append(component.DOFade(0f, 1f));
		_sequence.Insert(0f, component.transform.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 1f));
		_sequence.AppendInterval(1f);
		_sequence.SetLoops(-1);

		// Play the optional particle effect when the level-up panel is shown.
		if (levelUpEffect != null)
		{
			levelUpEffect.Play();
		}
	}
}

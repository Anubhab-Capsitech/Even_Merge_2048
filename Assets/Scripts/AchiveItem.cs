using Assets.Scripts.Configs;
using Assets.Scripts.GameManager;
using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchiveItem : MonoBehaviour
{
	public Image img_progress;

	public Text txt_src;

	public Text txt_dst;

	public Text txt_title;

	public Text txt_desc;

	public Text txt_finish;

	public Text txt_awards;

	public Text m_btn_txt;

	public Button btn_get;

	public Image img_icon;

	public int type;

	[SerializeField]
	public List<Sprite> sprites = new List<Sprite>();

	[SerializeField]
	public List<Color> m_btnColors = new List<Color>();

	[SerializeField]
	public List<Color> m_btnColors2 = new List<Color>();

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnEnable()
	{
		this.BindDataToUI();
	}

	private void BindDataToUI()
	{
		LocalData localData = AchiveData.GetInstance().Get(this.type);
		TAchive tAchive = Configs.TAchives[localData.key.ToString()];
		float num = (float)localData.value / (float)tAchive.Value;
		num = ((num >= 1f) ? 1f : num);
		this.img_progress.fillAmount = num;
		this.txt_title.GetComponent<LanguageComponent>().SetText(tAchive.Title);
		this.txt_desc.GetComponent<LanguageComponent>().SetText(tAchive.Desc);
		this.txt_src.text = localData.value.ToString() + "/" + tAchive.Value.ToString();
		this.txt_awards.text = string.Format("{0}", tAchive.Item);
		int num2 = tAchive.ID % 100;
		num2 = ((num2 > this.sprites.Count) ? this.sprites.Count : num2);
		this.img_icon.sprite = this.sprites[num2 - 1];
		int skinID = GM.GetInstance().SkinID;
		List<Color> list;
		if (skinID != 1)
		{
			if (skinID != 2)
			{
				list = this.m_btnColors;
			}
			else
			{
				list = this.m_btnColors2;
			}
		}
		else
		{
			list = this.m_btnColors;
		}
		switch (localData.status)
		{
		case -3:
			this.btn_get.GetComponent<Image>().color = list[2];
			this.m_btn_txt.GetComponent<LanguageComponent>().SetText("TXT_NO_20004");
			return;
		case -2:
			this.btn_get.GetComponent<Image>().color = list[1];
			this.m_btn_txt.GetComponent<LanguageComponent>().SetText("TXT_NO_20001");
			return;
		case -1:
			this.btn_get.GetComponent<Image>().color = list[0];
			this.m_btn_txt.GetComponent<LanguageComponent>().SetText("TXT_NO_20001");
			return;
		default:
			return;
		}
	}

	public void OnClickButton()
	{
		int status = AchiveData.GetInstance().Get(this.type).status;
		if (status == -3)
		{
			ToastManager.Show("TXT_NO_50017", true);
			return;
		}
		if (status == -1)
		{
			ToastManager.Show("TXT_NO_50016", true);
			return;
		}
		LocalData localData = AchiveData.GetInstance().Get(this.type);
		if (!AchiveData.GetInstance().Finish(this.type))
		{
			return;
		}
		TAchive tAchive = Configs.TAchives[localData.key.ToString()];
		GM.GetInstance().AddDiamond(tAchive.Item, true);
		Utils.ShowVideoConfirm(tAchive.Item, "TXT_NO_50015", Confirm.ConfirmType.VIDEO2);
		this.BindDataToUI();
		PersistentAudioManager.Instance.PlayEffect("sound_eff_achive");
	}
}

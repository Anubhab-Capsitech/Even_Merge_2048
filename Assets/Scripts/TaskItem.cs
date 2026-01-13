using Assets.Scripts.Configs;
using Assets.Scripts.GameManager;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class TaskItem : MonoBehaviour
{
	
	public Image img_pregress;

	public Text txt_desc;

	public Button button;

	public Text btn_txt;

	public Text txt_progress;

	public Text txt_awards;

	[SerializeField]
	public List<Color> m_colors = new List<Color>();

	[SerializeField]
	public List<Color> m_colors2 = new List<Color>();

	public int id;

	private void Start()
	{
		GlobalEventHandle.OnRefreshTaskHandle = (Action<int>)Delegate.Combine(GlobalEventHandle.OnRefreshTaskHandle, new Action<int>(this.DoRefresh));
		this.BindDataToUI();
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		GlobalEventHandle.OnRefreshTaskHandle = (Action<int>)Delegate.Remove(GlobalEventHandle.OnRefreshTaskHandle, new Action<int>(this.DoRefresh));
	}

	private void OnEnable()
	{
		this.BindDataToUI();
	}

	public void BindDataToUI()
	{
		if (this.id == 0)
		{
			return;
		}
		if (!Configs.TTasks.ContainsKey(this.id.ToString()))
		{
			return;
		}
		LocalData localData = TaskData.GetInstance().Get(this.id);
		TTask tTask = Configs.TTasks[this.id.ToString()];
		this.txt_awards.text = string.Format("{0}", tTask.Item);
		this.txt_desc.GetComponent<LanguageComponent>().SetText(tTask.Desc);
		int num = (localData.value > tTask.Value) ? tTask.Value : localData.value;
		this.txt_progress.text = string.Format("{0}/{1}", num, tTask.Value);
		float num2 = (float)localData.value / (float)tTask.Value;
		num2 = ((num2 > 1f) ? 1f : num2);
		this.img_pregress.fillAmount = num2;
		int skinID = GM.GetInstance().SkinID;
		List<Color> list;
		if (skinID != 1)
		{
			if (skinID != 2)
			{
				list = this.m_colors;
			}
			else
			{
				list = this.m_colors2;
			}
		}
		else
		{
			list = this.m_colors;
		}
		if (localData.status == -1)
		{
			this.btn_txt.GetComponent<LanguageComponent>().SetText("TXT_NO_20002");
			this.button.GetComponent<Image>().color = list[0];
			return;
		}
		if (localData.status == -2)
		{
			this.btn_txt.GetComponent<LanguageComponent>().SetText("TXT_NO_20001");
			this.button.GetComponent<Image>().color = list[1];
			return;
		}
		if (localData.status == -3)
		{
			this.btn_txt.GetComponent<LanguageComponent>().SetText("TXT_NO_20004");
			this.button.GetComponent<Image>().color = list[2];
		}
	}

	public void OnClickButton()
	{
		if (this.id == 0)
		{
			return;
		}
		if (!Configs.TTasks.ContainsKey(this.id.ToString()))
		{
			return;
		}
		LocalData data = TaskData.GetInstance().Get(this.id);
		switch (data.status)
		{
		case -3:
			ToastManager.Show("TXT_NO_50006", true);
			return;
		case -2:
			PersistentAudioManager.Instance.PlayEffect("sound_eff_task");
			AdsManager.GetInstance().Play(AdsManager.AdType.GetTask, delegate
			{
				if (!TaskData.GetInstance().Finish(this.id))
				{
					GlobalEventHandle.EmitDoClickBottom();
					return;
				}
				TTask tTask = Configs.TTasks[data.key.ToString()];
				GM.GetInstance().AddDiamond(tTask.Item, true);
				this.BindDataToUI();
			}, null, 5, null);
			return;
		case -1:
			GlobalEventHandle.EmitDoClickBottom();
			return;
		default:
			return;
		}
	}

	private void DoRefresh(int id)
	{
		if (this.id != id)
		{
			return;
		}
		this.BindDataToUI();
	}
}

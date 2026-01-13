using Assets.Scripts.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingLanguage : MonoBehaviour
{
	[SerializeField]
	public List<Toggle> m_toggles = new List<Toggle>();

	private Dictionary<int, SystemLanguage> m_dicts = new Dictionary<int, SystemLanguage>();

	private void Start()
	{
		this.m_dicts.Add(0, SystemLanguage.English);
		this.m_dicts.Add(1, SystemLanguage.Chinese);
		this.m_dicts.Add(2, SystemLanguage.ChineseTraditional);
		this.m_dicts.Add(3, SystemLanguage.Indonesian);
		this.m_dicts.Add(4, SystemLanguage.Portuguese);
		this.m_dicts.Add(5, SystemLanguage.Japanese);
		this.m_dicts.Add(6, SystemLanguage.Russian);
		this.SelectLanguage();
	}

	private void Update()
	{
	}

	public void OnClickToggle(bool isOn)
	{
		for (int i = 0; i < this.m_toggles.Count; i++)
		{
			if (this.m_toggles[i].isOn)
			{
				Language.GetInstance().Set(this.m_dicts[i]);
			}
		}
	}

	public void OnClickClose()
	{
		DialogManager.GetInstance().Close(null);
	}

	private void SelectLanguage()
	{
		this.Hide();
		SystemLanguage id = Language.GetInstance().Id;
		if (id <= SystemLanguage.Japanese)
		{
			if (id != SystemLanguage.Chinese)
			{
				if (id == SystemLanguage.Indonesian)
				{
					this.m_toggles[3].isOn = true;
					return;
				}
				if (id != SystemLanguage.Japanese)
				{
					goto IL_B8;
				}
				this.m_toggles[5].isOn = true;
				return;
			}
		}
		else if (id <= SystemLanguage.Russian)
		{
			if (id == SystemLanguage.Portuguese)
			{
				this.m_toggles[4].isOn = true;
				return;
			}
			if (id != SystemLanguage.Russian)
			{
				goto IL_B8;
			}
			this.m_toggles[6].isOn = true;
			return;
		}
		else if (id != SystemLanguage.ChineseSimplified)
		{
			if (id != SystemLanguage.ChineseTraditional)
			{
				goto IL_B8;
			}
			this.m_toggles[2].isOn = true;
			return;
		}
		this.m_toggles[1].isOn = true;
		return;
		IL_B8:
		this.m_toggles[0].isOn = true;
	}

	private void Hide()
	{
		using (List<Toggle>.Enumerator enumerator = this.m_toggles.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.isOn = false;
			}
		}
	}
}

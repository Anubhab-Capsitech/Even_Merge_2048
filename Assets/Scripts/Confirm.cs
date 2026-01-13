using Assets.Scripts.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Confirm : MonoBehaviour
{
	public enum ConfirmType
	{
		OK,
		OkAndCancel,
		VIDEO,
		VIDEO2
	}

	private Action m_onClickOkHandle;

	private Action m_onClickCancelHandle;

	public Text m_txt_content;

	public Text m_txt_value;

	public GameObject m_btn_ok;

	public GameObject m_btn_cancel;

	public GameObject m_btn_confirm;

	public GameObject m_btn_vedio;

	private Confirm.ConfirmType m_type = Confirm.ConfirmType.OkAndCancel;

	private void Start()
	{
		switch (this.m_type)
		{
		case Confirm.ConfirmType.OK:
			this.m_btn_ok.SetActive(false);
			this.m_btn_vedio.SetActive(false);
			this.m_btn_cancel.SetActive(false);
			this.m_btn_confirm.SetActive(true);
			return;
		case Confirm.ConfirmType.OkAndCancel:
			this.m_btn_ok.SetActive(true);
			this.m_btn_cancel.SetActive(true);
			this.m_btn_confirm.SetActive(false);
			this.m_btn_vedio.SetActive(false);
			return;
		case Confirm.ConfirmType.VIDEO:
			this.m_btn_ok.SetActive(false);
			this.m_btn_cancel.SetActive(true);
			this.m_btn_confirm.SetActive(false);
			this.m_btn_vedio.SetActive(true);
			return;
		case Confirm.ConfirmType.VIDEO2:
			this.m_btn_ok.SetActive(true);
			this.m_btn_cancel.SetActive(true);
			this.m_btn_confirm.SetActive(false);
			this.m_btn_vedio.SetActive(false);
			return;
		default:
			return;
		}
	}

	private void Update()
	{
	}

	public void SetType(Confirm.ConfirmType type)
	{
		this.m_type = type;
	}

	public void SetText(string txt, bool isID = true)
	{
		if (isID)
		{
			this.m_txt_content.GetComponent<LanguageComponent>().SetText(txt);
		}
		else
		{
			this.m_txt_content.text = txt;
		}
		this.m_txt_content.font = Language.GetFont();
	}

	public void SetValue(int value)
	{
		this.m_txt_value.text = value.ToString();
	}

	public void OnClickOk()
	{
		DialogManager.GetInstance().Close(null);
		Action _action = this.m_onClickOkHandle;
		if (_action == null)
		{
			return;
		}
        _action();
	}

	public void OnClickCancel()
	{
		DialogManager.GetInstance().Close(null);
		Action _action = this.m_onClickCancelHandle;
		if (_action == null)
		{
			return;
		}
        _action();
	}

	public void SetCallFunc(Action okCallFunc = null, Action cancelCallFunc = null)
	{
		this.m_onClickOkHandle = okCallFunc;
		this.m_onClickCancelHandle = cancelCallFunc;
	}
}

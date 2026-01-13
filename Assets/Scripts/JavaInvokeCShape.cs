using System;
using UnityEngine;

public class JavaInvokeCShape : MonoBehaviour
{
	private static JavaInvokeCShape m_instance;

	public Action OnAdsComplateHandle;

	public Action OnAdsCancelHandle;

	public Action OnShareHandle;

	private void Awake()
	{
		JavaInvokeCShape.m_instance = this;
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public static JavaInvokeCShape GetInstance()
	{
		return JavaInvokeCShape.m_instance;
	}

	public void OnAdsComplateCallback(string channel)
	{
		Action _action = this.OnAdsComplateHandle;
		if (_action == null)
		{
			return;
		}
        _action();
	}

	public void OnAdsCancelCallback(string channel)
	{
		Action _action = this.OnAdsCancelHandle;
		if (_action == null)
		{
			return;
		}
        _action();
	}

	public void OnShareSuccess(string channel)
	{
		ToastManager.Show("share sucess", false);
		Action _action = this.OnShareHandle;
		if (_action == null)
		{
			return;
		}
        _action();
	}
}

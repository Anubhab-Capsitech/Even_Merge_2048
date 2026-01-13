using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Splash : MonoBehaviour
{
   
	public Image m_logo;

	public Image m_logo2;

	private bool m_isComplateAni;

	private AsyncOperation m_async;

	private void Awake()
	{
		this.m_logo.gameObject.SetActive(false);
		this.m_logo.color = new Color(1f, 1f, 1f, 0f);
		this.m_logo2.gameObject.SetActive(false);
		this.m_logo2.color = new Color(1f, 1f, 1f, 0f);
	}

	private void Start()
	{
		this.m_logo.gameObject.SetActive(true);
		Sequence _sequence = DOTween.Sequence();
        _sequence.Append(this.m_logo.DOFade(1f, 1f));
        _sequence.AppendCallback(delegate
		{
			this.m_isComplateAni = true;
		});
		this.m_logo2.gameObject.SetActive(true);
		this.m_logo2.DOFade(1f, 1f);
		base.StartCoroutine(this.LoadAsyncScene());
	}

	private void Update()
	{
	}

	
	private IEnumerator LoadAsyncScene()
	{
		while (true)
		{
			int num = 0;
			if (num != 0)
			{
				if (num != 1)
				{
					break;
				}
			}
			else
			{
				this.m_async = SceneManager.LoadSceneAsync("MainScene");
				this.m_async.allowSceneActivation = false;
			}
			if (this.m_async.isDone)
			{
				goto Block_4;
			}
			if (this.m_isComplateAni)
			{
				this.m_async.allowSceneActivation = true;
			}
			yield return null;
		}
		yield break;
		Block_4:
		yield break;
	}
}

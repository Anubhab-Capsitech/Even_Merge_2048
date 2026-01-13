using Assets.Scripts.GameManager;
using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Task : MonoBehaviour
{

	[Serializable]
	private sealed class CommonClass
	{
		public static readonly Task.CommonClass _common = new Task.CommonClass();

		public static TweenCallback _tween1;

		public static TweenCallback _tween2;

		internal void EntranceAni1()
		{
		}

		internal void EntranceAni2()
		{
		}
	}

	[SerializeField]
	public List<TaskItem> items = new List<TaskItem>();

	public RectTransform m_video;

	public Text m_videoTimer;

	private void Start()
	{
		GlobalTimer.GetInstance().RefreshHandle += new Action(this.Refresh);
		GlobalEventHandle.AdsHandle += new Action<string, bool>(this.OnRefreshAdsTimer);
	}

	private void Update()
	{
		Utils.BackListener(base.gameObject, delegate
		{
			this.OnClickReturn();
		});
	}

	private void OnEnable()
	{
		this.EntranceAni();
	}

	private void OnDestroy()
	{
		GlobalTimer.GetInstance().RefreshHandle -= new Action(this.Refresh);
	}

	public void OnClickAds()
	{
		AdsManager.GetInstance().Play(AdsManager.AdType.Stimulate, null, null, 5, null);
	}

	public void OnClickReturn()
	{
		GlobalEventHandle.EmitClickPageButtonHandle("main", 0);
	}

	private void Refresh()
	{
		using (List<TaskItem>.Enumerator enumerator = this.items.GetEnumerator())
		{
			while (enumerator.MoveNext())
			{
				enumerator.Current.BindDataToUI();
			}
		}
	}

	private void EntranceAni()
	{
		if (AdsManager.GetInstance().IsWatch)
		{
			this.m_videoTimer.GetComponent<LanguageComponent>().SetText("TXT_NO_50037");
		}
		if (AdsManager.GetInstance().IsWatch)
		{
			this.PlayAdsTipAni();
		}
		Sequence sequence = DOTween.Sequence();
		for (int i = 0; i < this.items.Count; i++)
		{
			TaskItem obj = this.items[i];
			Vector3 localPosition = obj.transform.localPosition;
			float y = localPosition.y;
			localPosition.y = -568f;
			obj.transform.localPosition = localPosition;
			obj.gameObject.SetActive(false);
			Tweener t = obj.transform.DOLocalMoveY(y, 0.2f, false).SetDelay((float)i * 0.05f).OnStart(delegate
			{
				obj.gameObject.SetActive(true);
			});
			sequence.Insert(0f, t);
		}
		Sequence arg_11A_0 = sequence;
		TweenCallback arg_11A_1;
		if ((arg_11A_1 = Task.CommonClass._tween1) == null)
		{
			arg_11A_1 = (Task.CommonClass._tween1 = new TweenCallback(Task.CommonClass._common.EntranceAni1));
		}
		arg_11A_0.OnStart(arg_11A_1);
		Sequence arg_140_0 = sequence;
		TweenCallback arg_140_1;
		if ((arg_140_1 = Task.CommonClass._tween2) == null)
		{
			arg_140_1 = (Task.CommonClass._tween2 = new TweenCallback(Task.CommonClass._common.EntranceAni2));
		}
		arg_140_0.OnComplete(arg_140_1);
	}

	private void OnRefreshAdsTimer(string timer, bool isWatch)
	{
		this.m_videoTimer.text = timer;
		if (AdsManager.GetInstance().IsWatch)
		{
			this.m_videoTimer.GetComponent<LanguageComponent>().SetText("TXT_NO_50037");
		}
		if (isWatch)
		{
			this.PlayAdsTipAni();
			return;
		}
		this.StopAdsTipsAni();
	}

	private void PlayAdsTipAni()
	{
		this.StopAdsTipsAni();
		Sequence expr_0B = DOTween.Sequence();
		expr_0B.Append(this.m_video.transform.DOScale(1.2f, 1f).SetEase(Ease.Linear));
		expr_0B.Append(this.m_video.transform.DOScale(1f, 1f).SetEase(Ease.Linear));
		expr_0B.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, 20f), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
		expr_0B.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, -20f), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
		expr_0B.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, 10f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
		expr_0B.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, -10f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
		expr_0B.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
		expr_0B.SetLoops(-1);
		expr_0B.SetTarget(this.m_video);
	}

	private void StopAdsTipsAni()
	{
		DOTween.Kill(this.m_video, false);
		this.m_video.localRotation = new Quaternion(0f, 0f, 0f, 0f);
		this.m_video.localScale = new Vector3(1f, 1f, 1f);
	}
}

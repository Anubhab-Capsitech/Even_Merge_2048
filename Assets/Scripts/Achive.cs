using Assets.Scripts.GameManager;
using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Achive : MonoBehaviour
{
    /*
	private sealed class __c__DisplayClass8_0
	{
		public AchiveItem obj;

		internal void _EntranceAni_b__2()
		{
			this.obj.gameObject.SetActive(true);
		}
	}
    */

	[Serializable]
	private sealed class AnimClass
	{
		public static readonly Achive.AnimClass animFunc = new Achive.AnimClass();

		public static TweenCallback _animCallback1;

		public static TweenCallback _animCallback2;

		internal void _EntranceAni1()
		{
		}

		internal void _EntranceAni2()
		{
		}
	}

	[SerializeField]
	public List<AchiveItem> m_items;

	public RectTransform m_video;

	public Text m_videoTimer;

	private void Start()
	{
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

	public void OnClickAds()
	{
		AdsManager.GetInstance().Play(AdsManager.AdType.Stimulate, null, null, 5, null);
	}

	public void OnClickReturn()
	{
		GlobalEventHandle.EmitClickPageButtonHandle("main", 0);
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
		for (int i = 0; i < this.m_items.Count; i++)
		{
			AchiveItem obj = this.m_items[i];
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
		Sequence _sequence1 = sequence;
		TweenCallback _tweenCallBack1;
		if ((_tweenCallBack1 = Achive.AnimClass._animCallback1) == null)
		{
            _tweenCallBack1 = (Achive.AnimClass._animCallback1 = new TweenCallback(Achive.AnimClass.animFunc._EntranceAni1));
		}
        _sequence1.OnStart(_tweenCallBack1);
		Sequence _sequence2 = sequence;
		TweenCallback _tweenCallback2;
		if ((_tweenCallback2 = Achive.AnimClass._animCallback2) == null)
		{
            _tweenCallback2 = (Achive.AnimClass._animCallback2 = new TweenCallback(Achive.AnimClass.animFunc._EntranceAni2));
		}
        _sequence2.OnComplete(_tweenCallback2);
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
		Sequence _sequence = DOTween.Sequence();
        _sequence.Append(this.m_video.transform.DOScale(1.2f, 1f).SetEase(Ease.Linear));
        _sequence.Append(this.m_video.transform.DOScale(1f, 1f).SetEase(Ease.Linear));
        _sequence.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, 20f), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        _sequence.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, -20f), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        _sequence.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, 10f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        _sequence.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, -10f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        _sequence.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        _sequence.SetLoops(-1);
        _sequence.SetTarget(this.m_video);
	}

	private void StopAdsTipsAni()
	{
		DOTween.Kill(this.m_video, false);
		this.m_video.localRotation = new Quaternion(0f, 0f, 0f, 0f);
		this.m_video.localScale = new Vector3(1f, 1f, 1f);
	}
}

using Assets.Scripts.GameManager;
using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class Skin : MonoBehaviour
{
	

	public GameObject[] m_items;

	public Sprite[] m_sprites;

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
		this.InitUI();
	}

	private void OnDestroy()
	{
	}

	public void OnClickAds()
	{
		AdsManager.GetInstance().Play(AdsManager.AdType.Stimulate, null, null, 5, null);
	}

	public void OnClickReturn()
	{
		GlobalEventHandle.EmitClickPageButtonHandle("main", 0);
	}

	public void OnClickSkin(int id = 0)
	{
		List<int> skinData = GM.GetInstance().GetSkinData();
		if (id > skinData.Count)
		{
			return;
		}
		if (skinData[id - 1] == 1)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("Prefabs/skinBuy") as GameObject);
			gameObject.GetComponent<SkinUnLock>().Load(id);
			gameObject.GetComponent<SkinUnLock>().SetOnUnlockSuccess(delegate(int skinID)
			{
				GM.GetInstance().SetLocalSkinID(id);
				this.InitUI();
			});
			DialogManager.GetInstance().show(gameObject, false);
			return;
		}
		GM.GetInstance().SetLocalSkinID(id);
		this.InitUI();
	}

	private void InitUI()
	{
		if (AdsManager.GetInstance().IsWatch)
		{
			this.m_videoTimer.GetComponent<LanguageComponent>().SetText("TXT_NO_50037");
		}
		if (AdsManager.GetInstance().IsWatch)
		{
			this.PlayAdsTipAni();
		}
		if (this.m_items == null || this.m_sprites == null)
		{
			return;
		}
		List<int> skinData = GM.GetInstance().GetSkinData();
		int num = 0;
		while (num < this.m_items.Length && num < skinData.Count)
		{
			int num2 = skinData[num];
			GameObject gameObject = this.m_items[num];
			switch (num2)
			{
			case 0:
				if (GM.GetInstance().SkinID == num + 1)
				{
					gameObject.transform.Find("img_status").GetComponent<Image>().sprite = this.m_sprites[1];
				}
				else
				{
					gameObject.transform.Find("img_status").GetComponent<Image>().sprite = this.m_sprites[0];
				}
				break;
			case 1:
				gameObject.transform.Find("img_status").GetComponent<Image>().sprite = this.m_sprites[2];
				break;
			case 2:
				if (GM.GetInstance().SkinID == num + 1)
				{
					gameObject.transform.Find("img_status").GetComponent<Image>().sprite = this.m_sprites[1];
				}
				else
				{
					gameObject.transform.Find("img_status").GetComponent<Image>().sprite = this.m_sprites[0];
				}
				break;
			}
			num++;
		}
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

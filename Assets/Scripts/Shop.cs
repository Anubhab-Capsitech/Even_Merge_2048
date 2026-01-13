using Assets.Scripts.GameManager;
using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
//using UnityEngine.Purchasing;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{

	public GameObject adsItem;

	public GameObject SubscriptionItem;

	[SerializeField]
	public List<GameObject> items = new List<GameObject>();

	public RectTransform m_video;

	public Text m_videoTimer;

	private int m_type = 1;

	private bool isPlaying;

	public int Type
	{
		get
		{
			return this.m_type;
		}
		set
		{
			this.m_type = value;
		}
	}

	private void Start()
	{
		GlobalEventHandle.AdsHandle += new Action<string, bool>(this.OnRefreshAdsTimer);
		this.LoadData();
		this.InitUI();
		this.InitEvent();
	}

	private void Update()
	{
		Utils.BackListener(base.gameObject, delegate
		{
			this.OnClickReturn();
		});
	}

	private void OnDestroy()
	{
		this.DestroyEvent();
	}

	private void OnEnable()
	{
	}

	public void OnClickAds()
	{
		AdsManager.GetInstance().Play(AdsManager.AdType.Stimulate, null, null, 5, null);
	}

	public void OnClickReturn()
	{
		//if (GM.GetInstance().GameId != 0 && this.Type != 2)
		//{
		//	Utils.ShowVideoConfirm(5, "TXT_NO_50053", Confirm.ConfirmType.VIDEO);
		//}
		GlobalEventHandle.EmitClickPageButtonHandle("main", 0);
	}

	public void OnClickPurchase(string productId)
	{
		if (this.isPlaying)
		{
			return;
		}
		PersistentAudioManager.Instance.PlayEffect("sound_eff_button");
		//Store.getInstance().InitiatePurchase(productId);
	}

	//public void OnRestorePurchase()
	//{
	//	Store.getInstance().RestorePurchase();
	//}

	private void LoadData()
	{
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
		this.adsItem.transform.Find("btn_purchase").gameObject.SetActive(AdsManager.GetInstance().isShowAds());
		this.adsItem.transform.Find("btn_restore").gameObject.SetActive(AdsManager.GetInstance().isShowAds());
		//this.SubscriptionItem.transform.Find("btn_purchase").gameObject.SetActive(!GoodsManager.GetInstance().isPurchaseSubscription());
		//this.SubscriptionItem.transform.Find("btn_restore").gameObject.SetActive(!GoodsManager.GetInstance().isPurchaseSubscription());
		//this.SubscriptionItem.transform.Find("img_more").gameObject.SetActive(!GoodsManager.GetInstance().isPurchaseSubscription());
	}

	private void InitEvent()
	{
		GoodsManager _goodsManager = GoodsManager.GetInstance();
        //_goodsManager.ProcessConsumableHanle = (Action<Product, bool>)Delegate.Combine(_goodsManager.ProcessConsumableHanle, new Action<Product, bool>(this.OnProcessConsumable));
		GoodsManager _goodsManager2 = GoodsManager.GetInstance();
       // _goodsManager2.ProcessNonConsumableHandle = (Action<Product, bool>)Delegate.Combine(_goodsManager2.ProcessNonConsumableHandle, new Action<Product, bool>(this.OnProcessNonConsumable));
		GoodsManager _goodsManager3 = GoodsManager.GetInstance();
       // _goodsManager3.ProcessSubscriptionHandle = (Action<SubscriptionInfo, bool>)Delegate.Combine(_goodsManager3.ProcessSubscriptionHandle, new Action<SubscriptionInfo, bool>(this.OnProcessSubscription));
	}

	private void DestroyEvent()
	{
		GoodsManager _goodsManager = GoodsManager.GetInstance();
       // _goodsManager.ProcessConsumableHanle = (Action<Product, bool>)Delegate.Remove(_goodsManager.ProcessConsumableHanle, new Action<Product, bool>(this.OnProcessConsumable));
		GoodsManager _goodsManager2 = GoodsManager.GetInstance();
       // _goodsManager2.ProcessNonConsumableHandle = (Action<Product, bool>)Delegate.Remove(_goodsManager2.ProcessNonConsumableHandle, new Action<Product, bool>(this.OnProcessNonConsumable));
		GoodsManager _goodsManager3 = GoodsManager.GetInstance();
		//_goodsManager3.ProcessSubscriptionHandle = (Action<SubscriptionInfo, bool>)Delegate.Remove(_goodsManager3.ProcessSubscriptionHandle, new Action<SubscriptionInfo, bool>(this.OnProcessSubscription));
	}

	private void EntranceAni()
	{
		Sequence sequence = DOTween.Sequence();
		for (int i = 0; i < this.items.Count; i++)
		{
			GameObject obj = this.items[i];
			Vector3 localPosition = obj.transform.localPosition;
			localPosition.x = -1f * obj.GetComponent<RectTransform>().sizeDelta.x;
			obj.transform.localPosition = localPosition;
			obj.SetActive(false);
			Tweener t = obj.transform.DOMoveX(0f, 0.2f, false).SetDelay((float)i * 0.05f).OnStart(delegate
			{
				obj.SetActive(true);
			});
			sequence.Insert(0f, t);
		}
		sequence.OnStart(delegate
		{
			this.isPlaying = true;
		});
		sequence.OnComplete(delegate
		{
			this.isPlaying = false;
		});
	}

	//private void OnProcessConsumable(Product product, bool isInit)
	//{
	//	ToastManager.Show("TXT_NO_50005", true);
	//}

	//private void OnProcessNonConsumable(Product product, bool isInit)
	//{
	//	this.InitUI();
	//	if (!isInit)
	//	{
	//		ToastManager.Show("TXT_NO_50005", true);
	//	}
	//}

	//private void OnProcessSubscription(SubscriptionInfo info, bool isInit)
	//{
	//	this.InitUI();
	//}

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

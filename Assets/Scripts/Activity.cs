// using Assets.Scripts.Configs;
// using Assets.Scripts.GameManager;
// using DG.Tweening;
// using System;
// using System.Collections.Generic;
// using System.Runtime.CompilerServices;
// using UnityEngine;
// using UnityEngine.UI;

// public class Activity : MonoBehaviour
// {

// 	private sealed class DisplayClass
// 	{
// 		public Activity _this;

// 		public int id;

// 		internal void OnClickVideo()
// 		{
// 			this._this.OnClickButton(this.id);
// 			DialogManager.GetInstance().Close(null);
// 		}
// 	}


// 	[Serializable]
// 	private sealed class AnimClass
// 	{
// 		public static readonly Activity.AnimClass _animFunc = new Activity.AnimClass();

// 		public static Action _action;

// 		internal void _OnClickVideo()
// 		{
// 			DialogManager.GetInstance().Close(null);
// 		}
// 	}

// 	public GameObject m_prefab_dialog;

// 	[SerializeField]
// 	public List<GameObject> m_items = new List<GameObject>();

// 	public RectTransform m_video;

// 	public Text m_videoTimer;

// 	[SerializeField]
// 	public List<Color> m_colors = new List<Color>();

// 	[SerializeField]
// 	public List<Color> m_colors2 = new List<Color>();

// 	private void Start()
// 	{
// 		Debug.Log("activity Start");
// 		//this.InitUI();
// 	}

// 	private void Update()
// 	{
// 	}

// 	private void OnEnable()
// 	{
// 	}

// 	public void OnClickButton(int id)
// 	{
// 		if (!Configs.TActivitys.ContainsKey(id.ToString()))
// 		{
// 			return;
// 		}
// 		LoginData.GetInstance().SetSignInData(id, 2);
// 		TActivity tActivity = Configs.TActivitys[id.ToString()];
// 		GM.GetInstance().AddDiamond(tActivity.Item, true);
// 			Debug.Log("AddDiamond Added: " );
// 		//this.InitUI();
// 	}

// 	public void OnClickNo()
// 	{
// 		int[] _loginData = LoginData.GetInstance().GetSignInData();
// 		int num = 0;
// 		bool flag = false;
// 		int[] array = _loginData;
//         for (int i = 0; i < array.Length; i++)
// 		{
// 			int _loginIndex = array[i];
// 			num++;
// 			if (_loginIndex == 1)
// 			{
// 				flag = true;
// 				break;
// 			}
// 		}
// 		if (!flag)
// 		{
// 			return;
// 		}
// 		this.OnClickButton(num);
// 		DialogManager.GetInstance().Close(null);
// 	}

// 	public void OnClickVideo()
// 	{
// 		int[] _loginData = LoginData.GetInstance().GetSignInData();
// 		int id = 0;
// 		bool flag = false;
// 		int[] array = _loginData;
//         for (int i = 0; i < array.Length; i++)
// 		{
// 			int _dataIndex = array[i];
// 			int id2 = id + 1;
// 			id = id2;
// 			if (_dataIndex == 1)
// 			{
// 				flag = true;
// 				break;
// 			}
// 		}
// 		if (!flag)
// 		{
// 			return;
// 		}
// 		if (!Configs.TActivitys.ContainsKey(id.ToString()))
// 		{
// 			return;
// 		}
// 		TActivity tActivity = Configs.TActivitys[id.ToString()];
// 		AdsManager _adsManager = AdsManager.GetInstance();
// 		AdsManager.AdType _multiAwards = AdsManager.AdType.MultiAwards;
// 		Action _action1 = delegate
// 		{
// 			this.OnClickButton(id);
// 			DialogManager.GetInstance().Close(null);
// 		};
// 		Action _action3 = null;
// 		int _action4 = tActivity.Item;
// 		Action _action5;
// 		if ((_action5 = Activity.AnimClass._action) == null)
// 		{
//             _action5 = (Activity.AnimClass._action = new Action(Activity.AnimClass._animFunc._OnClickVideo));
// 		}
// 		_adsManager.Play(_multiAwards,_action1, _action3, _action4, _action5);
// 	}

// 	public void OnClickAds()
// 	{
// 		AdsManager.GetInstance().Play(AdsManager.AdType.Stimulate, null, null, 5, null);
// 	}

// 	public void OnClickReturn()
// 	{
// 		GlobalEventHandle.EmitClickPageButtonHandle("main", 0);
// 	}

// 	private void InitUI()
// 	{
// 		int serialLoginCount = LoginData.GetInstance().GetSerialLoginCount();
// 		int[] signInData = LoginData.GetInstance().GetSignInData();
// 		for (int i = 0; i < signInData.Length; i++)
// 		{
// 			this.ShowLogin(this.m_items[i], serialLoginCount, signInData[i], i + 1);
// 		}
// 	}

// 	private void ShowLogin(GameObject obj, int count, int status, int id)
// 	{
// 		int skinID = GM.GetInstance().SkinID;
// 		List<Color> list;
// 		if (skinID != 1)
// 		{
// 			if (skinID != 2)
// 			{
// 				list = this.m_colors;
// 			}
// 			else
// 			{
// 				list = this.m_colors2;
// 			}
// 		}
// 		else
// 		{
// 			list = this.m_colors;
// 		}
// 		TActivity tActivity = Configs.TActivitys[id.ToString()];
// 		Component _component = obj.transform.Find("img_icon/img_bg/txt_value");
// 		Transform transform = obj.transform.Find("txt_desc01");
// 		Transform transform2 = obj.transform.Find("img_item_bg");
// 		Transform transform3 = obj.transform.Find("img_finish");
// 		Transform transform4 = obj.transform.Find("button/txt");
//         _component.GetComponent<Text>().text = string.Format("{0}", tActivity.Item.ToString());
// 		transform.GetComponent<LanguageComponent>().SetText(tActivity.Desc);
// 		switch (status)
// 		{
// 		case 0:
// 			transform3.gameObject.SetActive(false);
// 			transform2.GetComponent<Image>().color = list[1];
// 			transform4.GetComponent<LanguageComponent>().SetText("TXT_NO_20001");
// 			return;
// 		case 1:
// 			transform3.gameObject.SetActive(false);
// 			transform2.GetComponent<Image>().color = list[1];
// 			transform4.GetComponent<LanguageComponent>().SetText("TXT_NO_20001");
// 			return;
// 		case 2:
// 			transform3.gameObject.SetActive(true);
// 			transform2.GetComponent<Image>().color = list[2];
// 			transform4.GetComponent<LanguageComponent>().SetText("TXT_NO_50006");
// 			return;
// 		default:
// 			return;
// 		}
// 	}

// 	private void ShowAwards(int day)
// 	{
// 		GameObject _gameObject = UnityEngine.Object.Instantiate<GameObject>(this.m_prefab_dialog);
// 		_gameObject.GetComponent<ActivityDialog>().Load(day);
// 		DialogManager.GetInstance().show(_gameObject, false);
// 	}

// 	private void OnRefreshAdsTimer(string timer, bool isWatch)
// 	{
// 		this.m_videoTimer.text = timer;
// 		if (AdsManager.GetInstance().IsWatch)
// 		{
// 			this.m_videoTimer.GetComponent<LanguageComponent>().SetText("TXT_NO_50037");
// 		}
// 		if (isWatch)
// 		{
// 			this.PlayAdsTipAni();
// 			return;
// 		}
// 		this.StopAdsTipsAni();
// 	}

// 	private void PlayAdsTipAni()
// 	{
// 		this.StopAdsTipsAni();
// 		Sequence _sequence = DOTween.Sequence();
//         _sequence.Append(this.m_video.transform.DOScale(1.2f, 1f).SetEase(Ease.Linear));
//         _sequence.Append(this.m_video.transform.DOScale(1f, 1f).SetEase(Ease.Linear));
//         _sequence.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, 20f), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
//         _sequence.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, -20f), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
//         _sequence.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, 10f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
//         _sequence.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, -10f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
//         _sequence.Append(this.m_video.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
//         _sequence.SetLoops(-1);
//         _sequence.SetTarget(this.m_video);
// 	}

// 	private void StopAdsTipsAni()
// 	{
// 		DOTween.Kill(this.m_video, false);
// 		this.m_video.localRotation = new Quaternion(0f, 0f, 0f, 0f);
// 		this.m_video.localScale = new Vector3(1f, 1f, 1f);
// 	}
// }

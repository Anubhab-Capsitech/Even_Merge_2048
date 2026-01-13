using Assets.Scripts.Configs;
using Assets.Scripts.GameManager;
using Assets.Scripts.Utils;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/*
 * This scrips will show information for UI in main scene
 */
public class MainScene : MonoBehaviour
{
	public static MainScene __instance;

    [Header("Popup Prefabs")]
    [SerializeField] private GameObject profileStatsPopupPrefab;
    [SerializeField] private GameObject leaderboardPopupPrefab;

    void Awake()
	{
		MainScene.__instance = this;
        Time.timeScale = 1f;
	}
	public EvenMergeModeSwitcher modeSwitcher;
	private Dictionary<string, GameObject> nodes = new Dictionary<string, GameObject>();

	private string curBtnType = "main";

	public GameObject txt_lv;

	public GameObject img_exp;

	public GameObject icon_gem;

	public GameObject icon_ads;

	public Text txt_ads_timer;

	public GameObject txt_diamond;

	public GameObject panel_cotent;

	public GameObject panel_handle;

	public GameObject panel_top;

	public GameObject panel_maxTop;

	public RectTransform m_btn_achiev;

	public RectTransform m_btn_task;

	public RectTransform m_btn_activity;

	public RectTransform m_btn_start;

	public RectTransform m_btn_shop;

	public RectTransform m_btn_skin;

	public RectTransform m_btn_video;

	public RectTransform m_txt_videoTimer;

	public RectTransform m_img_buttons;

	public GameObject gamelist;

	public GameObject content;

	public PageView gameview;

	public GameList gameContral;

	public Dictionary<string, Vector2> m_nodesPositions = new Dictionary<string, Vector2>();

	// public GameObject m_splash;

	public int music_sound;

	private void Start()
	{
		this.LoadData();
		this.InitUI();
		this.InitEvent();
		// this.RunSplash();
		
		// Load AdMob banner ad for the main menu scene
		if (AdMobManager.Instance != null)
		{
			AdMobManager.Instance.LoadBannerAd();
		}
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		this.DestroyEvent();
	}

	private void OnApplicationPause(bool pause)
	{
		if (pause)
		{
			GlobalTimer.GetInstance().TrackMiniTime();
			GlobalTimer.GetInstance().TrackTotalTime();
			AdsManager.GetInstance().SaveInsertTime();
			PlayerAnalytics.Instance?.EndSession();  // firebase analytics - session end on pause
			UnityEngine.Debug.Log("game pause ................");
			return;
		}
		AdsUtil.LoadRewardAds();
		AdsManager.GetInstance().PlayTransiformGroundAds();
		PlayerAnalytics.Instance?.StartSession();  // firebase analytics - resume session
		UnityEngine.Debug.Log("game continue .............");
	}

	private void OnApplicationQuit()
	{
		GlobalTimer.GetInstance().TrackMiniTime();
		GlobalTimer.GetInstance().TrackTotalTime();
		PlayerAnalytics.Instance?.EndSession();    // firebase analytics - session end on quit	
		Debug.Log("reached OnApplicationQuit");
		UnityEngine.Debug.Log("game quit .............");
	}

	public void OnClickStartGame(int id = 0)
	{
		this.gameContral.StopGuideAni();
		int num = (new int[]
		{
			2,
			1,
			3,
			0
		})[this.gameview.PageIdx];
		if (num == 0)
		{
			return;
		}
		if (num == 1)
		{
			if (PlayerPrefs.GetInt("LocalData_guide_game01", 0) == 0)
			{
				GlobalEventHandle.EmitClickPageButtonHandle("G00103", 0);
				return;
			}
		}
		else if (num == 3 && id == 0)
		{
			GlobalEventHandle.EmitClickPageButtonHandle("G003", 0);
			return;
		}
		this.gamelist.SetActive(false);
		this.content.SetActive(true);
		this.gameContral.gameObject.SetActive(true);
		this.gameContral.LoadGame(num, id, true);
		this.DoPageOutAni();
		PersistentAudioManager.Instance.PlayEffect("sound_eff_button");


	}

	public void onClickBottom(string type)
	{
		// Check if this object is destroyed (nodes dictionary might be null)
		if (this.nodes == null)
		{
			return;
		}
		
		string text = type;
		if (text.Equals("shop02"))
		{
			type = "shop";
		}
		this.gameContral.StopGuideAni();
		if (!this.curBtnType.Equals(type))
		{
			this.curBtnType = type;
			foreach (KeyValuePair<string, GameObject> current in this.nodes)
			{
				if (current.Value != null)
				{
					current.Value.SetActive(false);
				}
			}
			if (this.nodes[type] == null)
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>
				{
					{
						"shop",
						"shop"
					},
					{
						"main",
						"gamelist"
					},
					{
						"achive",
						"achive"
					},
					{
						"task",
						"task"
					},
					{
						"skin",
						"skin"
					},
					{
						"activity",
						"activity"
					},
					{
						"setting",
						"setting"
					},
					{
						"G00103",
						"G00103"
					},
					{
						"G003",
						"G003"
					}
				};
				GameObject gameObject = Resources.Load("Prefabs/" + dictionary[type]) as GameObject;
				if (gameObject != null)
				{
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(gameObject);
					gameObject2.transform.SetParent(this.panel_cotent.transform, false);
					gameObject2.name = type;
					this.nodes[type] = gameObject2;
				}
			}
			else
			{
				this.nodes[type].SetActive(true);
			}
			if (!type.Equals("main"))
			{
				this.DoPageOutAni();
			}
			else if (GM.GetInstance().GameId == 0)
			{
				this.DoPageInAni();
			}
			if (text.Equals("shop02"))
			{
				this.nodes[type].GetComponent<Shop>().Type = 2;
			}
			else if (text.Equals("shop"))
			{
				this.nodes[type].GetComponent<Shop>().Type = 1;
			}
			PersistentAudioManager.Instance.PlayEffect("sound_eff_button");
			return;
		}
		if (!type.Equals("main"))
		{
			return;
		}
		// Check if nodes dictionary and the specific node exist and are not destroyed
		if (this.nodes == null || !this.nodes.ContainsKey(type) || this.nodes[type] == null)
		{
			return;
		}
		if (!this.nodes[type].activeSelf)
		{
			return;
		}
		GlobalEventHandle.EmitDoGoHome();
		this.DoPageInAni();
	}

	public void OnClickSetting()
	{
		GameObject obj = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("Prefabs/setting") as GameObject);
		DialogManager.GetInstance().show(obj, false);
        PersistentAudioManager.Instance.PlayEffect("sound_eff_popup");
        
        // Pause the game when settings dialog is opened (for 2D game)
        Time.timeScale = 0f;
        if (GM.GetInstance().GameId == 2 && G2BoardGenerator.GetInstance() != null)
        {
            G2BoardGenerator.GetInstance().IsPuase = true;
        }
    }

    public void OnClickProfile()
    {
        if (profileStatsPopupPrefab == null)
        {
            Debug.LogError("ProfileStatsPopup prefab is not assigned in the inspector!");
            return;
        }

        GameObject obj = UnityEngine.Object.Instantiate<GameObject>(profileStatsPopupPrefab);
        DialogManager.GetInstance().show(obj, false);
        PersistentAudioManager.Instance.PlayEffect("sound_eff_popup");

        // Pause the game when profile dialog is opened (for 2D game)
        Time.timeScale = 0f;
        if (GM.GetInstance().GameId == 2 && G2BoardGenerator.GetInstance() != null)
        {
            G2BoardGenerator.GetInstance().IsPuase = true;
        }
    }

    public void OnClickLeaderboard()
    {
        if (leaderboardPopupPrefab == null)
        {
            Debug.LogError("LeaderboardPopup prefab is not assigned in the inspector!");
            return;
        }

        GameObject obj = UnityEngine.Object.Instantiate<GameObject>(leaderboardPopupPrefab);
        DialogManager.GetInstance().show(obj, false);

        // Move the popup up a bit as requested
        RectTransform rt = obj.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition += new Vector2(0, 200f);
        }
        PersistentAudioManager.Instance.PlayEffect("sound_eff_popup");

        // Pause the game when leaderboard dialog is opened (for 2D game)
        Time.timeScale = 0f;
        if (GM.GetInstance().GameId == 2 && G2BoardGenerator.GetInstance() != null)
        {
            G2BoardGenerator.GetInstance().IsPuase = true;
        }
    }

    public void OnClickExpButton(int value)
	{
		PlayerPrefs.DeleteAll();
	}

	public void onClickAds()
	{
		AdsManager.GetInstance().Play(AdsManager.AdType.Stimulate, null, null, 5, null);
	}

	public void OnClickFacebook()
	{


	}

	public void OnClickMail()
	{

	}

	public void ClearLocalData()
	{
		PlayerPrefs.DeleteAll();
		GM.GetInstance().Init();
		this.InitUI();
	}

	void OnEnable()
	{
		// Ensure the toggle is visible when this menu scene becomes active
		if (modeSwitcher != null)
		{
			modeSwitcher.ShowToggle(true);
			Debug.Log("[MainScene] OnEnable() called - Shown toggle", this);
		}
		else
		{
			Debug.LogWarning("[MainScene] modeSwitcher not assigned in inspector", this);
		}
	}


	private void LoadData()
	{
		this.nodes.Add("shop", null);
		this.nodes.Add("main", base.transform.Find("panel_content/gamelist").gameObject);
		this.nodes.Add("achive", null);
		this.nodes.Add("task", null);
		this.nodes.Add("skin", null);
		// this.nodes.Add("activity", null);
		this.nodes.Add("setting", null);
		this.nodes.Add("G00103", null);
		this.nodes.Add("G003", null);
		this.m_nodesPositions.Add("buttons", this.m_img_buttons.anchoredPosition);
		this.m_nodesPositions.Add("shop", this.m_btn_shop.anchoredPosition);
		this.m_nodesPositions.Add("start", this.m_btn_start.anchoredPosition);
		this.m_nodesPositions.Add("skin", this.m_btn_skin.anchoredPosition);
		UnityEngine.Debug.Log(this.m_btn_skin.anchoredPosition);
		UnityEngine.Debug.Log(this.m_btn_shop.anchoredPosition);
		this.m_nodesPositions.Add("task", this.m_btn_task.anchoredPosition);
		this.m_nodesPositions.Add("activity", this.m_btn_activity.anchoredPosition);
		this.m_nodesPositions.Add("achiev", this.m_btn_achiev.anchoredPosition);
		this.m_nodesPositions.Add("video", this.m_btn_video.anchoredPosition);
		this.m_nodesPositions.Add("videoTime", this.m_txt_videoTimer.anchoredPosition);
	}

	private void InitUI()
	{
		if (AdsManager.GetInstance().IsWatch)
		{
			this.txt_ads_timer.GetComponent<LanguageComponent>().SetText("TXT_NO_50037");
		}
		this.LoadDiamondUI();
		this.LoadExpUI();
		this.PlayGENTipAni();
		this.PlayAdsTipAni();
		if (GM.GetInstance().isSavedGame())
		{
			this.HideOther();
		}
	}

	private void InitEvent()
	{
		try
		{
			GlobalEventHandle.GetDiamondHandle += new Action<int, bool>(this.OnGetDiamond);
			GlobalEventHandle.ConsumeDiamondHandle += new Action<int>(this.OnConsumeGEM);
			GlobalEventHandle.AddExpHandle += new Action<bool>(this.OnAddExp);
			GlobalEventHandle.DoClickBottom += new Action(this.OnDoClickBottomButton);
			GlobalEventHandle.OnClickPageButtonHandle += new Action<string, int>(this.OnClickPageButton);
			GlobalEventHandle.DoUseProps += new Action<bool, List<GameObject>>(this.OnDoUseProps);
			GlobalEventHandle.AdsHandle += new Action<string, bool>(this.OnRefreshAdsTimer);
			GoodsManager.GetInstance().ShowSubscriptionHanle += new Action<int, int>(this.ShowSubscriptionAwards);

			if (this.gameview != null)
			{
				this.gameview.OnClickHandle = new Action<int>(this.OnClickStartGame);
			}
			else
			{
				Debug.LogWarning("MainScene.InitEvent: gameview is NULL");
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("MainScene.InitEvent encountered exception: " + ex.ToString());
		}
	}

	private void DestroyEvent()
	{
		// Unsubscribe from all events to prevent calling destroyed objects
		GlobalEventHandle.GetDiamondHandle -= new Action<int, bool>(this.OnGetDiamond);
		GlobalEventHandle.ConsumeDiamondHandle -= new Action<int>(this.OnConsumeGEM);
		GlobalEventHandle.AddExpHandle -= new Action<bool>(this.OnAddExp);
		GlobalEventHandle.DoClickBottom -= new Action(this.OnDoClickBottomButton);
		GlobalEventHandle.OnClickPageButtonHandle -= new Action<string, int>(this.OnClickPageButton);
		GlobalEventHandle.DoUseProps -= new Action<bool, List<GameObject>>(this.OnDoUseProps);
		GlobalEventHandle.AdsHandle -= new Action<string, bool>(this.OnRefreshAdsTimer);
		GoodsManager.GetInstance().ShowSubscriptionHanle -= new Action<int, int>(this.ShowSubscriptionAwards);
	}

	public void LoadDiamondUI()
	{
		this.txt_diamond.GetComponent<OverlayNumber>().SetStartNumber(GM.GetInstance().Diamond);
	}

	private void LoadExpUI()
	{
        float currentExp = GM.GetInstance().Exp;
        this.txt_lv.GetComponent<TextMeshProUGUI>().text = GM.GetInstance().GetTierName(currentExp);
        
        // Fill amount based on Tier Progress
        this.img_exp.GetComponent<Image>().fillAmount = GM.GetInstance().GetTierProgress(currentExp);
	}

	private void OnGetDiamond(int num, bool isPlayAni)
	{
		// Check if required components are destroyed/null (object might be destroyed when scene changes)
		if (this.txt_diamond == null)
		{
			return;
		}
		
		if (isPlayAni)
		{
			int toNum = GM.GetInstance().Diamond;
			this.PlayGEMAni(delegate
			{
				if (this.txt_diamond != null)
				{
					this.txt_diamond.GetComponent<OverlayNumber>().setNum(toNum);
				}
			});
		}
		else
		{
			this.txt_diamond.GetComponent<OverlayNumber>().setNum(GM.GetInstance().Diamond);
		}
		//PersistentAudioManager.Instance.PlayEffect("sound_eff_coin");
	}

	private void OnConsumeGEM(int num)
	{
		// Check if required components are destroyed/null (object might be destroyed when scene changes)
		if (this.txt_diamond == null)
		{
			return;
		}
		
		this.txt_diamond.GetComponent<OverlayNumber>().setNum(GM.GetInstance().Diamond);
		//PersistentAudioManager.Instance.PlayEffect("sound_eff_coin");
	}

	private void OnAddExp(bool isLevelUp)
	{
		// Check if required components are destroyed/null (object might be destroyed when scene changes)
		if (this.txt_lv == null || this.img_exp == null)
		{
			return;
		}
		
		this.LoadExpUI();
		if (!isLevelUp)
		{
			return;
		}
		if (GM.GetInstance().GameId == 2)
		{
			G2BoardGenerator.GetInstance().IsPuase = true;
		}
		this.ShowLevelUp();
	}

	private void OnDoClickBottomButton()
	{
		this.onClickBottom("main");
	}

	private void OnClickPageButton(string name, int value)
	{
		// Check if this object is destroyed (nodes dictionary might be null)
		if (this.nodes == null)
		{
			return;
		}
		
		if (name.Equals("Game01"))
		{
			this.curBtnType = "main";
			this.OnClickStartGame(1);
			return;
		}
		if (name.Equals("Game03"))
		{
			this.curBtnType = "main";
			this.OnClickStartGame(value);
			return;
		}
		this.onClickBottom(name);
	}

	private void OnDoUseProps(bool isDel, List<GameObject> objs)
	{
		// Check if required components are destroyed/null (object might be destroyed when scene changes)
		if (this.panel_handle == null)
		{
			return;
		}
		
		this.panel_handle.SetActive(!isDel);
		if (isDel)
		{
			return;
		}
		if (objs != null)
		{
			using (List<GameObject>.Enumerator enumerator = objs.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current != null)
					{
						enumerator.Current.transform.SetParent(this.panel_handle.transform, true);
					}
				}
			}
		}
		Transform transform = this.panel_handle.transform.Find("txt_tips");
		if (transform == null)
		{
			return;
		}
		Text component = transform.GetComponent<Text>();
		if (component == null)
		{
			return;
		}
		DOTween.Kill(transform, false);
		Sequence expr_86 = DOTween.Sequence();
		expr_86.Append(component.DOFade(1f, 0f));
		expr_86.Append(component.DOFade(0.5f, 1f));
		expr_86.Append(component.DOFade(1f, 1f));
		expr_86.SetLoops(-1);
		expr_86.SetTarget(transform);
		string[] array = new string[]
		{
			"TXT_NO_50039",
			"TXT_NO_50040",
			"TXT_NO_50041"
		};
		// if (GM.GetInstance().GameId == 1)
		// {
		// 	component.GetComponent<LanguageComponent>().SetText(array[Game1DataLoader.GetInstance().CurPropId - 1]);
		// }
	}

	private void OnRefreshAdsTimer(string timer, bool isWatch)
	{
		// Check if required components are destroyed/null (object might be destroyed when scene changes)
		if (this.txt_ads_timer == null)
		{
			return;
		}
		
		this.txt_ads_timer.text = timer;
		if (AdsManager.GetInstance().IsWatch)
		{
			this.txt_ads_timer.GetComponent<LanguageComponent>().SetText("TXT_NO_50037");
		}
		if (isWatch)
		{
			this.PlayAdsTipAni();
			return;
		}
		this.StopAdsTipsAni();
	}

	private void ShowSubscriptionAwards(int day, int value)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("Prefabs/subscriptionAwards") as GameObject);
		gameObject.GetComponent<SubscriptionDialog>().Load(day, value);
		DialogManager.GetInstance().show(gameObject, false);
		PersistentAudioManager.Instance.PlayEffect("sound_eff_button");
	}

	private void PlayGEMAni(Action callback)
	{
		GameObject asset = Resources.Load("Prefabs/effect/eff_gem") as GameObject;
		System.Random rd = new System.Random();
		Sequence arg_50_0 = DOTween.Sequence();
		Sequence actions = DOTween.Sequence();
		arg_50_0.AppendCallback(delegate
		{
			GameObject prefab = UnityEngine.Object.Instantiate<GameObject>(asset);
			prefab.transform.SetParent(this.panel_maxTop.transform, false);
			prefab.transform.localPosition = new Vector3((float)rd.Next(-80, 80), (float)rd.Next(0, 80), 0f);
			prefab.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
			Vector3 position = this.icon_gem.transform.position;
			Tweener t = prefab.transform.DOMove(position, 0.8f, false).SetEase(Ease.InBack).OnComplete(delegate
			{
				UnityEngine.Object.Destroy(prefab);
			});
			actions.Insert(0f, t);
		}).AppendInterval(0.01f).SetLoops(10);
		actions.OnComplete(delegate
		{
			Action expr_06 = callback;
			if (expr_06 == null)
			{
				return;
			}
			expr_06();
		});
	}

	private void PlayGENTipAni()
	{
		Sequence expr_05 = DOTween.Sequence();
		expr_05.Append(this.icon_gem.transform.DOScale(1.1f, 1f).SetEase(Ease.Linear));
		expr_05.Append(this.icon_gem.transform.DOScale(1f, 1f).SetEase(Ease.Linear));
		expr_05.SetLoops(-1);
	}

	private void PlayAdsTipAni()
	{
		this.StopAdsTipsAni();
		Sequence expr_0B = DOTween.Sequence();
		expr_0B.Append(this.icon_ads.transform.DOScale(1.2f, 1f).SetEase(Ease.Linear));
		expr_0B.Append(this.icon_ads.transform.DOScale(1f, 1f).SetEase(Ease.Linear));
		expr_0B.Append(this.icon_ads.transform.DOLocalRotate(new Vector3(0f, 0f, 20f), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
		expr_0B.Append(this.icon_ads.transform.DOLocalRotate(new Vector3(0f, 0f, -20f), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
		expr_0B.Append(this.icon_ads.transform.DOLocalRotate(new Vector3(0f, 0f, 10f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
		expr_0B.Append(this.icon_ads.transform.DOLocalRotate(new Vector3(0f, 0f, -10f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
		expr_0B.Append(this.icon_ads.transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
		expr_0B.SetLoops(-1);
		expr_0B.SetTarget(this.icon_ads);
	}

	private void StopAdsTipsAni()
	{
		this.icon_ads.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
		this.icon_ads.transform.localScale = new Vector3(1f, 1f, 1f);
		DOTween.Kill(this.icon_ads, false);
	}

	private void ShowLevelUp()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("Prefabs/LevelUp") as GameObject);
		DialogManager.GetInstance().show(gameObject, false);
		gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("Prefabs/effect/eff_levelup") as GameObject);
		gameObject.transform.SetParent(base.transform, false);
		UnityEngine.Object.Destroy(gameObject, 5f);
		PersistentAudioManager.Instance.PlayEffect("sound_eff_levelUp");
	}

	public void DoPageOutAni()
	{
		float num = 1000f;
		Vector2 vector = new Vector2(this.m_nodesPositions["buttons"].x - 500f, this.m_nodesPositions["buttons"].y);
		float duration = Math.Abs(vector.x - this.m_nodesPositions["buttons"].x) / num;
		duration = 0f;
		DOTween.Kill(this.m_img_buttons, false);
		DOTween.To(() => this.m_img_buttons.anchoredPosition, delegate (Vector2 xx)
		{
			this.m_img_buttons.anchoredPosition = xx;
		}, vector, duration).SetTarget(this.m_img_buttons);
		vector = new Vector2(this.m_nodesPositions["video"].x + 300f, this.m_nodesPositions["video"].y);
		duration = Math.Abs(vector.x - this.m_nodesPositions["video"].x) / num;
		duration = 0f;
		DOTween.Kill(this.m_btn_video, false);
		DOTween.To(() => this.m_btn_video.anchoredPosition, delegate (Vector2 xx)
		{
			this.m_btn_video.anchoredPosition = xx;
		}, vector, duration).SetTarget(this.m_btn_video);
		vector = new Vector2(this.m_nodesPositions["videoTime"].x + 300f, this.m_nodesPositions["videoTime"].y);
		duration = Math.Abs(vector.x - this.m_nodesPositions["videoTime"].x) / num;
		duration = 0f;
		DOTween.Kill(this.m_txt_videoTimer, false);
		DOTween.To(() => this.m_txt_videoTimer.anchoredPosition, delegate (Vector2 xx)
		{
			this.m_txt_videoTimer.anchoredPosition = xx;
		}, vector, duration).SetTarget(this.m_txt_videoTimer);
		vector = new Vector2(this.m_nodesPositions["skin"].x, this.m_nodesPositions["skin"].y - 300f);
		duration = Math.Abs(vector.y - this.m_nodesPositions["skin"].y) / num;
		duration = 0f;
		DOTween.Kill(this.m_btn_skin, false);
		DOTween.To(() => this.m_btn_skin.anchoredPosition, delegate (Vector2 xx)
		{
			this.m_btn_skin.anchoredPosition = xx;
		}, vector, duration).SetTarget(this.m_btn_skin);
		vector = new Vector2(this.m_nodesPositions["shop"].x, this.m_nodesPositions["shop"].y - 300f);
		duration = Math.Abs(vector.y - this.m_nodesPositions["shop"].y) / num;
		duration = 0f;
		DOTween.Kill(this.m_btn_shop, false);
		DOTween.To(() => this.m_btn_shop.anchoredPosition, delegate (Vector2 xx)
		{
			this.m_btn_shop.anchoredPosition = xx;
		}, vector, duration).SetTarget(this.m_btn_shop);
	}

	public void DoPageInAni()
	{
		float num = 1000f;
		float duration = Math.Abs(this.m_nodesPositions["buttons"].x - this.m_img_buttons.anchoredPosition.x) / num;
		DOTween.Kill(this.m_img_buttons, false);
		DOTween.To(() => this.m_img_buttons.anchoredPosition, delegate (Vector2 xx)
		{
			this.m_img_buttons.anchoredPosition = xx;
		}, this.m_nodesPositions["buttons"], duration).SetTarget(this.m_img_buttons);
		duration = Math.Abs(this.m_nodesPositions["video"].x - this.m_btn_video.anchoredPosition.x) / num;
		DOTween.Kill(this.m_btn_video, false);
		DOTween.To(() => this.m_btn_video.anchoredPosition, delegate (Vector2 xx)
		{
			this.m_btn_video.anchoredPosition = xx;
		}, this.m_nodesPositions["video"], duration).SetTarget(this.m_btn_video);
		duration = Math.Abs(this.m_nodesPositions["videoTime"].x - this.m_txt_videoTimer.anchoredPosition.x) / num;
		DOTween.Kill(this.m_txt_videoTimer, false);
		DOTween.To(() => this.m_txt_videoTimer.anchoredPosition, delegate (Vector2 xx)
		{
			this.m_txt_videoTimer.anchoredPosition = xx;
		}, this.m_nodesPositions["videoTime"], duration).SetTarget(this.m_txt_videoTimer);
		duration = Math.Abs(this.m_nodesPositions["skin"].y - this.m_btn_skin.anchoredPosition.y) / num;
		DOTween.Kill(this.m_btn_skin, false);
		DOTween.To(() => this.m_btn_skin.anchoredPosition, delegate (Vector2 xx)
		{
			this.m_btn_skin.anchoredPosition = xx;
		}, this.m_nodesPositions["skin"], duration).SetTarget(this.m_btn_skin);
		duration = Math.Abs(this.m_nodesPositions["shop"].y - this.m_btn_shop.anchoredPosition.y) / num;
		DOTween.Kill(this.m_btn_shop, false);
		DOTween.To(() => this.m_btn_shop.anchoredPosition, delegate (Vector2 xx)
		{
			this.m_btn_shop.anchoredPosition = xx;
		}, this.m_nodesPositions["shop"], duration).SetTarget(this.m_btn_shop);
	}

	public void HideOther()
	{
		Vector2 anchoredPosition = new Vector2(this.m_nodesPositions["buttons"].x - 500f, this.m_nodesPositions["buttons"].y);
		this.m_img_buttons.anchoredPosition = anchoredPosition;
		anchoredPosition = new Vector2(this.m_nodesPositions["video"].x + 300f, this.m_nodesPositions["video"].y);
		this.m_btn_video.anchoredPosition = anchoredPosition;
		anchoredPosition = new Vector2(this.m_nodesPositions["videoTime"].x + 300f, this.m_nodesPositions["videoTime"].y);
		this.m_txt_videoTimer.anchoredPosition = anchoredPosition;
		anchoredPosition = new Vector2(this.m_nodesPositions["skin"].x, this.m_nodesPositions["skin"].y - 300f);
		this.m_btn_skin.anchoredPosition = anchoredPosition;
		anchoredPosition = new Vector2(this.m_nodesPositions["shop"].x, this.m_nodesPositions["shop"].y - 300f);
		this.m_btn_shop.anchoredPosition = anchoredPosition;
	}

	// private void RunSplash()
	// {
	// 	PersistentAudioManager.Instance.audio_bg.mute = true;
	// 	this.m_splash.SetActive(true);
	// 	this.m_splash.GetComponent<Image>().DOFade(0f, 0.5f).OnComplete(delegate
	// 	{
	// 		PersistentAudioManager.Instance.audio_bg.mute = false;
	// 		this.m_splash.SetActive(false);
	// 	});
	// }
}

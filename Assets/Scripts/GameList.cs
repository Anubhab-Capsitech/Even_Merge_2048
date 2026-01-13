using Assets.Scripts.Configs;
using Assets.Scripts.GameManager;
using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameList : MonoBehaviour
{

    // assign these in Inspector
    public EvenMergeModeSwitcher modeSwitcher; // assign Canvas -> EvenMergeModeSwitcher component
    public string scene3DName = "scene";       // Build Settings name for 3D gameplay


    // public Animator LevelBarAnimator;
    public GameObject Logo;
    public Animator BGanimator;

    // ADD THESE UI REFERENCES - Assign in Inspector
    public GameObject settingsButton;   // Assign your Settings button here
    public GameObject pauseButton;      // Assign your Pause button here
    public GameObject profileButton;    // Assign your Profile button here
    public GameObject leaderboardButton; // Assign your Leaderboard button here

    [Serializable]

    private sealed class __c
    {
        public static readonly GameList.__c __9 = new GameList.__c();

        public static Action __9__20_1;

        internal void _Update_b__20_1()
        {
            Utils.ExitGame();
        }
    }

    private sealed class __c__DisplayClass29_0
    {
        public GameList __4__this;

        public Vector3 savePos;

        // internal void _PlayGuideAni_b__0()
        // {
        //     this.__4__this.m_img_finger.localPosition = this.savePos;
        // }
    }

    private sealed class __c__DisplayClass36_0
    {
        public GameList __4__this;

        public Transform img_box;

        internal void _PlayRecordAni_b__0()
        {
            this.__4__this.m_node001.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        }

        internal void _PlayRecordAni_b__1()
        {
            this.__4__this.m_node002.transform.localPosition = this.__4__this.m_savePage2Block;
            this.__4__this.m_node002.GetComponent<Image>().color = Color.white;
            this.img_box.GetComponent<Image>().color = Color.white;
            this.__4__this.m_maxNumber03.GetComponent<G2Block>().FadeIn();
            this.__4__this.m_maxNumber03.transform.localScale = Vector3.one;
            this.__4__this.m_maxNumber02.GetComponent<G2Block>().setNum(this.__4__this.m_record_number);
            this.__4__this.m_maxNumber03.GetComponent<G2Block>().setNum(this.__4__this.m_record_number);
            this.__4__this.m_arrow.GetComponent<Image>().color = Color.white;
        }

        internal void _PlayRecordAni_b__2()
        {
            this.__4__this.m_node002.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            this.img_box.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            this.__4__this.m_maxNumber03.GetComponent<G2Block>().HideNumber();
            this.__4__this.m_maxNumber02.GetComponent<G2Block>().setNum(this.__4__this.m_record_number * 2);
            this.__4__this.m_maxNumber03.GetComponent<G2Block>().setNum(this.__4__this.m_record_number * 2);
        }
    }

    public GameObject gamelist;

    public GameObject content;

    public Dictionary<int, GameObject> m_gameDict = new Dictionary<int, GameObject>();

    public RectTransform m_video;

    public Text m_videoTimer;

    public RectTransform m_btn_return;
    public GameObject[] MainMenuItems;

    // public G1Block m_maxNumber01;

    public G2Block m_maxNumber02;

    public G2Block m_maxNumber03;

    public Text m_txt_maxScore01;

    public Text m_txt_maxScore02;

    public Text m_txt_maxScore03;

    public GameObject m_node001;

    public GameObject m_node002;

    public GameObject m_arrow;

    // public Transform m_img_finger;

    public Transform m_img_arrow;

    private int m_record_number = 64;

    private Vector3 m_savePage2Block = Vector3.zero;

    public CloudAnim cloudAnim1;

    private void Start()
    {
        // Auto-discover Leaderboard Button if not assigned
        if (leaderboardButton == null)
        {
            // Find all buttons in the scene, including inactive ones
            Button[] sceneButtons = Resources.FindObjectsOfTypeAll<Button>();
            foreach (var btn in sceneButtons)
            {
                // Filter out buttons that are assets (not in scene)
                if (btn.gameObject.scene.rootCount == 0) continue;

                for (int i = 0; i < btn.onClick.GetPersistentEventCount(); i++)
                {
                    if (btn.onClick.GetPersistentMethodName(i) == "OnClickLeaderboard")
                    {
                        leaderboardButton = btn.gameObject;
                        Debug.Log("GameList: Auto-assigned Leaderboard Button found: " + btn.name);
                        break;
                    }
                }
                if (leaderboardButton != null) break;
            }
        }

        this.m_savePage2Block = this.m_node002.transform.localPosition;
        this.Init();
        cloudAnim1.StartMovement(); //starts cloud animation on main menu
        this.PlayRecordAni();
        this.InitEvent();
        if (GM.GetInstance().isFirstGame())
        {
            GM.GetInstance().SetFristGame();
            base.transform.Find("list/view").GetComponent<PageView>().OnDragHandle = delegate
            {
                this.StopGuideAni();
            };
            this.PlayGuideAni();
        }
    }

    private void Update()
    {
        Utils.BackListener(base.gameObject, delegate
        {
            if (GM.GetInstance().GameId != 0)
            {
                this.OnClickReturn();
                return;
            }

            return;
        });
    }

    public static GameList Instance;
    void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        this.PlayRecordAni();
        cloudAnim1.StartMovement(); //starts cloud animation on 
    }

    private void OnDestroy()
    {
        // Unsubscribe from all events to prevent calling destroyed objects
        GlobalEventHandle.DoGoHome -= new Action(this.Init);
        GlobalEventHandle.OnRefreshAchiveHandle -= new Action<int>(this.RefreshRecord);
        GlobalEventHandle.OnRefreshMaxScoreHandle -= new Action<string[]>(this.RefreshMaxScore);
        GlobalEventHandle.AdsHandle -= new Action<string, bool>(this.OnRefreshAdsTimer);
    }

    public void OnClickStartGame(int id)
    {

        // Hide toggle so it doesn't overlap gameplay
        modeSwitcher?.ShowToggle(false);

        // If 3D selected -> load 3D gameplay scene
        if (modeSwitcher != null && !modeSwitcher.Is2D())
        {
            // FIX: Rely on the actual save file ("SavedCubes") rather than the global GameId.
            // This allows resuming 3D game even if the player switched to 2D in the meantime.
            bool hasSavedData = PlayerPrefs.HasKey("SavedCubes");
            
            // CRITICAL FIX: If the previous game was a Game Over, do NOT resume.
            // Force a fresh start if the player is hitting "Start" from the main menu.
            bool isGameOver = PlayerPrefs.GetInt("LastGameOverState", 0) != 0;
            
            bool shouldResume = hasSavedData && !isGameOver; 
            
            if (!shouldResume)
            {
                // When starting a NEW game from the main menu, clear any saved state
                // This ensures a fresh start instead of resuming from a previous game over state
                // Clear PlayerPrefs keys directly since instances may not exist in main menu scene
                PlayerPrefs.DeleteKey("SavedCubes");
                PlayerPrefs.DeleteKey("PatternAlreadySpawned");
                PlayerPrefs.DeleteKey("LastGameOverState");
                
                // Reset current score to 0, but preserve high score
                PlayerPrefs.SetInt("currentHeightScore", 0);
                PlayerPrefs.DeleteKey("popupShown"); // Reset popup shown flag
                
                // Set GameId to 3 to mark that we're starting/resuming 3D game
                GM.GetInstance().SetSavedGameID(3);
            }
            else 
            {
                 // If resuming, ensure the GameId is set back to 3 
                 // (in case it was 2 from playing the other game)
                 GM.GetInstance().SetSavedGameID(3);
            }
            
            PlayerPrefs.Save();
            
            SceneManager.LoadScene(scene3DName);
            return;
        }
        PersistentAudioManager.Instance.PlayEffect("sound_eff_button");
        this.gamelist.SetActive(false);
        cloudAnim1.StopMovement();     //stops cloud animation when game starts
                                       //this.content.SetActive(true);
        if (this.content != null) this.content.SetActive(true);
        // Ensure we only load G2 – adapt if your ids differ
        int gameId = 2;
        this.LoadGame(gameId, 0, true);
        UiSwitching(true);
        GM.GetInstance().GameId = gameId;
    }

    // MODIFIED METHOD - Keep settings and pause buttons active during gameplay
    public void UiSwitching(bool inGame)
    {
        if (inGame)
        {
            foreach (GameObject obj in MainMenuItems)
            {
                // Skip deactivating settings and pause buttons
                if (obj == settingsButton || obj == pauseButton)
                {
                    continue;
                }

                obj.SetActive(false);
            }

            // Hide ProfileButton when 2D game starts
            if (profileButton != null)
            {
                profileButton.SetActive(false);
            }

            // Hide LeaderboardButton when 2D game starts
            if (leaderboardButton != null)
            {
                leaderboardButton.SetActive(false);
            }

            // Ensure both buttons are active during gameplay
            if (settingsButton != null)
            {
                settingsButton.SetActive(true);
            }
            if (pauseButton != null)
            {
                pauseButton.SetActive(true);
            }

            BGanimator.enabled = false;
            cloudAnim1.StopMovement();  //stop clouds
            Logo.SetActive(false);
        }
        else
        {
            foreach (GameObject obj in MainMenuItems)
            {
                obj.SetActive(true);
            }

            // Show ProfileButton when returning to main menu
            if (profileButton != null)
            {
                profileButton.SetActive(true);
            }

            // Show LeaderboardButton when returning to main menu
            if (leaderboardButton != null)
            {
                leaderboardButton.SetActive(true);
            }

            // Hide pause button when returning to main menu
            if (pauseButton != null)
            {
                pauseButton.SetActive(false);
            }

            BGanimator.enabled = true;
            cloudAnim1.StartMovement(); //start clouds
            Logo.SetActive(true);
        }
    }


    public void BarAnimation(bool inGame)
    {
        if (inGame)
        {
            // LevelBarAnimator.SetTrigger("GameStart");
            BGanimator.enabled = false;
            Logo.SetActive(false);

        }
        else
        {
            // LevelBarAnimator.SetTrigger("GameEnd");
            BGanimator.enabled = true;
            Logo.SetActive(true);
        }

    }
    public void OnClickReturn()
    {
        Debug.Log($"[GameList.OnClickReturn] called. current GameId={GM.GetInstance().GameId}", this);

        //PersistentAudioManager.Instance.PlayClickSound();
        PersistentAudioManager.Instance.PlayEffect("sound_eff_popup");

        if (GM.GetInstance().GameId == 0)
        {
            this.HideTopBtn();
            GlobalEventHandle.EmitDoGoHome();
            GlobalEventHandle.EmitClickPageButtonHandle("main", 0);

            // only show toggle when we're actually at the main menu
            modeSwitcher?.ShowToggle(true);
            Debug.Log("[GameList.OnClickReturn] At main menu - Showed toggle", this);

            return;
        }

        switch (GM.GetInstance().GameId)
        {
            case 2:
                {
                    Debug.Log("[GameList.OnClickReturn] handling GameId=2 (G2) return handler", this);

                    Action<GameList> handler = G2BoardGenerator.GetInstance().OnClickReturnHandle;
                    if (handler == null)
                    {
                        Debug.Log("[GameList.OnClickReturn] G2 handler is NULL. Not showing toggle.", this);
                        return;
                    }

                    // call the G2-specific return handler
                    handler(this);

                    // After the handler runs the game state might change.
                    // Show toggle only if the global GameId now indicates main menu (0).
                    int nowId = GM.GetInstance().GameId;
                    Debug.Log($"[GameList.OnClickReturn] After G2 handler, GameId={nowId}", this);
                    if (nowId == 0)
                    {
                        modeSwitcher?.ShowToggle(true);
                        Debug.Log("[GameList.OnClickReturn] Handler resulted in GameId==0 -> Showed toggle", this);
                    }
                    else
                    {
                        Debug.Log("[GameList.OnClickReturn] Handler did not return to main menu -> Not showing toggle", this);
                    }

                    return;
                }
            default:
                return;
        }
    }

    // below is the orginal OnClickReturn code 
    // public void OnClickReturn()
    // {
    // 	if (GM.GetInstance().GameId == 0)
    // 	{
    // 		this.HideTopBtn();
    // 		GlobalEventHandle.EmitDoGoHome();
    // 		GlobalEventHandle.EmitClickPageButtonHandle("main", 0);
    // 		modeSwitcher?.ShowToggle(true);
    // 		return;
    // 	}
    // 	switch (GM.GetInstance().GameId)
    // 	{
    // 		// case 1:
    // 		// 	{
    // 		// 		Action<GameList> expr_36 = Game1DataLoader.GetInstance().OnClickReturnHandle;
    // 		// 		if (expr_36 == null)
    // 		// 		{
    // 		// 			return;
    // 		// 		}
    // 		// 		expr_36(this);
    // 		// 		return;
    // 		// 	}
    // 		case 2:
    // 			{
    // 				Action<GameList> expr_4C = G2BoardGenerator.GetInstance().OnClickReturnHandle;
    // 				if (expr_4C == null)
    // 				{

    // 					return;
    // 				}
    // 				expr_4C(this);

    // 				return;
    // 			}
    // 		// case 3:
    // 		// 	{
    // 		// 		Action<GameList> expr_62 = G3BoardGenerator.GetInstance().OnClickReturnHandle;
    // 		// 		if (expr_62 == null)
    // 		// 		{
    // 		// 			return;
    // 		// 		}
    // 		// 		expr_62(this);
    // 		// 		return;
    // 		// 	}
    // 		default:
    // 			return;
    // 	}
    // }

    public void LoadGame(int id, int value = 0, bool isPageIn = true)
    {
        this.PlayRecordAni();

        if (AdsManager.GetInstance().IsWatch)
        {
            this.m_videoTimer.GetComponent<LanguageComponent>().SetText("TXT_NO_50037");
        }
        if (AdsManager.GetInstance().IsWatch)
        {
            this.PlayAdsTipAni();
        }
        if (this.content.activeSelf)
        {
            this.ShowTopBtn();
        }
        else
        {
            this.HideTopBtn();
        }
        if (id == 0)
        {
            return;
        }
        foreach (KeyValuePair<int, GameObject> current in this.m_gameDict)
        {
            current.Value.SetActive(false);
        }
        if (this.m_gameDict.ContainsKey(id))
        {
            this.m_gameDict[id].SetActive(true);
            switch (id)
            {
                // case 1:
                // 	Game1DataLoader.GetInstance().StartNewGame();
                // 	break;
                case 2:
                    // Only start a new game if we don't have a saved one for this ID
                    if (GM.GetInstance().isSavedGame() && GM.GetInstance().GetSavedGameID() == 2)
                    {
                        Debug.Log("Resuming saved game instead of starting new one.");
                        // RESUME FIX: Restore game loop but keep it PAUSED.
                        // IsStart = true allows the Update loop to eventually run.
                        // IsPuase = true keeps it frozen until the user explicitly unpauses (e.g. via Pause Menu).
                        G2BoardGenerator.GetInstance().IsStart = true;
                        G2BoardGenerator.GetInstance().IsPuase = true;
                        G2BoardGenerator.GetInstance().IsPlaying = false;
                    }
                    else
                    {
                        G2BoardGenerator.GetInstance().StartNewGame();
                    }
                    break;
                    // case 3:
                    // 	G3BoardGenerator.GetInstance().StartNewGame(value);
                    // 	break;
            }
        }
        else
        {
            Dictionary<int, string> dictionary = new Dictionary<int, string>
            {
                {
                    1,
                    "Prefabs/G001"
                },
                {
                    2,
                    "Prefabs/G002"
                },
                {
                    3,
                    "Prefabs/G00301"
                }
            };
            if (!dictionary.ContainsKey(id))
            {
                return;
            }
            GameObject gameObject = Resources.Load(dictionary[id]) as GameObject;
            gameObject = UnityEngine.Object.Instantiate<GameObject>(gameObject);
            gameObject.transform.SetParent(this.content.transform, false);
            // if (id == 3 && value != 0)
            // {
            // 	G3BoardGenerator.GetInstance().StartNewGame(value);
            // }
            this.m_gameDict.Add(id, gameObject);
        }
        if (isPageIn)
        {
            this.PlayGameIn();
        }
        //AppsflyerUtils.TrackPlayGame(id);
    }

    public void HideTopBtn()
    {
        this.m_video.gameObject.SetActive(false);
        this.m_videoTimer.gameObject.SetActive(false);
        this.m_btn_return.gameObject.SetActive(false);
    }

    public void ShowTopBtn()
    {
        this.m_video.gameObject.SetActive(true);
        this.m_btn_return.gameObject.SetActive(true);
        this.m_videoTimer.gameObject.SetActive(true);
    }

    public void PlayGuideAni()
    {
        Sequence arg_23_0 = DOTween.Sequence();
        // Vector3 savePos = this.m_img_finger.localPosition;
        // arg_23_0.Append(this.m_img_finger.DOLocalMove(savePos + new Vector3(200f, 0f, 0f), 1.5f, false));
        arg_23_0.AppendCallback(delegate
        {
            // this.m_img_finger.localPosition = savePos;
        });
        arg_23_0.SetLoops(-1);
        // arg_23_0.SetTarget(this.m_img_finger);
        // this.m_img_finger.gameObject.SetActive(true);
    }

    public void StopGuideAni()
    {
        // DOTween.Kill(this.m_img_finger, false);
        // this.m_img_finger.gameObject.SetActive(false);
    }

    private void Init()
    {
        // Previous behaviour:
        //   - If a saved game existed (GM.isSavedGame == true), we immediately hid the
        //     game list and auto-loaded the saved game into `content`.
        // This caused the 2D mode to bypass the main menu when restarting the app,
        // which is different from the 3D flow (3D always shows the main menu first
        // so the player can choose 2D/3D, then continue).
        //
        // New behaviour:
        //   - Always start by showing the main game list (mode selector).
        //   - The presence of a saved game is still tracked via GM, and individual
        //     game modes (e.g. G2BoardGenerator for 2D) decide whether to load saved
        //     data when the player actually starts that mode.
        //
        // This aligns 2D with 3D: on app restart you see the main menu first, then
        // after choosing 2D/3D you continue from where you left off.

        // Show main menu list by default.
        this.gamelist.SetActive(true);
        if (this.content != null)
        {
            this.content.SetActive(false);
        }

        // We intentionally no longer auto-call LoadGame here.

        this.RefreshRecord(3);
        this.RefreshMaxScore(new string[]
        {
            GM.GetInstance().GetScoreRecord(1).ToString(),
            GM.GetInstance().GetScoreRecord(2).ToString(),
            GM.GetInstance().GetScoreRecord(3).ToString()
        });
    }

    private void InitEvent()
    {
        GlobalEventHandle.DoGoHome += new Action(this.Init);
        GlobalEventHandle.OnRefreshAchiveHandle = (Action<int>)Delegate.Combine(GlobalEventHandle.OnRefreshAchiveHandle, new Action<int>(this.RefreshRecord));
        GlobalEventHandle.OnRefreshMaxScoreHandle = (Action<string[]>)Delegate.Combine(GlobalEventHandle.OnRefreshMaxScoreHandle, new Action<string[]>(this.RefreshMaxScore));
        GlobalEventHandle.AdsHandle += new Action<string, bool>(this.OnRefreshAdsTimer);
    }

    private void PlayGameIn()
    {
        if (this.content == null)
        {
            return;
        }
        this.content.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
        Sequence expr_38 = DOTween.Sequence();
        expr_38.Append(this.content.transform.DOScale(1.1f, 0.3f));
        expr_38.Append(this.content.transform.DOScale(1f, 0.1f));
        expr_38.SetUpdate(true);
    }

    private void RefreshRecord(int id = 3)
    {
        // Check if required components are destroyed/null (object might be destroyed when scene changes)
        if (this.m_maxNumber02 == null || this.m_maxNumber03 == null)
        {
            return;
        }

        if (id != 3 && id != 6)
        {
            return;
        }
        int num = 5;
        LocalData localData = AchiveData.GetInstance().Get(3);
        num = ((localData.value > num) ? localData.value : num);
        // this.m_maxNumber01.setNum(num);
        num = 64;
        localData = AchiveData.GetInstance().Get(6);
        num = ((localData.value > num) ? localData.value : num);
        this.m_maxNumber02.setNum(num);
        this.m_maxNumber03.setNum(num);
        this.m_record_number = num;
    }

    public void RefreshMaxScore(string[] array)
    {
        // Check if required components are destroyed/null (object might be destroyed when scene changes)
        if (this.m_txt_maxScore01 == null || this.m_txt_maxScore02 == null || this.m_txt_maxScore03 == null)
        {
            return;
        }

        if (array == null || array.Length < 3)
        {
            return;
        }

        this.m_txt_maxScore01.text = array[0];
        this.m_txt_maxScore02.text = array[1];
        if (Configs.TG00301.ContainsKey(array[2]))
        {
            this.m_txt_maxScore03.text = string.Format("{0}/300", Configs.TG00301[array[2]].Level);
            return;
        }
        this.m_txt_maxScore03.text = string.Format("{0}/300", 0);
    }

    private void PlayRecordAni()
    {
        DOTween.Kill(this.m_node001, false);
        DOTween.Kill(this.m_node002, false);
        if (!this.gamelist.activeSelf)
        {
            return;
        }
        Sequence expr_3A = DOTween.Sequence();
        expr_3A.AppendCallback(delegate
        {
            this.m_node001.transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        });
        expr_3A.Append(this.m_node001.transform.DOBlendableLocalRotateBy(new Vector3(0f, 0f, 5f), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        expr_3A.Append(this.m_node001.transform.DOBlendableRotateBy(new Vector3(0f, 0f, -10f), 1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        expr_3A.Append(this.m_node001.transform.DOBlendableRotateBy(new Vector3(0f, 0f, 5f), 0.5f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        expr_3A.SetLoops(-1);
        expr_3A.SetTarget(this.m_node001);
        Vector3 arg_116_0 = this.m_node002.transform.localPosition;
        Vector3 localPosition = this.m_maxNumber02.transform.localPosition;
        Transform img_box = this.m_node002.transform.Find("img_02");
        Sequence expr_148 = DOTween.Sequence();
        expr_148.AppendCallback(delegate
        {
            this.m_node002.transform.localPosition = this.m_savePage2Block;
            this.m_node002.GetComponent<Image>().color = Color.white;
            img_box.GetComponent<Image>().color = Color.white;
            this.m_maxNumber03.GetComponent<G2Block>().FadeIn();
            this.m_maxNumber03.transform.localScale = Vector3.one;
            this.m_maxNumber02.GetComponent<G2Block>().setNum(this.m_record_number);
            this.m_maxNumber03.GetComponent<G2Block>().setNum(this.m_record_number);
            this.m_arrow.GetComponent<Image>().color = Color.white;
        });
        expr_148.Append(this.m_arrow.GetComponent<Image>().DOFade(0.5f, 0.5f));
        expr_148.Append(this.m_arrow.GetComponent<Image>().DOFade(1f, 0.5f));
        expr_148.Append(this.m_arrow.GetComponent<Image>().DOFade(0.5f, 0.5f));
        expr_148.Append(this.m_arrow.GetComponent<Image>().DOFade(1f, 0.5f));
        expr_148.Append(this.m_arrow.GetComponent<Image>().DOFade(0f, 0.5f));
        expr_148.Append(this.m_node002.transform.DOLocalMove(localPosition, 0.5f, false).OnComplete(delegate
        {
            this.m_node002.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            img_box.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0f);
            this.m_maxNumber03.GetComponent<G2Block>().HideNumber();
            this.m_maxNumber02.GetComponent<G2Block>().setNum(this.m_record_number * 2);
            this.m_maxNumber03.GetComponent<G2Block>().setNum(this.m_record_number * 2);
        }));
        expr_148.Append(this.m_maxNumber03.GetComponent<G2Block>().FadeOut());
        expr_148.Insert(3f, this.m_maxNumber03.transform.DOScale(1.5f, 0.5f));
        expr_148.AppendInterval(1f);
        expr_148.SetLoops(-1);
        expr_148.SetTarget(this.m_node002);
    }

    private void OnRefreshAdsTimer(string timer, bool isWatch)
    {
        // Check if required components are destroyed/null (object might be destroyed when scene changes)
        if (this.content == null || this.m_videoTimer == null)
        {
            return;
        }

        if (!this.content.activeSelf)
        {
            return;
        }
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
        Transform transform = this.m_video.transform.Find("icon");
        Sequence expr_21 = DOTween.Sequence();
        expr_21.Append(transform.transform.DOScale(1.2f, 1f).SetEase(Ease.Linear));
        expr_21.Append(transform.transform.DOScale(1f, 1f).SetEase(Ease.Linear));
        expr_21.Append(transform.DOLocalRotate(new Vector3(0f, 0f, 20f), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        expr_21.Append(transform.DOLocalRotate(new Vector3(0f, 0f, -20f), 0.2f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        expr_21.Append(transform.DOLocalRotate(new Vector3(0f, 0f, 10f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        expr_21.Append(transform.DOLocalRotate(new Vector3(0f, 0f, -10f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        expr_21.Append(transform.DOLocalRotate(new Vector3(0f, 0f, 0f), 0.1f, RotateMode.LocalAxisAdd).SetEase(Ease.Linear));
        expr_21.SetLoops(-1);
        expr_21.SetTarget(this.m_video);
    }

    private void StopAdsTipsAni()
    {
        Transform arg_22_0 = this.m_video.transform.Find("icon");
        DOTween.Kill(this.m_video, false);
        arg_22_0.localRotation = new Quaternion(0f, 0f, 0f, 0f);
        arg_22_0.localScale = new Vector3(1f, 1f, 1f);
    }
}
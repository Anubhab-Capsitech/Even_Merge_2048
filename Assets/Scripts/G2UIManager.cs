using Assets.Scripts.GameManager;
using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/*
 * this class will show information of game UI in game over
 */
public class G2UIManager : MonoBehaviour
{
    //common function, first sharing will get 5 diamonds
	[Serializable]
	private sealed class CommonClass
	{
		public static readonly G2UIManager.CommonClass _commonClass = new G2UIManager.CommonClass();

		public static Action _action;

		internal void OnClickShare()
		{
			if (GM.GetInstance().isFirstShare())
			{
				GM.GetInstance().ResetFirstShare(1);
				GM.GetInstance().AddDiamond(5, true);
			Debug.Log("AddDiamond Added: " );
			}
		}
	}
    //score value text
	public TextMeshProUGUI m_txt_score_value;
    //best score value text
	public TextMeshProUGUI m_txt_record_value;
    //button play again
	public Button m_btn_again;
    //button quit game play
	public Button m_btn_main;
    //game ID (default 2 for block 2048 shooter)
	private int gameID = 2;

	public Image m_img_circle;

	public LanguageComponent m_txt_tips;

	public Transform[] m_tips_component;

	public string[] m_tips_content;

	private void Start()
	{
		GM.GetInstance().SetSavedGameID(0);
		GM.GetInstance().SaveScore(this.gameID, 0);
        G2BoardGenerator.GetInstance().FinishCount = 0;
		//PersistentAudioManager.Instance.PlayEffect("sound_eff_victory");
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		DOTween.Kill(this.m_img_circle, false);
	}

	public void Load(int score, int maxScore)
	{
		this.m_txt_score_value.text = string.Format((score < 1000) ? "{0}" : "{0:0,00}", score);
		this.m_txt_record_value.text = string.Format((maxScore < 1000) ? "{0}" : "{0:0,00}", maxScore);
	}

	public void OnClickAgain()
	{
		GM.GetInstance().SetSavedGameID(this.gameID);
        G2BoardGenerator.GetInstance().StartNewGame();
		DialogManager.GetInstance().Close(null);
        PersistentAudioManager.Instance?.PlayClickSound();
    }

	public void OnClickAds()
	{
		AdsManager.GetInstance().Play(AdsManager.AdType.UseProp, delegate
		{
			GM.GetInstance().SetSavedGameID(this.gameID);
			DialogManager.GetInstance().Close(null);
			Action _action = G2BoardGenerator.GetInstance().DoVedioRefresh;
			if (_action == null)
			{
				return;
			}
            _action();
		}, null, 5, null);
	}

	public void OnClickHome()
	{
        // Reset 2D game state so the next run starts from a clean slate.
		// CRITICAL FIX: Kill the 2D game loop immediately.
		// Setting IsStart = false ensures Game2Manager.Update() returns immediately at the very top.
		G2BoardGenerator.GetInstance().IsStart = false; 
		G2BoardGenerator.GetInstance().IsPlaying = false;
		G2BoardGenerator.GetInstance().IsPuase = true;

        G2BoardGenerator.GetInstance().Score = 0;
		GM.GetInstance().SaveScore(this.gameID, 0);
		GM.GetInstance().SetSavedGameID(0);
		GM.GetInstance().ResetToNewGame();
		GM.GetInstance().ResetConsumeCount();

		// CRITICAL FIX: Clear the saved 2D map data so that when returning to 2D
		// after playing 3D, it starts a fresh game instead of loading the old game over state.
		// The map data is stored separately from SavedGameID, so we need to explicitly clear it.
		PlayerPrefs.DeleteKey("LocalData_OldGame");
		PlayerPrefs.DeleteKey("LocalData_OldLife");
		PlayerPrefs.DeleteKey("LocalData_OldPosX");
		PlayerPrefs.DeleteKey("LocalData_OldPosY");
		PlayerPrefs.Save();

		// Close the Game Over dialog.
		DialogManager.GetInstance().Close(null);

		// Switch the UI back to main-menu mode (hide in-game elements for 2D).
		GameList.Instance.UiSwitching(false);
		GameList.Instance.HideTopBtn();

		// Re-initialize the main menu layout and list.
		GlobalEventHandle.EmitDoGoHome();
		GlobalEventHandle.EmitClickPageButtonHandle("main", 0);

		// Ensure the 2D/3D toggle is visible again on the main menu.
		EvenMergeModeSwitcher.Instance?.ShowToggle(true);
        PersistentAudioManager.Instance?.PlayClickSound();
    }

	public void OnClickRate()
	{
        Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
	}

	private void PlayTips()
	{
		int idx = 0;
		this.m_img_circle.gameObject.SetActive(true);
		Sequence _sequence = DOTween.Sequence();
        _sequence.Append(this.m_img_circle.DOFade(0.3f, 1f));
        _sequence.Append(this.m_img_circle.DOFade(1f, 1f));
        _sequence.Append(this.m_img_circle.DOFade(0.3f, 1f));
        _sequence.Append(this.m_img_circle.DOFade(1f, 1f));
        _sequence.SetLoops(-1);
        _sequence.SetTarget(this.m_img_circle);
		Sequence _sequence2 = DOTween.Sequence();
        int idx1 = 0;
        _sequence2.AppendCallback(delegate
		{
			//int idx;
			this.m_img_circle.transform.localPosition = this.m_tips_component[idx1].localPosition;
			this.m_txt_tips.SetText(this.m_tips_content[idx1]);
			idx1++;
			idx = idx1;
			idx1 %= 3;
		});
        _sequence2.AppendInterval(4f);
        _sequence2.SetLoops(-1);
        _sequence2.SetTarget(this.m_img_circle);
	}
}

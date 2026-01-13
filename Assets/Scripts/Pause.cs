using Assets.Scripts.GameManager;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
	public TextMeshProUGUI m_lb_title;

	public TextMeshProUGUI m_lb_score;

	public TextMeshProUGUI m_lb_score_value;

	public Button m_btn_home;

	public Button m_btn_refresh;

	public Button m_btn_continue;

	public Action OnClickHomeHandle;

	public Action OnClickRefreshHandle;

	public Action OnClickContinueHandle;

	

	private bool isExitingToMenu = false;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		// When pause panel is destroyed (including via back gesture closing the dialog),
		// ensure the game is unpaused unless we're in a game over state OR exiting to menu
		if (!isExitingToMenu && G2BoardGenerator.GetInstance() != null && !G2BoardGenerator.GetInstance().IsGameOver)
		{
			G2BoardGenerator.GetInstance().IsPuase = false;
		}
	}

	public void SetScore(int score)
	{
		this.m_lb_score_value.text = string.Format((score < 1000) ? "{0}" : "{0:0,00}", score);
	}

	public void SetTitle(string id)
	{
		this.m_lb_score.GetComponent<LanguageComponent>().SetText(id);
	}

	public void OnClickHome()
	{
		isExitingToMenu = true; // Mark as exiting so OnDestroy doesn't unpause
		Action _action = this.OnClickHomeHandle;
		if (_action != null)
		{
            _action();
		}
		DialogManager.GetInstance().Close(null);
		GameList.Instance.UiSwitching(false);
		EvenMergeModeSwitcher.Instance?.ShowToggle(true);
		PersistentAudioManager.Instance.PlayClickSound();
	}
	
	public void OnClickMenu()
	{
		isExitingToMenu = true; // Mark as exiting so OnDestroy doesn't unpause
		// Reset game state before loading menu scene
		// This ensures the menu is shown instead of a saved game
		// The issue was that GameList.Init() checks isSavedGame() and hides menu items if true
		
        // DO NOT RESET for resume feature
		// GM.GetInstance().SetSavedGameID(0);
		// GM.GetInstance().ResetToNewGame();
		// GM.GetInstance().ResetConsumeCount();
		
		// Also reset the GameId to ensure clean state
		// GM.GetInstance().GameId = 0;
		
		// Close the dialog/pause panel
		DialogManager.GetInstance().Close(null);
		
		// Load the main menu scene by name
		SceneManager.LoadScene("MainScene 1");
        PersistentAudioManager.Instance.PlayClickSound();
    }

	public void OnClickRefresh()
	{
			

		// if (GM.GetInstance().IsRandomStatus(50))
		// {
		// 	AdsManager.GetInstance().Play(AdsManager.AdType.Refresh, delegate
		// 	{
		// 		Action _action = this.OnClickRefreshHandle;
		// 		if (_action == null)
		// 		{
		// 			return;
		// 		}
        //         _action();
		// 	}, null, 5, null);
		// }
		// else
		// {
			Action _action2 = this.OnClickRefreshHandle;
			if (_action2 != null)
			{
                _action2();
			}
		// }
		DialogManager.GetInstance().Close(null);
        PersistentAudioManager.Instance.PlayClickSound();
    }

	public void OnClickContinue()
	{
		// if (GM.GetInstance().IsRandomStatus(50))
		// {
		// 	AdsManager.GetInstance().Play(AdsManager.AdType.Continue, delegate
		// 	{
		// 		Action _action = this.OnClickContinueHandle;
		// 		if (_action == null)
		// 		{
		// 			return;
		// 		}
        //         _action();
		// 	}, null, 5, null);
		// }
		// else
		// {
			Action _action2 = this.OnClickContinueHandle;
			if (_action2 != null)
			{
                _action2();
			}
		// }
		DialogManager.GetInstance().Close(null);
        PersistentAudioManager.Instance.PlayClickSound();
    }
}

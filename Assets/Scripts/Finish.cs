using Assets.Scripts.GameManager;
using Assets.Scripts.Utils;
using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class Finish : MonoBehaviour
{
	private int m_gameID = 1;

	private int m_number = 1;

	private int m_type = 1;

	// public G1Block m_g00101;

	public G2Block m_g00201;

	public Image m_progress;

	public GameObject m_btn_continue;

	public GameObject m_btn_coins;

	public GameObject m_btn_no;

	public int GameID
	{
		get
		{
			return this.m_gameID;
		}
		set
		{
			this.m_gameID = value;
		}
	}

	public int Number
	{
		get
		{
			return this.m_number;
		}
		set
		{
			this.m_number = value;
		}
	}

	private void Start()
	{
		OnClickAgain();
		if (AdsUtil.isRewardAdsLoaded() || AdsUtil.isRewardISAdsLoaded())
		{
			this.m_type = 1;
		}
		else
		{
			this.m_type = 2;
		}
		this.m_btn_coins.gameObject.SetActive(false);
		this.m_btn_continue.gameObject.SetActive(false);
		this.m_progress.DOFillAmount(0f, 10f).SetEase(Ease.Linear).OnComplete(delegate
		{
			this.OnClickAgain();
		});
		Sequence sequence = DOTween.Sequence();
		int type = this.m_type;
		if (type != 1)
		{
			if (type == 2)
			{
				sequence.Append(this.m_btn_coins.transform.DOScale(1.1f, 1f));
				sequence.Append(this.m_btn_coins.transform.DOScale(1f, 1f));
				sequence.SetLoops(-1);
				sequence.SetTarget(this.m_btn_coins);
				this.m_btn_coins.gameObject.SetActive(true);
			}
		}
		else
		{
			sequence.Append(this.m_btn_continue.transform.DOScale(1.1f, 1f));
			sequence.Append(this.m_btn_continue.transform.DOScale(1f, 1f));
			sequence.SetLoops(-1);
			sequence.SetTarget(this.m_btn_continue);
			this.m_btn_continue.gameObject.SetActive(true);
		}
		sequence = DOTween.Sequence();
		sequence.Append(this.m_btn_no.transform.DOScale(0f, 0f));
		sequence.AppendInterval(2f);
		sequence.Append(this.m_btn_no.transform.DOScale(1f, 0.2f));
		sequence.SetTarget(this.m_btn_no);
		GM.GetInstance().SetFirstFinishGame();
	}

	private void Update()
	{
	}

	private void OnDestroy()
	{
		DOTween.Kill(this.m_btn_no, false);
		DOTween.Kill(this.m_btn_continue, false);
		DOTween.Kill(this.m_btn_coins, false);
		this.m_progress.DOKill(false);
	}

	private void OnApplicationFocus(bool isFocus)
	{
		if (isFocus)
		{
			UnityEngine.Debug.Log("unit ads true");
			return;
		}
		UnityEngine.Debug.Log("unit ads false");
	}

	public void Load(int gameId, int number)
	{
		GM.GetInstance().SetSavedGameID(0);
		GM.GetInstance().SaveScore(this.GameID, 0);
		this.GameID = gameId;
		this.Number = number;
		if (gameId == 1)
		{
			// this.m_g00101.gameObject.SetActive(true);
			this.m_g00201.gameObject.SetActive(false);
			// this.m_g00101.setNum(number);
			return;
		}
		if (gameId != 2)
		{
			return;
		}
		// this.m_g00101.gameObject.SetActive(false);
		this.m_g00201.gameObject.SetActive(true);
		this.m_g00201.setNum(number);
	}

	public void OnClickAds()
	{
		int gameID = this.GameID;
		// if (gameID == 1)
		// {
    //         /*
		// 	AdsManager.GetInstance().Play(AdsManager.AdType.ResetLife, delegate
		// 	{
		// 		GM.GetInstance().SetSavedGameID(this.GameID);
    //             Game1DataLoader.GetInstance().FillLife(false);
    //             Game1DataLoader.GetInstance().DoFillLife();
		// 		DialogManager.GetInstance().Close(null);
		// 	}, delegate
		// 	{
		// 		this.ShowFinish();
		// 	}, 5, null);
		// 	*/
    // //        if (AdsControl.Instance.GetRewardAvailable())
    // //        {

    // //            AdsControl.Instance.PlayDelegateRewardVideo(delegate
		// 		//{

		// 			GM.GetInstance().SetSavedGameID(this.GameID);
    //                 Game1DataLoader.GetInstance().FillLife(false);
    //                 Game1DataLoader.GetInstance().DoFillLife();
    //                 DialogManager.GetInstance().Close(null);
    //         //    });
    //         //}
    //         return;
		// }
		if (gameID != 2)
		{
			return;
		}
        /*
		AdsManager.GetInstance().Play(AdsManager.AdType.ResetLife2, delegate
		{
			GM.GetInstance().SetSavedGameID(this.GameID);
			DialogManager.GetInstance().Close(null);
			Action expr_25 = G2BoardGenerator.GetInstance().DoVedioRefresh;
			if (expr_25 == null)
			{
				return;
			}
			expr_25();
		}, delegate
		{
			this.ShowFinish();
		}, 5, null);
		*/
        //if (AdsControl.Instance.GetRewardAvailable())
        //{

        //    AdsControl.Instance.PlayDelegateRewardVideo(delegate
        //    {


                GM.GetInstance().SetSavedGameID(this.GameID);
                DialogManager.GetInstance().Close(null);
                Action expr_25 = G2BoardGenerator.GetInstance().DoVedioRefresh;
                if (expr_25 == null)
                {
                    return;
                }
                expr_25();
        //    });
        //}
    }

	public void OnClickCoin()
	{
		if (!GM.GetInstance().isFullGEM(100))
		{
			ToastManager.Show("TXT_NO_50001", true);
			return;
		}
		int gameID = this.GameID;
		// if (gameID == 1)
		// {
		// 	GM.GetInstance().SetSavedGameID(this.GameID);
    //         Game1DataLoader.GetInstance().FillLife(false);
    //         Game1DataLoader.GetInstance().DoFillLife();
		// 	DialogManager.GetInstance().Close(null);
		// 	return;
		// }
		if (gameID != 2)
		{
			return;
		}
		GM.GetInstance().SetSavedGameID(this.GameID);
		DialogManager.GetInstance().Close(null);
		Action expr_85 = G2BoardGenerator.GetInstance().DoVedioRefresh;
		if (expr_85 == null)
		{
			return;
		}
		expr_85();
	}

	public void OnClickAgain()
	{
		if (GM.GetInstance().IsFirstFinishGame())
		{
			this.ShowFinish();
			return;
		}
		AdsManager.GetInstance().Play(AdsManager.AdType.Finish, delegate
		{
			this.ShowFinish();
		}, null, 5, null);
	}

	private void ShowFinish()
	{
		this.m_progress.DOKill(false);
       // AdsControl.Instance.showAds();
		DialogManager.GetInstance().Close(delegate
		{
			int gameID = this.GameID;
			// if (gameID == 1)
			// {
			// 	GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("Prefabs/G00102") as GameObject);
			// 	gameObject.GetComponent<G1UIManager>().Load(Game1DataLoader.GetInstance().Score, Game1DataLoader.GetInstance().MaxScore);
			// 	DialogManager.GetInstance().show(gameObject, true);
			// 	return;
			// }
			if (gameID != 2)
			{
				return;
			}
			GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(Resources.Load("Prefabs/G00203") as GameObject);
			gameObject2.GetComponent<G2UIManager>().Load(G2BoardGenerator.GetInstance().Score, G2BoardGenerator.GetInstance().MaxScore);
			DialogManager.GetInstance().show(gameObject2, true);
            PersistentAudioManager.Instance.PlayEffect("sound_eff_happywingame");
        });
	}
}

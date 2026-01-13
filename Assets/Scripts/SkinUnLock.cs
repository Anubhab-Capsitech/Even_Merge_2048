using Assets.Scripts.GameManager;
using Assets.Scripts.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SkinUnLock : MonoBehaviour
{
	public Sprite[] m_skins;

	public Image m_skinIcon;

	public Text m_skinPrice;

	private int m_skinID = 1;

	private int[] prices = new int[]
	{
		0,
		100
	};

	private Action<int> OnUnlockSuccess;

	public int SkinID
	{
		get
		{
			return this.m_skinID;
		}
		set
		{
			this.m_skinID = value;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Load(int skinID)
	{
		this.SkinID = skinID;
		if (this.m_skinIcon != null)
		{
			this.m_skinIcon.sprite = this.m_skins[skinID - 1];
		}
		if (this.m_skinPrice != null)
		{
			this.m_skinPrice.text = this.prices[this.SkinID - 1].ToString();
		}
	}

	public void OnClickVedio()
	{
        /*
		AdsManager.GetInstance().Play(AdsManager.AdType.Skin, delegate
		{
			GM.GetInstance().SetSkinData(this.SkinID, 2);
			GM.GetInstance().SetSkinFreeTime(this.SkinID, DateTime.Now);
			//AppsflyerUtils.TrackBuySkin(this.SkinID, 1);
			Action<int> expr_38 = this.OnUnlockSuccess;
			if (expr_38 != null)
			{
				expr_38(this.SkinID);
			}
			DialogManager.GetInstance().Close(null);
		}, null, 5, null);
        */
        //if (AdsControl.Instance.GetRewardAvailable())
        //{

        //    AdsControl.Instance.PlayDelegateRewardVideo(delegate
        //    {

                GM.GetInstance().SetSkinData(this.SkinID, 2);
                GM.GetInstance().SetSkinFreeTime(this.SkinID, DateTime.Now);
                //AppsflyerUtils.TrackBuySkin(this.SkinID, 1);
                Action<int> expr_38 = this.OnUnlockSuccess;
                if (expr_38 != null)
                {
                    expr_38(this.SkinID);
                }
                DialogManager.GetInstance().Close(null);
        //    });
        //}
    }

	public void OnClickBuy()
	{
		if (!GM.GetInstance().isFullGEM(this.prices[this.SkinID - 1]))
		{
			ToastManager.Show("TXT_NO_50001", true);
			return;
		}
		GM.GetInstance().ConsumeGEM(this.prices[this.SkinID - 1]);
		GM.GetInstance().SetSkinData(this.SkinID, 0);
		Action<int> _action = this.OnUnlockSuccess;
		if (_action != null)
		{
            _action(this.SkinID);
		}
		DialogManager.GetInstance().Close(null);
	}

	public void OnClickClose()
	{
		DialogManager.GetInstance().Close(null);
	}

	public void SetOnUnlockSuccess(Action<int> callfunc)
	{
		this.OnUnlockSuccess = callfunc;
	}
}

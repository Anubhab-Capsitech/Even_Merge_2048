using Assets.Scripts.Utils;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;
//using UnityEngine.Purchasing;

namespace Assets.Scripts.GameManager
{
	internal class GoodsManager
	{
		private static GoodsManager m_intance;

		private int m_ads;

		//private SubscriptionInfo m_subscription;

		//public Action<Product, bool> ProcessNonConsumableHandle;

		//public Action<SubscriptionInfo, bool> ProcessSubscriptionHandle;

		//public Action<Product, bool> ProcessConsumableHanle;



		public event Action<int, int> ShowSubscriptionHanle;

		public int Ads
		{
			get
			{
				return this.m_ads;
			}
			set
			{
				this.m_ads = value;
			}
		}

		//public SubscriptionInfo Subscription
		//{
		//	get
		//	{
		//		return this.m_subscription;
		//	}
		//	set
		//	{
		//		this.m_subscription = value;
		//	}
		//}

		public static void Initialize()
		{
			GoodsManager.m_intance = new GoodsManager();
			//Store.getInstance().ProcessNonConsumableHandle = new Action<Product, bool>(GoodsManager.m_intance.OnProcessNonConsumable);
			//Store.getInstance().ProcessConsumableHanle = new Action<Product, bool>(GoodsManager.m_intance.OnProcessConsumable);
			//Store.getInstance().ProcessSubscriptionHandle = new Action<SubscriptionInfo, bool>(GoodsManager.m_intance.OnProcessSubscription);
		}

		public static GoodsManager GetInstance()
		{
			if (GoodsManager.m_intance == null)
			{
				GoodsManager.m_intance = new GoodsManager();
			}
			return GoodsManager.m_intance;
		}

		public bool isPuchasedAds()
		{
			return this.m_ads == 1;
		}

		//public bool isPurchaseSubscription()
		//{
		//	//return this.m_subscription != null;
		//}

		public void SendSubscriptionAwards()
		{
			//if (this.m_subscription == null)
			{
			//	return;
			}
			//if (this.m_subscription.isFreeTrial() == Result.False && this.m_subscription.isSubscribed() == Result.False)
			{
				return;
			}
			//if (this.isGeted())
			{
				return;
			}
			//int days = (DateTime.Now - this.m_subscription.getPurchaseDate()).Days;
			//if (days < 1 || days > 7)
			//{
			//	return;
			//}
			//int num = (DateTime.Now - this.GetPreTime()).Days * 120;
			//GM.GetInstance().AddDiamondBase(num);
			//this.Mark();
			Action<int, int> expr_8A = this.ShowSubscriptionHanle;
			if (expr_8A == null)
			{
				return;
			}
			//expr_8A(days + 1, num);
		}

		public void Reset()
		{
			PlayerPrefs.SetInt("LocalData_NowStatus", 0);
		}

		//private void OnProcessConsumable(Product product, bool isInit)
		//{
		//	GM.GetInstance().AddDiamond((int)product.definition.payout.quantity, false);
		//	Action<Product, bool> expr_23 = this.ProcessConsumableHanle;
		//	if (expr_23 == null)
		//	{
		//		return;
		//	}
		//	expr_23(product, isInit);
		//}

		//private void OnProcessNonConsumable(Product product, bool isInit)
		//{
		//	this.m_ads = 1;
		//	Action<Product, bool> expr_0D = this.ProcessNonConsumableHandle;
		//	if (expr_0D == null)
		//	{
		//		return;
		//	}
		//	expr_0D(product, isInit);
		//}

		//private void OnProcessSubscription(SubscriptionInfo info, bool isInit)
		//{
		//	this.m_ads = 1;
		//	this.m_subscription = info;
		//	if (!isInit)
		//	{
		//		if (this.m_subscription == null)
		//		{
		//			return;
		//		}
		//		if (this.m_subscription.isFreeTrial() == Result.False && this.m_subscription.isSubscribed() == Result.False)
		//		{
		//			return;
		//		}
		//		GM.GetInstance().AddDiamondBase(120);
		//		this.Mark();
		//		Action<int, int> expr_50 = this.ShowSubscriptionHanle;
		//		if (expr_50 != null)
		//		{
		//			expr_50(1, 120);
		//		}
		//	}
		//	else
			//{
		//		this.SendSubscriptionAwards();
		//	//}
		//	Action<SubscriptionInfo, bool> expr_6C = this.ProcessSubscriptionHandle;
		//	if (expr_6C == null)
		//	{
		//		return;
		//	}
		//	expr_6C(info, isInit);
		//}

		//private void SaveTime()
		//{
		//	PlayerPrefs.SetString("LocalData_PreSubscriptionTime", DateTime.Now.ToString("yyyy-MM-dd"));
		//}

		//private DateTime GetPreTime()
		//{
		//	string @string = PlayerPrefs.GetString("LocalData_PreSubscriptionTime", "-1");
		//	if (@string.Equals("-1"))
		//	{
		//		return DateTime.Now;
		//	}
		//	string[] array = @string.Split(new char[]
		//	{
		//		'-'
		//	});
		//	return new DateTime(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]));
		//}

		//private bool isGeted()
		//{
		//	return PlayerPrefs.GetInt("LocalData_NowStatus", 0) == 1;
		//}

		//private void Mark()
		//{
		//	this.SaveTime();
		//	PlayerPrefs.SetInt("LocalData_NowStatus", 1);
		//}
	}
}

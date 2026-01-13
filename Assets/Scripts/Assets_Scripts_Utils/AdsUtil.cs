using System;
using UnityEngine;

namespace Assets.Scripts.Utils
{
	public class AdsUtil
	{
		public static int showInsertAds(Action finishCallback = null, Action cancelCallback = null)
		{
			int result = 0;
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.IPhonePlayer)
			{
				if (platform == RuntimePlatform.Android)
				{
					// Utils.CallAndroiSDKdFunc<int>("FacebookHelper", "showInsertAds", out result, Array.Empty<object>());
				}
				else
				{
					// Utils.ShowWinAds();
				}
			}
			else
			{
				result = 0;
			}
			JavaInvokeCShape.GetInstance().OnAdsComplateHandle = finishCallback;
			JavaInvokeCShape.GetInstance().OnAdsCancelHandle = cancelCallback;
			return result;
		}

		public static bool isInsertAdsLoaded()
		{
			bool result = false;
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.IPhonePlayer)
			{
				if (platform == RuntimePlatform.Android)
				{
					// Utils.CallAndroiSDKdFunc<bool>("FacebookHelper", "isInsertAdLoaded", out result, Array.Empty<object>());
				}
				else
				{
					result = false;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static void LoadInsertAds()
		{
			// // int num = 0;
			// RuntimePlatform platform = Application.platform;
			// if (platform == RuntimePlatform.IPhonePlayer)
			// {
			// 	num = 0;
			// 	return;
			// }
			// if (platform == RuntimePlatform.Android)
			// {
			// 	// Utils.CallAndroiSDKdFunc<int>("FacebookHelper", "loadInsertAds", out num, Array.Empty<object>());
			// 	return;
			// }
			// num = 0;
		}

		public static int showRewardAds(Action finishCallback = null, Action cancelCallback = null)
		{
			int result = 0;
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.IPhonePlayer)
			{
				if (platform == RuntimePlatform.Android)
				{
					// Utils.CallAndroiSDKdFunc<int>("FacebookHelper", "showRewardVideoAds", out result, Array.Empty<object>());
				}
				else
				{
					Utils.ShowWinAds();
				}
			}
			else
			{
				result = 0;
			}
			JavaInvokeCShape.GetInstance().OnAdsComplateHandle = finishCallback;
			JavaInvokeCShape.GetInstance().OnAdsCancelHandle = cancelCallback;
			return result;
		}

		public static bool isRewardAdsLoaded()
		{
			bool result = false;
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.IPhonePlayer)
			{
				if (platform == RuntimePlatform.Android)
				{
					// Utils.CallAndroiSDKdFunc<bool>("FacebookHelper", "isRewardVedioAdLoaded", out result, Array.Empty<object>());
				}
				else
				{
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static void LoadRewardAds()
		{
			int num = 0;
			RuntimePlatform platform = Application.platform;
			if (platform == RuntimePlatform.IPhonePlayer)
			{
				num = 0;
				return;
			}
			if (platform == RuntimePlatform.Android)
			{
				// Utils.CallAndroiSDKdFunc<int>("FacebookHelper", "loadRewardVedioAds", out num, Array.Empty<object>());
				return;
			}
			num = 0;
		}

		public static int showInsertISAds(Action finishCallback = null, Action cancelCallback = null)
		{
			int result = 0;
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.IPhonePlayer)
			{
				if (platform == RuntimePlatform.Android)
				{
					Utils.CallAndroiSDKdFunc<int>("IronsourceHelper", "showInsertAds", out result, Array.Empty<object>());
				}
				else
				{
					Utils.ShowWinAds();
				}
			}
			else
			{
				result = 0;
			}
			JavaInvokeCShape.GetInstance().OnAdsComplateHandle = finishCallback;
			JavaInvokeCShape.GetInstance().OnAdsCancelHandle = cancelCallback;
			return result;
		}

		public static bool isInsertISAdsLoaded()
		{
			bool result = false;
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.IPhonePlayer)
			{
				if (platform == RuntimePlatform.Android)
				{
					Utils.CallAndroiSDKdFunc<bool>("IronsourceHelper", "isInsertAdLoaded", out result, Array.Empty<object>());
				}
				else
				{
					result = false;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static int showRewardISAds(Action finishCallback = null, Action cancelCallback = null)
		{
			int result = 0;
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.IPhonePlayer)
			{
				if (platform == RuntimePlatform.Android)
				{
					Utils.CallAndroiSDKdFunc<int>("IronsourceHelper", "showRewardVideoAds", out result, Array.Empty<object>());
				}
				else
				{
					Utils.ShowWinAds();
				}
			}
			else
			{
				result = 0;
			}
			JavaInvokeCShape.GetInstance().OnAdsComplateHandle = finishCallback;
			JavaInvokeCShape.GetInstance().OnAdsCancelHandle = cancelCallback;
			return result;
		}

		public static bool isRewardISAdsLoaded()
		{
			bool result = false;
			RuntimePlatform platform = Application.platform;
			if (platform != RuntimePlatform.IPhonePlayer)
			{
				if (platform == RuntimePlatform.Android)
				{
					Utils.CallAndroiSDKdFunc<bool>("IronsourceHelper", "isRewardVedioAdLoaded", out result, Array.Empty<object>());
				}
				else
				{
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}

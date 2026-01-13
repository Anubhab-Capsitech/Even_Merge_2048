using UnityEngine;


public class SimpleVibration : MonoBehaviour
{
 
 
#if UNITY_ANDROID && !UNITY_EDITOR
    private static AndroidJavaObject vibrator;
    private static AndroidJavaClass vibrationEffectClass;
#endif

    void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
            vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Vibration init failed: " + e.Message);
        }
#endif
    }

    public void VibrateSoft()
    {
        Debug.Log("Soft vibration triggered!");

#if UNITY_ANDROID && !UNITY_EDITOR
        try
        {
            if (vibrator == null)
            {
                Handheld.Vibrate();
                return;
            }

            // চেক করো ডিভাইস API 26 বা তার বেশি কি না
            AndroidJavaClass version = new AndroidJavaClass("android.os.Build$VERSION");
            int sdkInt = version.GetStatic<int>("SDK_INT");

            if (sdkInt >= 26 && vibrationEffectClass != null)
            {
                AndroidJavaObject vibrationEffect =
                    vibrationEffectClass.CallStatic<AndroidJavaObject>("createOneShot", 100L, 60);
                vibrator.Call("vibrate", vibrationEffect);
            }
            else
            {
                vibrator.Call("vibrate", 100L); // পুরনো Android এর জন্য fallback
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("Vibration error: " + e.Message);
            Handheld.Vibrate();
        }
#else
        Handheld.Vibrate();
#endif
    }
}


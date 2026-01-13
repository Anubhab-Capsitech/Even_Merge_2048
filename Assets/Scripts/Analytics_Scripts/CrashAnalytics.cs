using System;
using UnityEngine;
using Firebase.Crashlytics;

public class CrashAnalytics : MonoBehaviour
{
    public static CrashAnalytics Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else if (Instance != this) { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        // Subscribe to Unity logs safely
        Application.logMessageReceived += HandleUnityLog;
    }

    private void OnDisable()
    {
        // Unsubscribe to avoid callbacks on destroyed objects
        Application.logMessageReceived -= HandleUnityLog;
    }

    private void HandleUnityLog(string condition, string stackTrace, LogType type)
    {
        try
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
                // Crashlytics APIs are safe to call once Firebase is initialized.
                if (FirebaseManager.Instance != null && FirebaseManager.Instance.IsFirebaseReady())
                {
                    Crashlytics.Log(condition);
                    Crashlytics.Log(stackTrace);
                }
            }
        }
        catch (Exception ex)
        {
            // Never throw from a log handler
            Debug.LogWarning("[CrashAnalytics] Exception while sending log to Crashlytics: " + ex);
        }
    }

    /// <summary>
    /// Log a custom message to Crashlytics
    /// </summary>
    public void LogMessage(string message)
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsFirebaseReady())
        {
            return;
        }

        try
        {
            Crashlytics.Log(message);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[CrashAnalytics] Failed to log message: " + ex);
        }
    }

    /// <summary>
    /// Set a custom key-value pair for crash reports
    /// </summary>
    public void SetCustomKey(string key, string value)
    {
        if (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsFirebaseReady())
        {
            return;
        }

        try
        {
            Crashlytics.SetCustomKey(key, value);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[CrashAnalytics] Failed to set custom key: " + ex);
        }
    }

    /// <summary>
    /// Force a crash for testing - USE ONLY IN DEBUG BUILDS
    /// </summary>
    public void ForceCrashForTesting()
    {
        Debug.LogWarning("[CrashAnalytics] Forcing test crash...");
        throw new Exception("Manual Crash Test from CrashAnalytics");
    }
}

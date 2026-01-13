using UnityEngine;
using Firebase.Analytics;
using System;
using System.Collections;

public class PlayerAnalytics : MonoBehaviour
{
    public static PlayerAnalytics Instance;

    private DateTime sessionStartTime;
    private bool hasCheckedFirstSession = false;
    private bool sessionActive = false;

    private void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private IEnumerator Start()
    {
        // Wait until Firebase is initialized
        yield return StartCoroutine(WaitForFirebaseReady());

        // Now check & log first session
        yield return StartCoroutine(CheckAndRecordFirstSession());

        Debug.Log("✅ PlayerAnalytics started and first session checked.");
    }

    // -----------------------------------------------------------
    // WAIT FOR FIREBASE INITIALIZATION
    // -----------------------------------------------------------
    private IEnumerator WaitForFirebaseReady()
    {
        while (FirebaseManager.Instance == null || !FirebaseManager.Instance.IsFirebaseReady())
        {
            yield return null;
        }

        Debug.Log("✅ Firebase is ready in PlayerAnalytics.");
    }

    // -----------------------------------------------------------
    // ★ FIRST SESSION CHECK
    // -----------------------------------------------------------
    private IEnumerator CheckAndRecordFirstSession()
    {
        if (hasCheckedFirstSession)
            yield break;

        if (!PlayerPrefs.HasKey("first_session_logged"))
        {
            FirebaseAnalytics.LogEvent("first_session");
            PlayerPrefs.SetInt("first_session_logged", 1);
            Debug.Log("Analytics: first_session logged.");
        }

        hasCheckedFirstSession = true;
        yield return null;
    }

    // -----------------------------------------------------------
    // ★ SESSION START
    // -----------------------------------------------------------
    public void StartSession()
    {
        if (!FirebaseManager.Instance.IsFirebaseReady()) return;
        if (sessionActive) return;

        sessionActive = true;
        sessionStartTime = DateTime.UtcNow;

        FirebaseAnalytics.LogEvent("session_start");
        Debug.Log("Session started at: " + sessionStartTime.ToString("o"));
    }

    // -----------------------------------------------------------
    // ★ SESSION END
    // -----------------------------------------------------------
    public void EndSession()
    {
        if (!FirebaseManager.Instance.IsFirebaseReady()) return;
        if (!sessionActive) return;

        sessionActive = false;
        double durationSec = (DateTime.UtcNow - sessionStartTime).TotalSeconds;

        FirebaseAnalytics.LogEvent(
            "session_end",
            new Parameter("duration_sec", (int)durationSec)
        );

        FirebaseAnalytics.LogEvent("last_session_end");

        Debug.Log("Session ended. Duration (sec): " + durationSec);
    }

    // -----------------------------------------------------------
    // ★ MUSIC TOGGLE
    // -----------------------------------------------------------
    public void MusicSwitched(bool isOn)
    {
        if (!FirebaseManager.Instance.IsFirebaseReady()) return;

        FirebaseAnalytics.LogEvent(
            "music_switched",
            new Parameter("is_on", isOn ? 1 : 0)
        );

        Debug.Log("MusicSwitched called. isOn: " + isOn);
    }

    // -----------------------------------------------------------
    // ★ SFX TOGGLE
    // -----------------------------------------------------------
    public void SoundSwitched(bool isOn)
    {
        if (!FirebaseManager.Instance.IsFirebaseReady()) return;

        FirebaseAnalytics.LogEvent(
            "sfx_switched",
            new Parameter("is_on", isOn ? 1 : 0)
        );

        Debug.Log("SoundSwitched called. isOn: " + isOn);
    }

    // -----------------------------------------------------------
    // ★ CHAIN REACTION
    // -----------------------------------------------------------
    public void RegisterChainReaction()
    {
        if (!FirebaseManager.Instance.IsFirebaseReady()) return;

        FirebaseAnalytics.LogEvent("chain_reaction");
        Debug.Log("Chain reaction logged.");
    }

    // -----------------------------------------------------------
    // ★ PRIVACY POLICY BUTTON
    // -----------------------------------------------------------
    public void PrivacyPolicyCount()
    {
        if (!FirebaseManager.Instance.IsFirebaseReady()) return;

        FirebaseAnalytics.LogEvent("privacy_policy_click");
        Debug.Log("Privacy policy click logged.");
    }
}

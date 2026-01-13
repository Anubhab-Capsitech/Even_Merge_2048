using UnityEngine;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Database;
using System;
using System.Collections.Generic;
using System.Linq;

public class FirebaseManager : MonoBehaviour
{
    public static FirebaseManager Instance;
    private bool firebaseReady = false;
    private bool isInitializing = false;
    private FirebaseAuth auth;
    private DatabaseReference databaseReference;
    private FirebaseUser currentUser;

    public string CurrentUserId => currentUser != null ? currentUser.UserId : "";

    void Awake()
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

    void Start()
    {
        InitializeFirebase();
    }

    public void InitializeFirebase()
    {
        if (firebaseReady || isInitializing) return;
        
        isInitializing = true;
        Debug.Log("🔥 Starting Firebase initialization...");

        FirebaseApp.CheckAndFixDependenciesAsync()
            .ContinueWithOnMainThread(task =>
            {
                var status = task.Result;

                if (status == DependencyStatus.Available)
                {
                    FirebaseApp app = FirebaseApp.DefaultInstance;

                    // Enable Analytics
                    FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

                    // Initialize Auth
                    auth = FirebaseAuth.DefaultInstance;
                    
                    // Initialize Database
                    FirebaseDatabase database = FirebaseDatabase.DefaultInstance;
                    
                    // Disable persistence to ensure we always get fresh data from server
                    // This prevents cached data from showing stale information
                    database.SetPersistenceEnabled(false);
                    
                    databaseReference = database.RootReference;

                    Debug.Log("🔥 Firebase dependencies resolved. Persistence disabled.");

                    // Sign in anonymously if not already signed in
                    if (auth.CurrentUser == null)
                    {
                        SignInAnonymously();
                    }
                    else
                    {
                        currentUser = auth.CurrentUser;
                        Debug.Log($"🔥 User already signed in: {currentUser.UserId}");
                        firebaseReady = true;
                        isInitializing = false;
                    }
                }
                else
                {
                    Debug.LogError($"❌ Could not resolve all Firebase dependencies: {status}");
                    isInitializing = false;
                }
            });
    }

    private void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            isInitializing = false;

            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymously was canceled.");
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymously encountered an error: " + task.Exception);
                return;
            }

            currentUser = task.Result.User;
            firebaseReady = true;
            Debug.Log($"🔥 Signed in anonymously: {currentUser.UserId}");
        });
    }

    public void SaveUserProfile(UserProfile profile, Action<bool> callback)
    {
        if (!firebaseReady)
        {
             Debug.LogError("Firebase not ready.");
             callback?.Invoke(false);
             return;
        }

        if (currentUser == null)
        {
             Debug.LogError("User not signed in.");
             callback?.Invoke(false);
             return;
        }

        string json = JsonUtility.ToJson(profile);
        Debug.Log($"Attempting to save profile for {currentUser.UserId}: {json}");

        databaseReference.Child("users").Child(currentUser.UserId).SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"❌ Failed to save user profile. Exception: {task.Exception}");
                    foreach (var inner in task.Exception.InnerExceptions) {
                        Debug.LogError($"Inner: {inner.Message}");
                    }
                    callback?.Invoke(false);
                }
                else if (task.IsCanceled)
                {
                    Debug.LogError("❌ Save user profile canceled.");
                    callback?.Invoke(false);
                }
                else if (task.IsCompleted)
                {
                    Debug.Log("✅ User profile saved to Firebase.");
                    callback?.Invoke(true);
                }
                else
                {
                    // Should not happen, but covering bases
                    Debug.LogError("❌ Task finished in unknown state.");
                    callback?.Invoke(false);
                }
            });
    }

    public void GetUserProfile(Action<UserProfile> callback)
    {
        if (!firebaseReady || currentUser == null)
        {
            Debug.LogError("Firebase not ready or user not signed in.");
            callback?.Invoke(null);
            return;
        }

        string userId = currentUser.UserId;
        Debug.Log($"🔍 Fetching profile for user: {userId}");

        // GetValueAsync() should fetch from server, but we'll add a small delay
        // to ensure any previous cache is cleared
        DatabaseReference userRef = databaseReference.Child("users").Child(userId);
        
        // Force a fresh fetch by using GetValueAsync (which respects persistence settings)
        // Since we disabled persistence, this should always fetch from server
        userRef.GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"❌ Failed to fetch user profile. Exception: {task.Exception}");
                    callback?.Invoke(null);
                }
                else if (task.IsCanceled)
                {
                    Debug.LogError("❌ Fetch user profile was canceled.");
                    callback?.Invoke(null);
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    if (snapshot != null && snapshot.Exists)
                    {
                        try
                        {
                            string json = snapshot.GetRawJsonValue();
                            Debug.Log($"✅ Profile found in Firebase: {json}");
                            UserProfile profile = JsonUtility.FromJson<UserProfile>(json);
                            callback?.Invoke(profile);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError($"❌ Error parsing profile JSON: {e.Message}");
                            callback?.Invoke(null);
                        }
                    }
                    else
                    {
                        Debug.Log($"ℹ️ No profile found for user: {userId}");
                        callback?.Invoke(null);
                    }
                }
                else
                {
                    Debug.LogError("❌ Task finished in unknown state.");
                    callback?.Invoke(null);
                }
            });
    }

    public bool IsFirebaseReady()
    {
        return firebaseReady;
    }

    /// <summary>
    /// Fetches leaderboard data from Firebase, sorted by score (descending).
    /// </summary>
    /// <param name="scoreType">"2D" for highScore2D, "3D" for highScore3D</param>
    /// <param name="limit">Maximum number of entries to return (0 = unlimited)</param>
    /// <param name="callback">Callback with list of UserProfile sorted by score</param>
    public void GetLeaderboard(string scoreType, int limit, Action<List<UserProfile>> callback)
    {
        if (!firebaseReady)
        {
            Debug.LogError("Firebase not ready.");
            callback?.Invoke(new List<UserProfile>());
            return;
        }

        Debug.Log($"🔍 Fetching leaderboard for {scoreType} scores...");

        DatabaseReference usersRef = databaseReference.Child("users");
        
        // Note: For small databases, we fetch everything and sort in memory.
        // For larger databases, we would use OrderByChild("xp").LimitToLast(limit)
        usersRef.GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError($"❌ Failed to fetch leaderboard. Exception: {task.Exception}");
                    callback?.Invoke(new List<UserProfile>());
                }
                else if (task.IsCanceled)
                {
                    Debug.LogError("❌ Fetch leaderboard was canceled.");
                    callback?.Invoke(new List<UserProfile>());
                }
                else if (task.IsCompleted)
                {
                    DataSnapshot snapshot = task.Result;
                    List<UserProfile> leaderboard = new List<UserProfile>();

                    if (snapshot != null && snapshot.Exists)
                    {
                        foreach (DataSnapshot userSnapshot in snapshot.Children)
                        {
                            try
                            {
                                string json = userSnapshot.GetRawJsonValue();
                                UserProfile profile = JsonUtility.FromJson<UserProfile>(json);
                                
                                // Only include profiles with valid username
                                if (profile != null && !string.IsNullOrEmpty(profile.username))
                                {
                                    leaderboard.Add(profile);
                                }
                            }
                            catch (Exception e)
                            {
                                Debug.LogWarning($"⚠️ Error parsing user profile: {e.Message}");
                            }
                        }

                        // Sort by XP (descending)
                        leaderboard = leaderboard.OrderByDescending(p => p.xp).ToList();

                        // Apply limit if specified
                        if (limit > 0 && leaderboard.Count > limit)
                        {
                            leaderboard = leaderboard.Take(limit).ToList();
                        }

                        Debug.Log($"✅ Leaderboard fetched: {leaderboard.Count} entries");
                        callback?.Invoke(leaderboard);
                    }
                    else
                    {
                        Debug.Log("ℹ️ No users found in database");
                        callback?.Invoke(new List<UserProfile>());
                    }
                }
                else
                {
                    Debug.LogError("❌ Task finished in unknown state.");
                    callback?.Invoke(new List<UserProfile>());
                }
            });
    }
    /// <summary>
    /// Updates only the avatar ID of the current user.
    /// </summary>
    public void UpdateUserAvatarId(int avatarId, Action<bool> callback)
    {
        if (!firebaseReady || currentUser == null)
        {
            Debug.LogError("Firebase not ready or user not signed in.");
            callback?.Invoke(false);
            return;
        }

        databaseReference.Child("users").Child(currentUser.UserId).Child("avatarId").SetValueAsync(avatarId)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted || task.IsCanceled)
                {
                    Debug.LogError($"❌ Failed to update avatar ID. Exception: {task.Exception}");
                    callback?.Invoke(false);
                }
                else
                {
                    Debug.Log("✅ Avatar ID updated in Firebase.");
                    callback?.Invoke(true);
                }
            });
    }
}

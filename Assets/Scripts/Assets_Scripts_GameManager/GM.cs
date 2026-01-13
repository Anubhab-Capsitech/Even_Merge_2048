using Assets.Scripts.Configs;
using Assets.Scripts.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.GameManager
{
	internal class GM
	{
		private static GM g_intance;

		private int diamond;

		private int lv = 1;

		private float exp; // Changed to float

		// ... (keep other fields)
		private int m_consumeCount;

		private int m_gameId;

		private int m_skinID = 1;

		private bool m_isPlayVedioAds;

		private System.Random m_random = new System.Random();

		public float Exp
		{
			get
			{
				return this.exp;
			}
			set
			{
				this.exp = value;
			}
		}

        // ... (keep other properties)

        public bool CheckAndSetNewXP(float newXP)
        {
            // Only update if the new calculated XP is higher than our current saved XP
            if (newXP > this.Exp)
            {
                float oldExp = this.Exp;
                this.Exp = newXP;
                PlayerPrefs.SetFloat("LocalData_Exp", this.Exp); // Save as float
                
                GlobalEventHandle.EmitAddExpHandle(false);

                string oldTier = GetTierName(oldExp);
                string newTier = GetTierName(this.Exp);

                // Update Level index for compatibility
                this.Lv = GetTierIndex(this.Exp);
                PlayerPrefs.SetInt("LocalData_Lv", this.Lv);

                if (oldTier != newTier)
                {
                    GlobalEventHandle.EmitAddExpHandle(true); // Tier Up Popup
                    return true;
                }
            }
            return false;
        }

        // Deprecated AddExp - Redirect or remove usage
        public bool AddExp(int value)
        {
             // This method is deprecated as it implies cumulative addition. 
             // We will remove its logic to prevent accidental accumulation.
             return false;
        }

		public int Diamond
		{
			get
			{
				return this.diamond;
			}
			set
			{
				this.diamond = value;
			}
		}

		public int Lv
		{
			get
			{
				return this.lv;
			}
			set
			{
				this.lv = value;
			}
		}

		public int ConsumeCount
		{
			get
			{
				return this.m_consumeCount;
			}
			set
			{
				this.m_consumeCount = value;
			}
		}

		public int GameId
		{
			get
			{
				return this.m_gameId;
			}
			set
			{
				this.m_gameId = value;
			}
		}

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

		public bool IsPlayVedioAds
		{
			get
			{
				return this.m_isPlayVedioAds;
			}
			set
			{
				this.m_isPlayVedioAds = value;
			}
		}

		public static GM GetInstance()
		{
			if (GM.g_intance == null)
			{
				GM.g_intance = new GM();
			}
			return GM.g_intance;
		}

		public bool Init()
		{
			this.SaveInstallTime();
			this.LoadLocalData();
			this.InitEvent();
			return true;
		}

		public int AddDiamond(int value, bool isPlayAni = true)
		{
			this.Diamond += value;
			PlayerPrefs.SetInt("LocalData_Diamond", this.Diamond);
			GlobalEventHandle.EmitGetDiamondHandle(value, isPlayAni);
			Debug.Log("AddDiamond Added: " + value);
			LoadLocalData();
			return this.Diamond;
		}

		public int AddDiamondBase(int value)
		{
			this.Diamond += value;
			PlayerPrefs.SetInt("LocalData_Diamond", this.Diamond);
			return this.Diamond;
		}

		public void ConsumeGEM(int value)
		{
			this.Diamond -= value;
			PlayerPrefs.SetInt("LocalData_Diamond", this.Diamond);
			GlobalEventHandle.EmitConsumeDiamondHandle(value);
		}

		public bool isFullGEM(int value)
		{
			return this.Diamond >= value;
		}

        public string GetTierName(float xp)
        {
            return XPUtils.GetTierFromXP(xp);
        }

        public int GetTierIndex(float xp)
        {
            return XPUtils.GetTierIndex(xp);
        }

        public float GetTierProgress(float xp)
        {
            return XPUtils.GetTierProgress(xp);
        }

		public void ResetToNewGame()
		{
		}

		public void ResetConsumeCount()
		{
			this.ConsumeCount = 0;
		}

		public void AddConsumeCount()
		{
			int consumeCount = this.ConsumeCount + 1;
			this.ConsumeCount = consumeCount;
		}

		public bool isSavedGame()
		{
			return this.GetSavedGameID() != 0;
		}

		public int GetSavedGameID()
		{
			return PlayerPrefs.GetInt("LocalData_GameId", 0);
		}

		public void SetSavedGameID(int value)
		{
			this.GameId = value;
			PlayerPrefs.SetInt("LocalData_GameId", value);
		}

		public string GetSavedGameMap()
		{
			return PlayerPrefs.GetString("LocalData_OldGame", "");
		}

		public string GetSavedLife()
		{
			return PlayerPrefs.GetString("LocalData_OldLife", "");
		}

		public Vector2 GetSavedPos()
		{
			return new Vector2(PlayerPrefs.GetFloat("LocalData_OldPosX", 0f), PlayerPrefs.GetFloat("LocalData_OldPosY", 0f));
		}

		public void SaveGame(string value, string life, float x = -9999999f, float y = -9999999f)
		{
			if (!value.Equals(""))
			{
				PlayerPrefs.SetString("LocalData_OldGame", value);
			}
			if (!life.Equals(""))
			{
				PlayerPrefs.SetString("LocalData_OldLife", life);
			}
			if (x != -9999999f)
			{
				PlayerPrefs.SetFloat("LocalData_OldPosX", x);
			}
			if (y != -9999999f)
			{
				PlayerPrefs.SetFloat("LocalData_OldPosY", y);
			}
		}

		public void SaveScore(int gameID, int score)
		{
			string[] array = PlayerPrefs.GetString("LocalData_OldScore", "0,0,0").Split(new char[]
			{
				','
			});
			if (gameID > array.Length)
			{
				return;
			}
			array[gameID - 1] = score.ToString();
			PlayerPrefs.SetString("LocalData_OldScore", string.Format("{0},{1},{2}", array[0], array[1], array[2]));
		}

		public int GetScore(int gameID)
		{
			string[] arg_25_0 = PlayerPrefs.GetString("LocalData_OldScore", "0,0,0").Split(new char[]
			{
				','
			});
			List<int> list = new List<int>();
			string[] array = arg_25_0;
			for (int i = 0; i < array.Length; i++)
			{
				string value = array[i];
				list.Add(Convert.ToInt32(value));
			}
			if (gameID > list.Count)
			{
				return 0;
			}
			return list[gameID - 1];
		}

		/// <summary>
		/// Gets the high score for 2D game version.
		/// </summary>
		public int GetHighScore2D()
		{
			return GetScoreRecord(2); // 2D game high score
		}

		/// <summary>
		/// Gets the high score for 3D game version.
		/// </summary>
		public int GetHighScore3D()
		{
			return PlayerPrefs.GetInt("heightScoreGet", 0); // 3D game high score
		}

		/// <summary>
		/// Gets the unified high score across all games (2D and 3D).
		/// Returns the maximum score from both game modes.
		/// Kept for backward compatibility.
		/// </summary>
		public int GetUnifiedHighScore()
		{
			int score2D = GetHighScore2D();
			int score3D = GetHighScore3D();
			return Mathf.Max(score2D, score3D);
		}

		/// <summary>
		/// Saves high score for 2D game version.
		/// Updates the high score if the new score is higher, and syncs to Firebase.
		/// </summary>
		public void SaveHighScore2D(int score)
		{
			int currentHigh = GetHighScore2D();
			
			if (score > currentHigh)
			{
				// Save to PlayerPrefs directly (don't call SaveScoreRecord to avoid circular call)
				string[] array = PlayerPrefs.GetString("LocalData_Record_Score", "0,0,0").Split(new char[] { ',' });
				if (array.Length >= 2)
				{
					array[1] = score.ToString(); // Index 1 is gameID 2
					PlayerPrefs.SetString("LocalData_Record_Score", string.Format("{0},{1},{2}", array[0], array[1], array[2]));
					PlayerPrefs.Save();
					
					// Sync to Firebase
					SaveHighScoresToFirebase(score);
					
					Debug.Log($"[HighScore2D] New 2D high score: {score} (was {currentHigh})");
				}
			}
		}

		/// <summary>
		/// Saves high score for 3D game version.
		/// Updates the high score if the new score is higher, and syncs to Firebase.
		/// </summary>
		public void SaveHighScore3D(int score)
		{
			int currentHigh = GetHighScore3D();

			Debug.Log($"[SaveHighScore3D] Attempting to save 3D score: {score}, currentHigh: {currentHigh}");

			if (score >= currentHigh)
			{
				// Save to PlayerPrefs
				PlayerPrefs.SetInt("heightScoreGet", score);
				PlayerPrefs.Save();

				Debug.Log($"[HighScore3D] Saved to PlayerPrefs: {score}. Now syncing to Firebase...");

				// Directly push ONLY the 3D high score to Firebase (simple and robust).
				// This avoids depending on any other state and mirrors the working 2D path.
				if (FirebaseManager.Instance != null &&
				    FirebaseManager.Instance.IsFirebaseReady() &&
				    !string.IsNullOrEmpty(FirebaseManager.Instance.CurrentUserId))
				{
					FirebaseManager.Instance.GetUserProfile(profile =>
					{
						if (profile != null)
						{
							// Update just the 3D high score here
							profile.highScore3D = score;

                            // XP Calculation for 3D: xp = score * 2.72 / 1000
                            float newXP = score * 2.72f / 1000f;
                            if (newXP > profile.xp)
                            {
                                profile.xp = newXP;
                                CheckAndSetNewXP(newXP); // Sync local GM Exp
                                Debug.Log($"[XP] Updated XP from 3D score. New XP: {profile.xp}");
                            }

							// Keep totalScore as the max of both (for backward compatibility)
							int best2D = profile.highScore2D;
							// profile.totalScore = Mathf.Max(best2D, profile.highScore3D);

                            // Update Tier in profile before saving to Firebase
                            profile.tier = GetTierName(profile.xp);

							FirebaseManager.Instance.SaveUserProfile(profile, success =>
							{
								if (success)
								{
									Debug.Log($"[Firebase] ✅ 3D high score & XP saved successfully: {profile.highScore3D}, XP: {profile.xp}");
								}
								else
								{
									Debug.LogError("[Firebase] ❌ Failed to save 3D high score to Firebase");
								}
							});
						}
						else
						{
							Debug.LogWarning("[Firebase] User profile not found when saving 3D high score");
						}
					});
				}
				else
				{
					Debug.LogWarning("[Firebase] Firebase not ready or user not signed in when saving 3D score");
				}

				Debug.Log($"[HighScore3D] New 3D high score: {score} (was {currentHigh})");
			}
			else
			{
				Debug.Log($"[HighScore3D] Score {score} is not higher than current high {currentHigh}, skipping save");
			}
		}

		/// <summary>
		/// Saves a unified high score that works across both 2D and 3D games.
		/// DEPRECATED: Use SaveHighScore2D or SaveHighScore3D instead.
		/// Kept for backward compatibility.
		/// </summary>
		public void SaveUnifiedHighScore(int score)
		{
			// Determine which game mode we're in based on GameId
			// GameId 2 = 2D game, GameId 3 = 3D game
			if (this.GameId == 2)
			{
				SaveHighScore2D(score);
			}
			else if (this.GameId == 3)
			{
				SaveHighScore3D(score);
			}
			else
			{
				// Fallback: try to determine from current scene or save to both
				// This maintains backward compatibility
				int currentUnifiedHigh = GetUnifiedHighScore();
				if (score > currentUnifiedHigh)
				{
					PlayerPrefs.SetInt("UnifiedHighScore", score);
					SaveHighScoresToFirebase();
					Debug.Log($"[UnifiedHighScore] New unified high score: {score} (was {currentUnifiedHigh})");
				}
			}
		}

		/// <summary>
		/// Saves both 2D and 3D high scores to Firebase database with retry mechanism.
		/// Retries up to 3 times if Firebase is not ready.
		/// </summary>
		private void SaveHighScoresToFirebaseWithRetry(int retryCount = 0, int maxRetries = 3)
		{
			if (retryCount >= maxRetries)
			{
				Debug.LogError($"[Firebase] Max retries ({maxRetries}) reached. Failed to save high scores to Firebase.");
				return;
			}
			
			// Find a MonoBehaviour to run the coroutine (prefer FirebaseManager, otherwise any MonoBehaviour)
			MonoBehaviour coroutineRunner = FirebaseManager.Instance != null ? FirebaseManager.Instance : UnityEngine.Object.FindObjectOfType<MonoBehaviour>();
			
			if (FirebaseManager.Instance == null)
			{
				Debug.LogWarning($"[Firebase] FirebaseManager.Instance is null (retry {retryCount}/{maxRetries}), will retry...");
				if (coroutineRunner != null)
				{
					coroutineRunner.StartCoroutine(RetrySaveAfterDelay(retryCount, maxRetries));
				}
				else
				{
					Debug.LogError("[Firebase] No MonoBehaviour found to run coroutine. Cannot retry Firebase save.");
				}
				return;
			}
			
			if (!FirebaseManager.Instance.IsFirebaseReady())
			{
				Debug.LogWarning($"[Firebase] Firebase is not ready yet (retry {retryCount}/{maxRetries}), will retry...");
				if (coroutineRunner != null)
				{
					coroutineRunner.StartCoroutine(RetrySaveAfterDelay(retryCount, maxRetries));
				}
				return;
			}
			
			string userId = FirebaseManager.Instance.CurrentUserId;
			if (string.IsNullOrEmpty(userId))
			{
				Debug.LogWarning($"[Firebase] CurrentUserId is empty (retry {retryCount}/{maxRetries}), will retry...");
				if (coroutineRunner != null)
				{
					coroutineRunner.StartCoroutine(RetrySaveAfterDelay(retryCount, maxRetries));
				}
				return;
			}
			
			// Firebase is ready, proceed with save
			SaveHighScoresToFirebase();
		}
		
		/// <summary>
		/// Coroutine to retry Firebase save after a delay.
		/// </summary>
		private IEnumerator RetrySaveAfterDelay(int retryCount, int maxRetries)
		{
			yield return new WaitForSeconds(1f); // Wait 1 second before retry
			SaveHighScoresToFirebaseWithRetry(retryCount + 1, maxRetries);
		}

		/// <summary>
		/// Saves both 2D and 3D high scores to Firebase database.
        /// Optionally takes the current 2D score to calculate XP accurately if it's the trigger.
		/// Also saves current level if available.
		/// This is public so UI flows (like ProfileStats) can force a sync before reading.
		/// </summary>
		public void SaveHighScoresToFirebase(int current2DScore = -1)
		{
			if (FirebaseManager.Instance == null)
			{
				Debug.LogWarning("[Firebase] FirebaseManager.Instance is null, cannot save high scores");
				return;
			}
			
			if (!FirebaseManager.Instance.IsFirebaseReady())
			{
				Debug.LogWarning("[Firebase] Firebase is not ready yet, cannot save high scores");
				return;
			}
			
			string userId = FirebaseManager.Instance.CurrentUserId;
			if (string.IsNullOrEmpty(userId))
			{
				Debug.LogWarning("[Firebase] CurrentUserId is empty, cannot save high scores");
				return;
			}
			
			int score2D = GetHighScore2D();
			int score3D = GetHighScore3D();
			Debug.Log($"[Firebase] Attempting to save scores - 2D: {score2D}, 3D: {score3D}, UserId: {userId}");
			
			FirebaseManager.Instance.GetUserProfile(profile =>
			{
				if (profile != null)
				{
					// Update both 2D and 3D high scores
					profile.highScore2D = score2D;
					profile.highScore3D = score3D;
					
                    // XP Calculation
                    // If called from SaveHighScore2D, we use the passed score which is presumably the new high score
                    int valid2DScore = current2DScore > 0 ? current2DScore : score2D;
                    
                    // 2D XP: score * 7.65 / 1000
                    float newXP2D = valid2DScore * 7.65f / 1000f;
                    
                    if (newXP2D > profile.xp)
                    {
                        profile.xp = newXP2D;
                        CheckAndSetNewXP(newXP2D); // Sync local GM Exp
                        Debug.Log($"[XP] Updated XP from 2D score. New XP: {profile.xp}");
                    }
                    
                    // Note: 3D XP update happens in SaveHighScore3D because it has its own flow.
                    // But if we wanted to be safe we could also check 3D score here, but usually this is called for 2D updates or general sync.
                    
					// Also update totalScore for backward compatibility (max of both)
					// profile.totalScore = Mathf.Max(score2D, score3D);
					
                    // Update Tier
                    profile.tier = GetTierName(profile.xp); // Uses the updated XP
					
					Debug.Log($"[Firebase] Updating profile - 2D: {profile.highScore2D}, 3D: {profile.highScore3D}, Tier: {profile.tier}, XP: {profile.xp}");
					
					FirebaseManager.Instance.SaveUserProfile(profile, success =>
					{
						if (success)
						{
							Debug.Log($"[Firebase] ✅ High scores saved successfully - 2D: {profile.highScore2D}, 3D: {profile.highScore3D}, XP: {profile.xp}");
						}
						else
						{
							Debug.LogError("[Firebase] ❌ Failed to save high scores to Firebase");
						}
					});
				}
				else
				{
					Debug.LogWarning("[Firebase] User profile not found, cannot save high scores. UserId: " + userId);
				}
			});
		}

		/// <summary>
		/// Saves high score to Firebase database.
		/// DEPRECATED: Use SaveHighScoresToFirebase instead.
		/// Kept for backward compatibility.
		/// </summary>
		private void SaveHighScoreToFirebase(int highScore)
		{
			// Redirect to new method that saves both scores
			SaveHighScoresToFirebase();
		}

		public void SaveScoreRecord(int gameID, int score)
		{
			string[] array = PlayerPrefs.GetString("LocalData_Record_Score", "0,0,0").Split(new char[]
			{
				','
			});
			if (gameID > array.Length)
			{
				return;
			}
			if (score > Convert.ToInt32(array[gameID - 1]))
			{
				array[gameID - 1] = score.ToString();
				if (gameID == 3)
				{
					///AppsflyerUtils.TrackNewLevel(3, score);
				}
				PlayerPrefs.SetString("LocalData_Record_Score", string.Format("{0},{1},{2}", array[0], array[1], array[2]));
				PlayerPrefs.Save();
				GlobalEventHandle.EmitRefreshMaxScoreHandle(array);
				
				// Save to Firebase if this is 2D game (gameID == 2) or 3D game (gameID == 3)
				if (gameID == 2)
				{
					// For 2D, save to Firebase directly (SaveHighScore2D would check again and fail)
					SaveHighScoresToFirebase();
					Debug.Log($"[SaveScoreRecord] 2D high score saved: {score}");
				}
				else if (gameID == 3)
				{
					// For 3D, also save to Firebase
					SaveHighScoresToFirebase();
					Debug.Log($"[SaveScoreRecord] 3D high score saved: {score}");
				}
			}
		}

		public int GetScoreRecord(int gameID)
		{
			string[] arg_25_0 = PlayerPrefs.GetString("LocalData_Record_Score", "0,0,0").Split(new char[]
			{
				','
			});
			List<int> list = new List<int>();
			string[] array = arg_25_0;
			for (int i = 0; i < array.Length; i++)
			{
				string value = array[i];
				list.Add(Convert.ToInt32(value));
			}
			if (gameID > list.Count)
			{
				return 0;
			}
			return list[gameID - 1];
		}

		public void SaveG003ScoreRecord(int LvID, int CheckPoint)
		{
			string @string = PlayerPrefs.GetString("LocalData_G003_Record_Score_" + LvID.ToString(), "-1");
			if (CheckPoint > Convert.ToInt32(@string))
			{
				///AppsflyerUtils.TrackNewLevel(3, CheckPoint);
				PlayerPrefs.SetString("LocalData_G003_Record_Score_" + LvID.ToString(), CheckPoint.ToString());
			}
		}

		public int GetG003ScoreRecord(int LvID)
		{
			return Convert.ToInt32(PlayerPrefs.GetString("LocalData_G003_Record_Score_" + LvID.ToString(), "-1"));
		}

		public bool isFirstGame()
		{
			return PlayerPrefs.GetInt("LocalData_FirstGame", 0) == 0;
		}

		public void SetFristGame()
		{
			PlayerPrefs.SetInt("LocalData_FirstGame", 1);
		}

		public bool IsFirstFinishGame()
		{
			return PlayerPrefs.GetInt("LocalData_IsFirstFinish", 0) < 2;
		}

		public void SetFirstFinishGame()
		{
			int @int = PlayerPrefs.GetInt("LocalData_IsFirstFinish", 0);
			if (@int < 10)
			{
				PlayerPrefs.SetInt("LocalData_IsFirstFinish", @int + 1);
			}
		}

		public int GetLocalSkinID()
		{
			return PlayerPrefs.GetInt("LocalData_SkinID", 1);
		}

		public void SetLocalSkinID(int id)
		{
			this.SkinID = id;
			PlayerPrefs.SetInt("LocalData_SkinID", id);
			Action expr_17 = GlobalEventHandle.DoTransiformSkin;
			if (expr_17 == null)
			{
				return;
			}
			expr_17();
		}

        public bool IsProfileCreated()
        {
            return PlayerPrefs.GetInt("ProfileCreated", 0) == 1;
        }

        public void SetProfileCreated()
        {
            PlayerPrefs.SetInt("ProfileCreated", 1);
        }

		public void SaveInstallTime()
		{
			if (PlayerPrefs.GetString("LocalData_InitTime", "-1").Equals("-1"))
			{
				PlayerPrefs.SetString("LocalData_InitTime", DateTime.Now.ToString("yyyy-MM-dd"));
			}
		}

		private DateTime GetInstallTime()
		{
			string @string = PlayerPrefs.GetString("LocalData_InitTime", "-1");
			if (@string.Equals("-1"))
			{
				return DateTime.Now;
			}
			string[] array = @string.Split(new char[]
			{
				'-'
			});
			return new DateTime(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]));
		}

		public bool IsNewUser()
		{
			return (DateTime.Now - this.GetInstallTime()).Days == 0;
		}

		public List<int> GetSkinData()
		{
			List<int> list = new List<int>();
			string[] array = PlayerPrefs.GetString("LocalData_SkinData", "0,1").Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				string value = array[i];
				list.Add(Convert.ToInt32(value));
			}
			return list;
		}

		public void SetSkinData(int skinID, int status)
		{
			string[] array = PlayerPrefs.GetString("LocalData_SkinData", "0,1").Split(new char[]
			{
				','
			});
			if (skinID > array.Length)
			{
				return;
			}
			array[skinID - 1] = status.ToString();
			PlayerPrefs.SetString("LocalData_SkinData", string.Format("{0},{1}", array[0], array[1]));
		}

		public DateTime GetSkinFreeTime(int skinID)
		{
			List<DateTime> list = new List<DateTime>();
			string[] array = PlayerPrefs.GetString("LocalData_SkinFreeTime", "-1,-1").Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (text.Equals("-1"))
				{
					list.Add(DateTime.Now);
				}
				else
				{
					string[] array2 = text.Split(new char[]
					{
						'-'
					});
					list.Add(new DateTime(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2])));
				}
			}
			return list[skinID - 1];
		}

		public void SetSkinFreeTime(int skinID, DateTime time)
		{
			List<DateTime> list = new List<DateTime>();
			string[] array = PlayerPrefs.GetString("LocalData_SkinFreeTime", "-1,-1").Split(new char[]
			{
				','
			});
			for (int i = 0; i < array.Length; i++)
			{
				string text = array[i];
				if (text.Equals("-1"))
				{
					list.Add(DateTime.Now);
				}
				else
				{
					string[] array2 = text.Split(new char[]
					{
						'-'
					});
					list.Add(new DateTime(int.Parse(array2[0]), int.Parse(array2[1]), int.Parse(array2[2])));
				}
			}
			if (skinID > list.Count)
			{
				return;
			}
			list[skinID - 1] = time;
			string text2 = "";
			for (int j = 0; j < list.Count; j++)
			{
				text2 += list[j].ToString("yyyy-MM-dd");
				if (j < list.Count - 1)
				{
					text2 += ",";
				}
			}
			PlayerPrefs.SetString("LocalData_SkinFreeTime", text2);
		}

		public void ResetSkinFreeTime()
		{
			List<int> skinData = this.GetSkinData();
			for (int i = 0; i < skinData.Count; i++)
			{
				if (skinData[i] == 2)
				{
					DateTime skinFreeTime = this.GetSkinFreeTime(i + 1);
					if ((DateTime.Now - skinFreeTime.AddDays(3.0)).Days > 0)
					{
						this.SetSkinData(i + 1, 1);
						if (this.SkinID == i + 1)
						{
							this.SetLocalSkinID(1);
						}
					}
				}
			}
		}

		public bool isFirstShare()
		{
			return PlayerPrefs.GetInt("LocalData_FirstShare", 0) == 0;
		}

		public void ResetFirstShare(int value = 0)
		{
			PlayerPrefs.SetInt("LocalData_FirstShare", value);
		}

		public bool IsRandomStatus(int value)
		{
			return this.m_random.Next(1, 100) < value;
		}

		public void LoadLocalData()
		{
			this.Diamond = PlayerPrefs.GetInt("LocalData_Diamond", 0);
			this.Lv = PlayerPrefs.GetInt("LocalData_Lv", 1);
			this.Exp = PlayerPrefs.GetFloat("LocalData_Exp", 0f); // Load as float
			this.GameId = this.GetSavedGameID();
			this.SkinID = this.GetLocalSkinID();
		}

		private void InitEvent()
		{
		}
	}
}

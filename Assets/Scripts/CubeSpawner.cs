using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class CubeData
{
    public int number;
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;

    public CubeData(int number, Vector3 position, Quaternion rotation, Vector3 scale)
    {
        this.number = number;
        this.position = position;
        this.rotation = rotation;
        this.scale = scale;
    }
}

[System.Serializable]
public class CubeDataList
{
    public List<CubeData> cubes;
    public CubeDataList(List<CubeData> cubes)
    {
        this.cubes = cubes;
    }
}

public class CubeSpawner : MonoBehaviour
{
    public static CubeSpawner Instance;

    [Header("Spawner Settings")]
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Color[] cubeColors;
    [SerializeField] private int cubesQueueCapacity = 20;
    [SerializeField] private bool autoQueueGrow = true;

    [HideInInspector] public int maxCubeNumber;
    private Queue<Cube> cubesQueue = new Queue<Cube>();
    private List<Cube> activeCubes = new List<Cube>();
    private Cube lastSpawnedCube;
    private readonly float fixedScale = 0.412f;
    private Vector3 defaultSpawnPosition;

    private const string PlayerPrefsKey = "SavedCubes";
    private const string SpawnOnceKey = "PatternAlreadySpawned";

    [Header("Random Cube Spawn Point")]
    public Transform randoncube;

    private void Awake()
    {
      
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        defaultSpawnPosition = transform.position;
        maxCubeNumber = (int)Mathf.Pow(2, 12);
        InitializeCubesQueue();
        LoadFromPlayerPrefs();

        // CRITICAL FIX: If we just loaded cubes from a save, ensure the spawner 
        // knows the "initial pattern" should be skipped, to prevent double-spawning.
        if (activeCubes.Count > 0)
        {
            PlayerPrefs.SetInt(SpawnOnceKey, 1);
            PlayerPrefs.Save();
        }

 
        bool hasSpawnedBefore = PlayerPrefs.GetInt(SpawnOnceKey, 0) == 1;

        if (!hasSpawnedBefore && randoncube != null)
        {
            SpawnPattern();
            PlayerPrefs.SetInt(SpawnOnceKey, 1);
            PlayerPrefs.Save();
        }
        else if (!hasSpawnedBefore)
        {
            Debug.LogWarning("❌ randoncube Transform not assigned - Cannot spawn pattern!");
        }
        else
        {
            Debug.Log("🟡 Pattern already spawned earlier — skipped!");
        }

        // Firebase Analytics - Start session for 3D game
        PlayerAnalytics.Instance?.StartSession();
    }

    #region Queue Initialization
    private void InitializeCubesQueue()
    {
        for (int i = 0; i < cubesQueueCapacity; i++)
            AddCubeToQueue();
    }

    private void AddCubeToQueue()
    {
        if (cubePrefab == null)
        {
            Debug.LogError("CubePrefab is not assigned!");
            return;
        }

        GameObject cubeGO = Instantiate(cubePrefab, defaultSpawnPosition, Quaternion.identity, transform);
        Cube cube = cubeGO.GetComponent<Cube>();

        if (cube == null)
        {
            Debug.LogError("CubePrefab does not have a Cube component!");
            Destroy(cubeGO);
            return;
        }

        cube.gameObject.SetActive(false);
        cube.IsMainCube = false;
        cube.transform.localScale = Vector3.one * fixedScale;
        cubesQueue.Enqueue(cube);
    }
    #endregion

    #region Spawn & Destroy
    public Cube Spawn(int number, Vector3 position, bool trackAsLastSpawned = true, bool animateScale = true)
    {
        if (HeightScore.Instance != null && HeightScore.Instance.IsGameOver) return null;

        if (cubesQueue.Count == 0)
        {
            if (autoQueueGrow)
            {
                cubesQueueCapacity++;
                AddCubeToQueue();
            }
            else
            {
                return null;
            }
        }

        Cube cube = cubesQueue.Dequeue();
        if (cube == null)
        {
            return null;
        }

        cube.transform.position = position;
        cube.SetNumber(number);
        cube.SetColor(GetColor(number));
        cube.transform.rotation = Quaternion.identity;

        Rigidbody rb = cube.CubeRigidbody;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        cube.IsMainCube = true;
        cube.HasBeenLaunched = false; // CRITICAL FIX: Reset launched state for pooled cubes
        cube.gameObject.SetActive(true);

        ParticleSystem ps = cube.GetComponentInChildren<ParticleSystem>();
        if (ps != null) ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (trackAsLastSpawned) lastSpawnedCube = cube;
        if (!activeCubes.Contains(cube)) activeCubes.Add(cube);

      
        if (animateScale)
        {
            cube.transform.localScale = Vector3.one * 0.2f;
            StartCoroutine(ScaleUpCube(cube));
        }
        else
        {
            cube.transform.localScale = Vector3.one * fixedScale;
        }

        return cube;
    }

    private System.Collections.IEnumerator ScaleUpCube(Cube cube)
    {
        float duration = 0.1f;
        float time = 0f;
        Vector3 startScale = Vector3.one * 0.2f;
        Vector3 endScale = Vector3.one * fixedScale;

        while (time < duration)
        {
            if (cube == null) yield break;
            cube.transform.localScale = Vector3.Lerp(startScale, endScale, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        if (cube != null)
            cube.transform.localScale = endScale;
    }

    public Cube SpawnRandom()
    {
        if (HeightScore.Instance != null && HeightScore.Instance.IsGameOver) return null;

        int randomNumber = GenerateRandomNumber();
        return Spawn(randomNumber, defaultSpawnPosition);
    }

    public void DestroyCube(Cube cube)
    {
        if (cube == null) return;

        Rigidbody rb = cube.CubeRigidbody;
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        cube.transform.rotation = Quaternion.identity;
        cube.IsMainCube = false;
        cube.transform.localScale = Vector3.one * fixedScale;
        cube.gameObject.SetActive(false);

        activeCubes.Remove(cube);
        cubesQueue.Enqueue(cube);
    }
    #endregion

    #region Random Pattern Spawning
    public void SpawnPattern()
    {
        if (randoncube == null)
        {
            Debug.LogWarning("❌ randoncube Transform not assigned!");
            return;
        }

        int patternType = Random.Range(0, 3);  
        switch (patternType)
        {
            case 0: SpawnSquarePattern(); break;
            case 1: SpawnTrianglePattern(); break;
            case 2: SpawnStackedPattern(); break;
        }

        Debug.Log("🎯 Pattern Spawned & Saved!");
        SaveToPlayerPrefs();
    }

    private void SpawnSquarePattern()
    {
        float spacing = 0.6f;
        Vector3 basePos = randoncube.position;
        Vector3[] offsets = new Vector3[]
        {
            new Vector3(-spacing, 0, -spacing),
            new Vector3(spacing, 0, -spacing),
            new Vector3(-spacing, 0, spacing),
            new Vector3(spacing, 0, spacing)
        };

        foreach (Vector3 offset in offsets)
        {
            Cube cube = Spawn(GenerateRandomNumber(), basePos + offset, false, false); 
            if (cube != null) cube.HasBeenLaunched = true;
        }

        Debug.Log("🟩 Spawned Square Pattern!");
    }

    private void SpawnTrianglePattern()
    {
        float spacing = 0.6f;
        Vector3 basePos = randoncube.position;

        Vector3[] offsets = new Vector3[]
        {
            new Vector3(0, 0, spacing),
            new Vector3(-spacing, 0, -spacing),
            new Vector3(spacing, 0, -spacing)
        };

        foreach (Vector3 offset in offsets)
        {
            Cube cube = Spawn(GenerateRandomNumber(), basePos + offset, false, false); 
            if (cube != null) cube.HasBeenLaunched = true;
        }

        Debug.Log("🔺 Spawned Triangle Pattern!");
    }

    private void SpawnStackedPattern()
    {
        Vector3 basePos = randoncube.position;
        float height = 0.45f;

        int firstNumber = GenerateRandomNumber();
        int secondNumber = GenerateRandomNumber();

        // Ensure both are different
        while (secondNumber == firstNumber)
            secondNumber = GenerateRandomNumber();

        for (int i = 0; i < 2; i++)
        {
            int numberToUse = (i == 0) ? firstNumber : secondNumber;
            Vector3 pos = basePos + new Vector3(0, i * height, 0);
            Cube cube = Spawn(numberToUse, pos, false, false); 
            if (cube != null) cube.HasBeenLaunched = true;
        }

        Debug.Log(" Spawned Stacked (Vertical) Pattern!");
    }
    #endregion

    #region Helpers
    public int GenerateRandomNumber()
    {
        return (int)Mathf.Pow(2, Random.Range(1, 6));
    }

    private Color GetColor(int number)
    {
        if (number < 2 || cubeColors == null || cubeColors.Length == 0)
            return Color.white;

        int logVal = (int)Mathf.Log(number, 2);
        int index = logVal - 1;

        if (index < 0 || index >= cubeColors.Length)
        {
            Debug.LogWarning("Cube number {number} exceeds color array. Using white.");
            return Color.white;
        }
        return cubeColors[index];
    }
    #endregion

    #region Save & Load
    public List<CubeData> SaveAllActiveCubes()
    {
        List<CubeData> cubesData = new List<CubeData>();

        foreach (Cube cube in activeCubes)
        {
           
            // CRITICAL FIX: Only save cubes that have been launched (i.e. are on the board).
            // Do NOT save the cube currently waiting at the launchpad (!HasBeenLaunched).
            // This prevents saving the "waiting" cube which would then be loaded as a "dumb" board cube,
            // causing a double-spawn when the game starts and spawns a NEW waiting cube on top of it.
            if (cube == null || !cube.gameObject.activeSelf || !cube.HasBeenLaunched)
                continue;

            cubesData.Add(new CubeData(
                cube.CubeNumber,
                cube.transform.position,
                cube.transform.rotation,
                Vector3.one * fixedScale // CRITICAL FIX: Save the target scale, not the current (potentially intermediate) scale
            ));
        }

        Debug.Log(" {cubesData.Count} cubes (excluding last spawned one)");
        return cubesData;
    }


    public void SaveToPlayerPrefs()
    {
        CubeDataList wrapper = new CubeDataList(SaveAllActiveCubes());
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(PlayerPrefsKey, json);
        PlayerPrefs.Save();
        Debug.Log(" Cubes saved: {wrapper.cubes.Count}");
    }

    public void LoadFromPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey(PlayerPrefsKey))
        {
            Debug.Log(" No saved cube data found.");
            return;
        }

        string json = PlayerPrefs.GetString(PlayerPrefsKey);
        if (string.IsNullOrEmpty(json))
        {
            Debug.LogWarning(" Saved cube data is empty ");
            return;
        }

        CubeDataList wrapper = JsonUtility.FromJson<CubeDataList>(json);
        if (wrapper == null || wrapper.cubes == null)
        {
            Debug.LogError(" Failed to parse saved cube data ");
            return;
        }

        foreach (Cube cube in new List<Cube>(activeCubes))
            DestroyCube(cube);

        foreach (CubeData data in wrapper.cubes)
        {
            Cube cube = Spawn(data.number, data.position, false, false);
            if (cube != null)
            {
                cube.transform.rotation = data.rotation;
                cube.transform.localScale = Vector3.one * fixedScale; // Ensure correct scale even if saved data was wrong
                cube.HasBeenLaunched = true;
                cube.IsMainCube = false; // Loaded board cubes are never the main launch cube
            }
        }

        Debug.Log(" Loaded cubes: {wrapper.cubes.Count}");
    }

    public void ClearSavedData()
    {
        PlayerPrefs.DeleteKey(PlayerPrefsKey);
        PlayerPrefs.DeleteKey(SpawnOnceKey);
        PlayerPrefs.Save();
        Debug.Log(" Saved cube data cleared — next time will spawn pattern again ");
    }
    #endregion

    private void OnApplicationPause(bool pause)
    {
        if (pause) SaveToPlayerPrefs();
    }

    private void OnApplicationQuit()
    {
        SaveToPlayerPrefs();
    }
}

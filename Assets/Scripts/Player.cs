using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[ExecuteAlways]
public class Player : MonoBehaviour
{
    public static Player Instance;

    [Header("Movement & Push")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float pushForce = 10f;
    [SerializeField] private float cubeMaxPosX = 3f;

    [Header("Touch Slider")]
    [SerializeField] private TouchSlider touchSlider;

    [Header("Custom Models")]
    [SerializeField] private GameObject customModelPrefab;
    [SerializeField] private GameObject secondCustomModelPrefab;

    [Header("UI Text")]
    [SerializeField] private TextMeshProUGUI firstModelUsageText;
    [SerializeField] private TextMeshProUGUI secondModelUsageText;

    [Header("Popup Settings")]
    [SerializeField] private Image popupImage;
    [SerializeField] private float popupDuration = 1.5f;
    [SerializeField] private float fadeSpeed = 2f;

    [Header("Model Icons (First Set)")]
    [SerializeField] private Image firstModelIcon;
    [SerializeField] private Image secondModelIcon;
    [SerializeField] private Sprite[] firstModelSprites = new Sprite[2];
    [SerializeField] private Sprite[] secondModelSprites = new Sprite[2];

    [Header("Model Icons (Second Set)")]
    [SerializeField] private Image firstModelIcon2;
    [SerializeField] private Image secondModelIcon2;
    [SerializeField] private Sprite[] firstModelSprites2 = new Sprite[2];
    [SerializeField] private Sprite[] secondModelSprites2 = new Sprite[2];

    private Cube mainCube;
    private GameObject customModel;
    private GameObject secondCustomModel;

    private bool isPointerDown;
    private bool canMove = true;
    private bool skipFirstDrag;
    private Vector3 cubePos;
    private Vector3 lastCubePos;
    private bool cubeDestroyedByColorBox = false;

    private bool usingCustomModel = false;
    private bool usingSecondCustomModel = false;

    private int firstModelUsageCount;
    private int secondModelUsageCount;

    private bool isPopupActive = false;


    private bool modelSwitchInProgress = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadData();
        SpawnCube();

        if (touchSlider != null)
        {
            touchSlider.OnPointerDownEvent += OnPointerDown;
            touchSlider.OnPointerDragEvent += OnPointerDrag;
            touchSlider.OnPointerUpEvent += OnPointerUp;
        }

        UpdateUsageText();
        UpdateModelIcons();
    }

    private void Update()
    {
        if (isPointerDown && canMove)
        {
            Transform active = GetCurrentActiveTransform();
            if (active != null)
                active.position = Vector3.Lerp(active.position, cubePos, moveSpeed * Time.deltaTime);
        }
    }

    private Transform GetCurrentActiveTransform()
    {
        if (mainCube != null) return mainCube.transform;
        if (customModel != null) return customModel.transform;
        if (secondCustomModel != null) return secondCustomModel.transform;
        return null;
    }

    #region Touch Controls
    public void OnPointerDown()
    {
        if (!canMove) return;
        isPointerDown = true;
        skipFirstDrag = true;

        Transform active = GetCurrentActiveTransform();
        if (active != null) cubePos = active.position;
    }

    private void OnPointerDrag(float xMovement)
    {
        if (!isPointerDown || !canMove) return;

        if (skipFirstDrag)
        {
            skipFirstDrag = false;
            return;
        }

        cubePos.x = Mathf.Clamp(xMovement * cubeMaxPosX, -cubeMaxPosX, cubeMaxPosX);
    }

    private void OnPointerUp()
    {
        if (!isPointerDown || !canMove) return;

        isPointerDown = false;
        canMove = false;

        GameObject spawnSound = GameObject.Find("spawn sound");
        spawnSound?.GetComponent<AudioSource>()?.Play();
        FindObjectOfType<SimpleVibration>()?.VibrateSoft();

        if (mainCube != null)
            LaunchMainCube();
        else if (customModel != null || secondCustomModel != null)
            LaunchCustomModel();

        Transform current = GetCurrentActiveTransform();
        if (current != null)
            lastCubePos = current.position;
    }
    #endregion

    #region Launch Logic
    private void LaunchMainCube()
    {
        if (mainCube == null) return;

        mainCube.EnableTrail();
        if (mainCube.CubeRigidbody != null)
            mainCube.CubeRigidbody.AddForce(Vector3.forward * pushForce, ForceMode.Impulse);

        mainCube.HasBeenLaunched = true;
        mainCube.IsMainCube = false;
        StartCoroutine(CheckIfCubeDestroyed());
    }

    private void LaunchCustomModel()
    {
        GameObject model = customModel != null ? customModel : secondCustomModel;
        if (model == null) return;

        Rigidbody rb = model.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.AddForce(Vector3.forward * pushForce, ForceMode.Impulse);
        }

        Cube cubeComp = model.GetComponent<Cube>();
        if (cubeComp != null)
        {
            cubeComp.HasBeenLaunched = true;
            StartCoroutine(CheckIfCustomModelDestroyed(model));
        }
        else
        {
            StartCoroutine(CheckIfCustomModelDestroyedWithTimeout(model));
        }
    }
    #endregion

    #region Cube Spawn / Destruction
    private IEnumerator CheckIfCubeDestroyed()
    {
        yield return new WaitForSeconds(0.5f);
        if (mainCube != null)
            mainCube.DisableTrail();

        if (mainCube != null && !cubeDestroyedByColorBox)
        {
            yield return new WaitForSeconds(0.3f);
            SpawnNewCube();
        }
        else
        {
            canMove = true;
            cubeDestroyedByColorBox = false;
        }
    }

    private IEnumerator CheckIfCustomModelDestroyed(GameObject model)
    {
        yield return new WaitForSeconds(0.5f);
        float timer = 0f;
        while (model != null && timer < 10f)
        {
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        canMove = true;
        usingCustomModel = false;
        usingSecondCustomModel = false;
        SpawnNewCube();
    }

    private IEnumerator CheckIfCustomModelDestroyedWithTimeout(GameObject model)
    {
        yield return new WaitForSeconds(0.5f);
        canMove = true;
        usingCustomModel = false;
        usingSecondCustomModel = false;
        SpawnNewCube();
    }

    private void SpawnNewCube()
    {
        if (HeightScore.Instance != null && HeightScore.Instance.IsGameOver) return;

        modelSwitchInProgress = false; 

        if (mainCube != null)
            mainCube.IsMainCube = false;

        canMove = true;
        cubeDestroyedByColorBox = false;

        if (usingCustomModel && customModelPrefab != null)
            ReplaceWithCustomModel();
        else if (usingSecondCustomModel && secondCustomModelPrefab != null)
            ReplaceWithSecondCustomModel();
        else
            SpawnCube();
    }

    private void SpawnCube()
    {
        if (HeightScore.Instance != null && HeightScore.Instance.IsGameOver) return;

        if (customModel != null) Destroy(customModel);
        if (secondCustomModel != null) Destroy(secondCustomModel);

        mainCube = CubeSpawner.Instance.SpawnRandom();
        if (mainCube == null) return;

        mainCube.IsMainCube = true;

        if (lastCubePos != Vector3.zero)
            mainCube.transform.position = lastCubePos;

        cubePos = mainCube.transform.position;
        mainCube.DisableTrail();

        usingCustomModel = false;
        usingSecondCustomModel = false;
    }
    #endregion

    #region Custom Models
    public void ReplaceWithCustomModel()
    {
        if (modelSwitchInProgress) return; 
        if (usingCustomModel || customModel != null) return;

        if (firstModelUsageCount <= 0 || isPopupActive)
        {
            ShowPopup();
            return;
        }

        modelSwitchInProgress = true;

        if (mainCube != null)
        {
            CubeSpawner.Instance.DestroyCube(mainCube);
            mainCube = null;
        }

        if (secondCustomModel != null)
            Destroy(secondCustomModel);

        Vector3 spawnPos = lastCubePos != Vector3.zero ? lastCubePos : transform.position;
        customModel = Instantiate(customModelPrefab, spawnPos, Quaternion.identity);

        Rigidbody rb = customModel.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        cubePos = customModel.transform.position;
        usingCustomModel = true;
        usingSecondCustomModel = false;

        firstModelUsageCount--;
        UpdateUsageText();
        SaveData();
        UpdateModelIcons();
    }

    public void ReplaceWithSecondCustomModel()
    {
        if (modelSwitchInProgress) return; 
        if (usingSecondCustomModel || secondCustomModel != null) return;

        if (secondModelUsageCount <= 0 || isPopupActive)
        {
            ShowPopup();
            return;
        }

        modelSwitchInProgress = true; 

        if (mainCube != null)
        {
            CubeSpawner.Instance.DestroyCube(mainCube);
            mainCube = null;
        }

        if (customModel != null)
            Destroy(customModel);

        Vector3 spawnPos = lastCubePos != Vector3.zero ? lastCubePos : transform.position;
        secondCustomModel = Instantiate(secondCustomModelPrefab, spawnPos, Quaternion.identity);

        Rigidbody rb = secondCustomModel.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        cubePos = secondCustomModel.transform.position;
        usingCustomModel = false;
        usingSecondCustomModel = true;

        secondModelUsageCount--;
        UpdateUsageText();
        SaveData();
        UpdateModelIcons();
    }
    #endregion

    #region Popup Logic
    private void ShowPopup()
    {
        if (popupImage == null || isPopupActive) return;

        isPopupActive = true;
        popupImage.gameObject.SetActive(true);
        popupImage.canvasRenderer.SetAlpha(0f);
        popupImage.CrossFadeAlpha(1f, 0.2f, false);
        StartCoroutine(HidePopupAfterDelay());
    }

    private IEnumerator HidePopupAfterDelay()
    {
        yield return new WaitForSeconds(popupDuration);
        popupImage.CrossFadeAlpha(0f, 0.3f, false);
        yield return new WaitForSeconds(0.3f);
        popupImage.gameObject.SetActive(false);
        isPopupActive = false;
    }
    #endregion

    #region Usage Management
    private void LoadData()
    {
        firstModelUsageCount = PlayerPrefs.GetInt("FirstModelUsageCount", 0);
        secondModelUsageCount = PlayerPrefs.GetInt("SecondModelUsageCount", 0);
        UpdateModelIcons();
    }

    private void SaveData()
    {
        PlayerPrefs.SetInt("FirstModelUsageCount", firstModelUsageCount);
        PlayerPrefs.SetInt("SecondModelUsageCount", secondModelUsageCount);
        PlayerPrefs.Save();
    }

    private void UpdateUsageText()
    {
        if (firstModelUsageText != null)
        {
            firstModelUsageText.text = firstModelUsageCount.ToString();
        }

        if (secondModelUsageText != null)
        {
            secondModelUsageText.text = secondModelUsageCount.ToString();
        }
    }

    private void UpdateModelIcons()
    {
        if (firstModelIcon != null && firstModelSprites.Length >= 2)
        {
            firstModelIcon.sprite = (firstModelUsageCount > 0) ? firstModelSprites[1] : firstModelSprites[0];
        }

        if (secondModelIcon != null && secondModelSprites.Length >= 2)
        {
            secondModelIcon.sprite = (secondModelUsageCount > 0) ? secondModelSprites[1] : secondModelSprites[0];
        }

        if (firstModelIcon2 != null && firstModelSprites2.Length >= 2)
        {
            firstModelIcon2.sprite = (firstModelUsageCount > 0) ? firstModelSprites2[1] : firstModelSprites2[0];
        }

        if (secondModelIcon2 != null && secondModelSprites2.Length >= 2)
        {
            secondModelIcon2.sprite = (secondModelUsageCount > 0) ? secondModelSprites2[1] : secondModelSprites2[0];
        }
    }

    public void AddFirstModelUses(int count)
    {
        firstModelUsageCount += count;
        UpdateUsageText();
        SaveData();
        UpdateModelIcons();
    }

    public void AddSecondModelUses(int count)
    {
        secondModelUsageCount += count;
        UpdateUsageText();
        SaveData();
        UpdateModelIcons();
    }

    public void ResetModelUses()
    {
        firstModelUsageCount = 0;
        secondModelUsageCount = 0;
        UpdateUsageText();
        SaveData();
        UpdateModelIcons();
    }
    #endregion

    public void NotifyCubeDestroyedByColorBox()
    {
        cubeDestroyedByColorBox = true;
        mainCube = null;

        if (customModel != null) Destroy(customModel);
        if (secondCustomModel != null) Destroy(secondCustomModel);

        usingCustomModel = false;
        usingSecondCustomModel = false;
    }

    private void OnDestroy()
    {
        if (touchSlider != null)
        {
            touchSlider.OnPointerDownEvent -= OnPointerDown;
            touchSlider.OnPointerDragEvent -= OnPointerDrag;
            touchSlider.OnPointerUpEvent -= OnPointerUp;
        }
    }

    private void OnApplicationQuit()
    {
        SaveData();
    }
}

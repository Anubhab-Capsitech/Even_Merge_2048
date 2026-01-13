using Assets.Scripts.GameManager;
using UnityEngine;

public class CubeCollision : MonoBehaviour
{
    private Cube cube;
    private SimpleVibration vibration;
    private AudioSource popSound;

    [Header("Color Settings")]
    public Color[] colors;

    [Header("Particle Settings")]
    public GameObject spawnParticlePrefab;

    private bool hasMerged = false; 

    private void Awake()
    {
        cube = GetComponent<Cube>();
        vibration = FindObjectOfType<SimpleVibration>();

        GameObject popObj = GameObject.Find("PopSound");
        if (popObj != null)
            popSound = popObj.GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasMerged) return;  

        Cube otherCube = collision.gameObject.GetComponent<Cube>();
        if (otherCube == null || cube.CubeID <= otherCube.CubeID)
            return;

        if (cube.CubeNumber == otherCube.CubeNumber)
        {
            CubeCollision otherCollision = otherCube.GetComponent<CubeCollision>();
            if (otherCollision != null && otherCollision.hasMerged)
                return;

            hasMerged = true;
            if (otherCollision != null)
                otherCollision.hasMerged = true;

            Invoke(nameof(ResetMergeFlag), 0.2f);
            if (otherCollision != null)
            {
                otherCollision.Invoke(nameof(ResetMergeFlag), 0.2f);
            }

            if (vibration != null)
                vibration.VibrateSoft();

            Vector3 contactPoint = collision.contacts.Length > 0
                ? collision.contacts[0].point
                : transform.position;

            if (otherCube.CubeNumber < CubeSpawner.Instance.maxCubeNumber)
            {
                Cube newCube = CubeSpawner.Instance.Spawn(
                    cube.CubeNumber * 2,
                    contactPoint + Vector3.up * 1f
                );

                if (newCube == null)
                {
                    Debug.LogWarning("CubeCollision: Spawn returned null, aborting merge effects.");
                    return;
                }

                // CRITICAL FIX: Merged cubes must be marked as launched so RedZone can detect them
                // Both parent cubes were launched, so the merged result is also considered launched
                newCube.HasBeenLaunched = true;
                newCube.IsMainCube = false; // Merged cubes are not the main launch cube

                if (spawnParticlePrefab != null)
                {
                    GameObject particle = Instantiate(
                        spawnParticlePrefab,
                        newCube.transform.position,
                        Quaternion.identity,
                        newCube.transform
                    );
                    Destroy(particle, 2f);
                }

                if (PopapScore.Instance != null)
                    PopapScore.Instance.ShowPopup(newCube.CubeNumber, newCube.transform.position);

                if (HeightScore.Instance != null)
                    HeightScore.Instance.AddScore(newCube.CubeNumber);

                AwardExpFromMerge(cube.CubeNumber);

                float pushForce = 2.5f;
                newCube.CubeRigidbody.AddForce(new Vector3(0, 0.3f, 1f) * pushForce, ForceMode.Impulse);

                Vector3 randomTorque = Random.insideUnitSphere * 1.5f;
                newCube.CubeRigidbody.AddTorque(randomTorque, ForceMode.Impulse);
            }

            if (popSound != null)
                popSound.Play();

            CubeSpawner.Instance.DestroyCube(cube);
            CubeSpawner.Instance.DestroyCube(otherCube);
        }

        PersistentAudioManager.Instance.PlayCubeCollisionSound();
    }

    private void ResetMergeFlag()
    {
        hasMerged = false;
    }

    /// <summary>
    /// Mirror the 2D EXP gain: award 2 * log2(value) EXP for each successful merge.
    /// </summary>
    /// <param name="baseNumber">Value of the cubes before they were merged.</param>
    private void AwardExpFromMerge(int baseNumber)
    {
        if (baseNumber <= 0)
            return;

        int expValue = Mathf.Max(1, Mathf.RoundToInt(Mathf.Log(baseNumber, 2f) * 2f));
        // GM.GetInstance().AddExp(expValue); // Deprecated: XP is based on High Score now
    }
}

using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float yRotationSpeed = 50f;

    private void Update()
    {
        if (!gameObject.activeInHierarchy) return;

        transform.Rotate(0f, yRotationSpeed * Time.deltaTime, 0f, Space.Self);
    }
}
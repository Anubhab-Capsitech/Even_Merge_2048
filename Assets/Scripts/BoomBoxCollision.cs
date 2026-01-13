using System.Collections;
using UnityEngine;

public class BoomBoxCollision : MonoBehaviour
{
    public GameObject particle;

    private void OnCollisionEnter(Collision collision)
    {
        Cube hitCube = collision.gameObject.GetComponent<Cube>();

        if (hitCube != null)
        {
            Vector3 contactPoint = collision.contacts[0].point;
            int cubeNumber = hitCube.CubeNumber;

            if (PopapScore.Instance != null)
            {
                PopapScore.Instance.ShowPopup(cubeNumber, contactPoint);
            }
            else
            {
                Debug.LogWarning("PopapScore.Instance is null ");
            }

            if (HeightScore.Instance != null)
            {
                HeightScore.Instance.AddScore(cubeNumber);
            }
            else
            {
                Debug.LogWarning("HeightScore.Instance is null");
            }

            Player player = FindObjectOfType<Player>();
            if (player != null)
            {
                player.NotifyCubeDestroyedByColorBox();
            }

            CubeSpawner.Instance.DestroyCube(hitCube);
            Destroy(gameObject);

            if (particle != null)
            {
                Instantiate(particle, contactPoint, Quaternion.identity);
            }
            else
            {
                Debug.LogWarning("Particle prefab not assigned in BoomBoxCollision ");
            }
        }

       
        CheckCustomModelCollision(collision.gameObject);
    }

    private void CheckCustomModelCollision(GameObject collidedObject)
    {
       
        Player player = FindObjectOfType<Player>();
        if (player == null) return;

  
        if (collidedObject.CompareTag("CustomModel") || collidedObject.name.Contains("CustomModel"))
        {
            if (player != null)
                player.NotifyCubeDestroyedByColorBox();

            Destroy(collidedObject);
            Destroy(gameObject);

            if (particle != null)
            {
                Instantiate(particle, collidedObject.transform.position, Quaternion.identity);
            }
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBoxControler : MonoBehaviour
{
    public GameObject particleEffect;

    private void OnCollisionEnter(Collision collision)
    {
        Cube hitCube = collision.gameObject.GetComponent<Cube>();

        if (hitCube != null)
        {
            Vector3 contactPoint = collision.contacts[0].point;
            int doubledNumber = hitCube.CubeNumber * 2;

            if (doubledNumber <= CubeSpawner.Instance.maxCubeNumber)
            {
                Cube newCube = CubeSpawner.Instance.Spawn(doubledNumber, contactPoint + Vector3.up * 1.6f);

                if (particleEffect != null)
                {
                    GameObject particle = Instantiate(particleEffect, newCube.transform.position, Quaternion.identity);
                    particle.transform.SetParent(newCube.transform);
                }

                PopapScore.Instance.ShowPopup(newCube.CubeNumber, newCube.transform.position);
                HeightScore.Instance.AddScore(newCube.CubeNumber);

                float pushForce = 2.5f;
                newCube.CubeRigidbody.AddForce(new Vector3(0, 0.3f, 1f) * pushForce, ForceMode.Impulse);
                float randomValue = Random.Range(-20f, 20f);
                Vector3 randomDirection = Vector3.one * randomValue;
                newCube.CubeRigidbody.AddTorque(randomDirection);

                GameObject.Find("ColorBoxsound").GetComponent<AudioSource>().Play();

                Player player = FindObjectOfType<Player>();
                if (player != null)
                    player.NotifyCubeDestroyedByColorBox();

                CubeSpawner.Instance.DestroyCube(hitCube);
                Destroy(gameObject);
            }
            else
            {
          
                Player player = FindObjectOfType<Player>();
                if (player != null)
                    player.NotifyCubeDestroyedByColorBox();

                CubeSpawner.Instance.DestroyCube(hitCube);
                Destroy(gameObject);
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
        }
    }
}
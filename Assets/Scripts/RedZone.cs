using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RedZone : MonoBehaviour
{

    private void OnTriggerStay(Collider other)
    {

        Cube cube = other.GetComponent<Cube>();
        if (cube != null)
        {
            if (cube.HasBeenLaunched && cube.CubeRigidbody.linearVelocity.magnitude < 0.1f)
            {

                HeightScore.Instance.GameOver();
                int currentScore = HeightScore.Instance.GetCurrentScore();
                int highScore = HeightScore.highScore;
                Debug.Log($"Collision! Current Score: {currentScore}, High Score: {highScore}");

                // 3D mode design: always show the happy panel, we only care about current score.
                UiManager.Instance.GameOverHappyPanel();
                Debug.Log("Game Over (3D) – showing Happy Panel only.");
            }
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        Cube cube = other.GetComponent<Cube>();
        if (cube != null)
        {
            if (cube.HasBeenLaunched && cube.CubeRigidbody.linearVelocity.magnitude < 0.1f)
            {
                HeightScore.Instance.GameOver();
                int currentScore = HeightScore.Instance.GetCurrentScore();
                int highScore = HeightScore.highScore;

                Debug.Log($"Collision! Current Score: {currentScore}, High Score: {highScore}");

                // 3D mode design: always show the happy panel, we only care about current score.
                UiManager.Instance.GameOverHappyPanel();
                Debug.Log("Game Over (3D) – showing Happy Panel only.");
            }
        }
    }




}

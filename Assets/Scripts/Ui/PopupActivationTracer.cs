using UnityEngine;

public class PopupActivationTracer : MonoBehaviour
{
    void OnEnable()
    {
        Debug.Log(
            $"[POPUP ENABLED] {gameObject.name}\n{System.Environment.StackTrace}"
        );
    }
}

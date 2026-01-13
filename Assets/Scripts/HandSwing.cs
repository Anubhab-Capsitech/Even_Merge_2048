using UnityEngine;

public class HandSwing : MonoBehaviour
{
    [Header("Swing Settings")]
    public float speed = 2f;          
    public float distance = 1f;       
    public bool useLocalPosition = true;  

    private Vector3 startPos;

    void Start()
    {
        startPos = useLocalPosition ? transform.localPosition : transform.position;
    }

    void Update()
    {
        float xOffset = Mathf.Sin(Time.time * speed) * distance;
        Vector3 newPos = startPos + new Vector3(xOffset, 0, 0);

        if (useLocalPosition)
        {
            transform.localPosition = newPos;
        }
        else
        {
            transform.position = newPos;
        }
    }
}

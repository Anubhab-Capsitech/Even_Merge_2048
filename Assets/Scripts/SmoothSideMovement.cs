using UnityEngine;

public class SmoothSideMovement : MonoBehaviour
{
   // public bool enableMovement = false;

    [Header("Floating Range")]
    public float minY = 10.6f;   
    public float midY = 15f;      
    public float maxY = 21f;      

    [Header("Motion Settings")]
    public float verticalSpeed = 0.3f;   
    public float driftSpeed = 0.25f;     
    public float driftAmount = 0.15f;     
    public float smoothTime = 1.5f;      

    private Vector3 startPos;
    private float randomOffset;
    private float targetY;
    private float velocityY;

    void Start()
    {
        startPos = transform.localPosition;
        randomOffset = Random.Range(0f, 100f);
        targetY = midY;
    }

    void Update()
    {
      //  if (!enableMovement) return;    

   
        float time = Time.time + randomOffset;

      
        float phase = (Mathf.Sin(time * verticalSpeed) + 1f) * 0.5f;

 
        float curve = Mathf.SmoothStep(0f, 1f, Mathf.Pow(phase, 1.2f));

 
        float desiredY = Mathf.Lerp(minY, maxY, curve);

 
        float newY = Mathf.SmoothDamp(transform.localPosition.y, desiredY, ref velocityY, smoothTime);
 
        float newX = startPos.x + Mathf.Sin(time * driftSpeed) * driftAmount;

        transform.localPosition = new Vector3(newX, newY, startPos.z);
    }
}


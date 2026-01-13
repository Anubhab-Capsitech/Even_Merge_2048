using UnityEngine;
using DG.Tweening;

public class CloudAnim : MonoBehaviour
{
    [Header("Cloud Drift Settings")]
    public float driftDistance = 25f;    // How far the cloud moves
    public float driftDuration = 25f;    // Slow natural movement
    public LoopType loopType = LoopType.Yoyo; // Choose between Yoyo and Restart
    public Ease driftEase = Ease.InOutSine;   // Movement feel

    private Vector3 startPosition;
    private Tween driftTween;

    void Awake()
    {
        startPosition = transform.position;
    }

    /// <summary>
    /// Begin slow cloud drifting.
    /// </summary>
    public void StartMovement()
    {
         if (driftTween != null && driftTween.IsActive()) return;

        float targetX = startPosition.x + driftDistance;

        driftTween = transform.DOMoveX(targetX, driftDuration)
            .SetEase(driftEase)
            .SetLoops(-1, loopType)
            .SetAutoKill(false);
    }

    /// <summary>
    /// Stop drifting and reset cloud.
    /// </summary>
    public void StopMovement()
    {
        if (driftTween != null)
        {
            driftTween.Kill();
            driftTween = null;
        }

        // optional reset
        transform.position = startPosition;
    }
}

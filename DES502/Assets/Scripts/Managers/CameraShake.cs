using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeSpeed = 10f;

    private Vector3 _originalPosition;

    void Awake()
    {
        _originalPosition = transform.position;
    }

    public void BeginShake(float intensity, float duration)
    {
        StartCoroutine(Shake(intensity, duration));
    }
    
    private IEnumerator Shake(float intensity, float duration)
    {
        float timer = 0;
        // get initial target
        Vector3 startPosition = _originalPosition;
        Vector3 shakeTarget = GetShakeTarget(_originalPosition, intensity);
        float percentTravelled = 0;
        // Keep moving to new targets while the timer hasn't expired
        while (timer < duration)
        {
            timer += Time.deltaTime;
            //Debug.Log("Shake timer: " + timer.ToString());
            if (percentTravelled >= 1)
            {
                startPosition = transform.position;
                shakeTarget = GetShakeTarget(_originalPosition, intensity);
                percentTravelled = 0;
            }
            // don't bother clamping, we don't care if it goes over a bit
            //percentTravelled += Time.deltaTime;
            percentTravelled = IncrementPercentTravelled(percentTravelled);
            // move to target
            MoveToShakeTarget(startPosition, shakeTarget, percentTravelled);
            yield return null;
        }
        // travel back to the original position
        startPosition = transform.position;
        percentTravelled = 0;
        while (percentTravelled != 1)
        {
            // increment timer while clamping to 1
            percentTravelled = IncrementPercentTravelled(percentTravelled);
            MoveToShakeTarget(startPosition, _originalPosition, percentTravelled);
            yield return null;
        }
    }

    static private Vector3 GetShakeTarget(Vector3 _originalPosition, float intensity)
    {
        Vector3 newTarget = _originalPosition + Random.insideUnitSphere * intensity;
        Debug.Log("New shake target: " + newTarget.ToString());
        return newTarget;
    }

    private void MoveToShakeTarget(Vector3 startPosition, Vector3 targetPosition, float travelledPercent)
    {
        transform.position = Vector3.Lerp(startPosition, targetPosition, travelledPercent);
    }

    // TODO: make this a void return that sets a member var?
    private float IncrementPercentTravelled(float currentValue)
    {
        float newValue = Mathf.Min(currentValue + (Time.deltaTime * shakeSpeed), 1);
        //Debug.Log("PercentTravelled: " + newValue);
        return newValue;
    }
}

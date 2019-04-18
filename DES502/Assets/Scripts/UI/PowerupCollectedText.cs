﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerupCollectedText : MonoBehaviour
{
    [SerializeField] [Range(0, 5)]
    [Tooltip("How long (in seconds) the powerup stays active for before being destroyed.")]
    private float _lifeDuration = 2f;
    [SerializeField] [Range(0, -2)]
    [Tooltip("How far (in grid units) the text travels before being destroyed.")]
    private float _floatOffset = 0.5f;
    [SerializeField] [Range(0, 3)]
    [Tooltip("How fast should the powerup text travel?")]
    private float _moveSpeed = 1;

    private bool _targetPositionReached = false;

    // functions as a custom constructor??
    public void Activate(string powerupName, Vector3 powerupPosition)
    {
        SetText(powerupName);
        StartCoroutine(FloatUpwards(powerupPosition));
        Invoke("QueueDeath", _lifeDuration);
    }

    private void SetText(string newText)
    {
        Text textComponent = GetComponent<Text>();
        textComponent.text = newText;
    }

    private IEnumerator FloatUpwards(Vector3 powerupPosition)
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = InGameCanvas.GetScreenPositionWithOffset(powerupPosition, _floatOffset);
        float percentTravelled = 0;
        while (percentTravelled < 1)
        {
            // why does this make it go super fast when multiplying by one????
            //percentTravelled = Mathf.Min(percentTravelled + Time.deltaTime * _floatSpeed, 1);
            percentTravelled = Mathf.Min(percentTravelled + Time.deltaTime * _moveSpeed, 1);
            //percentTravelled = Mathf.Min(percentTravelled + Time.deltaTime, 1);
            //Debug.Log("powerup text percent travelled: " + percentTravelled.ToString());
            transform.position = Vector3.Lerp(startPosition, targetPosition, percentTravelled);
            yield return null;
        }
        _targetPositionReached = true;
    }

    private void QueueDeath()
    {
        StartCoroutine(DieWhenTargetPositionReached());
    }

    private IEnumerator DieWhenTargetPositionReached()
    {
        while (!_targetPositionReached)
        {
            yield return null;
        }
        Destroy(gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameCanvas : MonoBehaviour
{
    [SerializeField] [Tooltip("Powerup text prefab to spawn")]
    private GameObject _powerupTextPrefab;
    [SerializeField] [Range(0, 3)]
    [Tooltip("Vertical offset (in grid units) to add to powerup text position when spawning.")]
    private float _powerupTextPositionOffset = 1;

    private Camera _camera;

    void Awake()
    {
        _camera = Camera.main;
    }

    public void SpawnPowerupText(string powerupName, Vector3 worldPosition)
    {
        //Debug.Log("SPAWN POWERUP TEXT");
        Vector3 textPosition = GetScreenPositionWithOffset(worldPosition, _powerupTextPositionOffset);
        //Debug.Log("Powerup text screen position: " + textPosition.ToString());
        GameObject spawnedPowerupTextObject = Instantiate(_powerupTextPrefab, textPosition, Quaternion.identity, transform);
        spawnedPowerupTextObject.GetComponent<PowerupCollectedText>().Activate(powerupName, worldPosition);
    }

    static public Vector3 GetScreenPositionWithOffset(Vector3 worldPosition, float verticalOffset)
    {
        Vector3 offset = new Vector3(0, verticalOffset, 0);
        return Camera.main.WorldToScreenPoint(worldPosition - offset);
    }
}

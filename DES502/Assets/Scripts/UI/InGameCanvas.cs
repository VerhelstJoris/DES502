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
        Vector3 textPosition = GetScreenPosition(worldPosition);
        //Debug.Log("Powerup text screen position: " + textPosition.ToString());
        GameObject spawnedPowerupTextObject = Instantiate(_powerupTextPrefab, textPosition, Quaternion.identity, transform);
        spawnedPowerupTextObject.GetComponent<PowerupCollectedText>().Activate(powerupName, worldPosition);
    }

    private Vector3 GetScreenPosition(Vector3 worldPosition)
    {
        Vector3 offset = new Vector3(0, _powerupTextPositionOffset, 0);
        return _camera.WorldToScreenPoint(worldPosition - offset);
    }
}

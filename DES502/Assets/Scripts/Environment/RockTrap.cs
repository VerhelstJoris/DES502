using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockTrap : Trap
{
    [Header("Rock Trap Specific")]
    [SerializeField] [Tooltip("Reference to the rock prefab.")]
    private GameObject _rockPrefab;
    [SerializeField] [Tooltip("Reference to the spawn location child object.")]
    private GameObject _rockSpawnLocation;
    private Vector3 _rockSpawnPosition;

    public override void Awake()
    {
        _spriteRenderer = _rockSpawnLocation.GetComponent<SpriteRenderer>();
        SetCooldownColors(true);
        _rockSpawnPosition = _rockSpawnLocation.transform.position;
    }

    public override void Trigger(CharacterController player)
    {
        Debug.Log("SPAWN ROCK");
        GameObject newRock = Instantiate(_rockPrefab, _rockSpawnPosition, Quaternion.identity);
        BeginCooldownTimer();
    }
}

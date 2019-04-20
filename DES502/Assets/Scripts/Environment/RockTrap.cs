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

    public override void Awake()
    {
        _spriteRenderer = _rockSpawnLocation.GetComponent<SpriteRenderer>();
    }

    public override void Trigger(CharacterController player)
    {
        Debug.Log("SPAWN ROCK");
        BeginCooldownTimer();
    }
}

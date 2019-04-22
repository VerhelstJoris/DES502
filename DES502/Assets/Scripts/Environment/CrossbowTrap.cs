using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowTrap : Trap
{
    [Header("Crossbow Trap-specific")]
    [SerializeField] [Tooltip("Reference to crossbow trap bolt prefab to instantiate.")]
    private GameObject _boltPrefab;
    [SerializeField] [Tooltip("Reference to bolt spawn location child (animation is also played here).")]
    private GameObject _boltSpawnLocation;

    public override void Awake()
    {
        _spriteRenderer = _boltSpawnLocation.GetComponent<SpriteRenderer>();
        _animator = _boltSpawnLocation.GetComponent<Animator>();
        SetCooldownColors();
    }

    public override void Trigger(CharacterController playerOverlapping)
    {
        Debug.Log("SPAWN BOLT");
        BeginCooldownTimer();
    }
}

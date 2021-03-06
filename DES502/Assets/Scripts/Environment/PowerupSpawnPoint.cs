﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawnPoint : MonoBehaviour
{
    [Header("General")]
    [Range(0f, 1f)] [Tooltip("Transparency percent to use for gizmo sprite, in 0-1 space.")]
    public float _spriteTransparency = 0.7f;

    /*
    [Header("Powerups to Spawn")]
    public bool _spawnReverseControls = false;
    public bool _spawnMoveSpeed = true;
    public bool _spawnRoot = true;
    public bool _spawnShield = true;
    public bool _spawnMeleeInstantKill = true;
    public bool _spawnRandom = true;
    */

    [HideInInspector]
    public bool _containsPowerup = false;
    private List<CharacterController> _playersOverlapping = new List<CharacterController>();
    /*
    [HideInInspector]
    public bool[] _powerupsToSpawn;
    */

    // Disabling SpriteRenderer method
    void OnValidate()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1f, 1f, 1f, _spriteTransparency);
    }

    void Awake()
    {
        // this needs to be gotten again for the build, as OnValidate() only triggers in-editor
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.enabled = false;
    }

    public void OnChildPowerupCollected()
    {
        _containsPowerup = false;
    }

    /*
    public bool[] GetPowerupsToSpawn()
    {
        bool[] powerups = new bool[]
        {
            _spawnReverseControls,
            _spawnMoveSpeed,
            _spawnRoot,
            _spawnShield,
            _spawnMeleeInstantKill,
            _spawnRandom,
        };
        return powerups;
    }
    */

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _playersOverlapping.Add(other.GetComponent<CharacterController>());
            //Debug.Log(_playersOverlapping.Count);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            _playersOverlapping.Remove(other.GetComponent<CharacterController>());
            //Debug.Log(_playersOverlapping.Count);
        }
    }

    public bool IsValid()
    {
        return (_playersOverlapping.Count == 0 && !_containsPowerup);
    }
}

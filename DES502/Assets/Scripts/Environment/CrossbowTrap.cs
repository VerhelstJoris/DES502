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
    [SerializeField] [Tooltip("Should this instance be facing right or left?")]
    private bool _facingRight = false;

    private void OnValidate()
    {
        _spriteRenderer = _boltSpawnLocation.GetComponent<SpriteRenderer>();
        UpdateSpriteDirection();
    }

    public override void Awake()
    {
        _spriteRenderer = _boltSpawnLocation.GetComponent<SpriteRenderer>();
        _animator = _boltSpawnLocation.GetComponent<Animator>();
        SetCooldownColors();
    }

    private void Start()
    {
        UpdateSpriteDirection();
    }

    public override void Trigger(CharacterController playerOverlapping)
    {
        SpawnBolt();
        BeginCooldownTimer();
    }

    private void SpawnBolt()
    {
        //Debug.Log("SPAWN BOLT");
        Vector3 boltSpawnPosition = _boltSpawnLocation.transform.position;
        Quaternion boltSpawnRotation = _boltSpawnLocation.transform.rotation;
        GameObject newBoltObject = Instantiate(_boltPrefab, boltSpawnPosition, boltSpawnRotation);
        //CrossbowBolt newBolt = newBoltObject.GetComponent<CrossbowBolt>();
        //newBolt.Activate(_facingRight);
        newBoltObject.GetComponent<CrossbowBolt>().Activate(_facingRight);
    }

    private void UpdateSpriteDirection()
    {
        _spriteRenderer.flipX = _facingRight;
    }
}

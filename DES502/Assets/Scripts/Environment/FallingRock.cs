using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : Trap
{
    [Header("Falling Rock-specific")]
    [SerializeField]  // TODO: add suitable range slider here!!
    [Tooltip("The minimum speed (velocity magnitude squared) for the rock to be travelling at in order to kill players.")]
    private float _minKillSpeed = 10;

    private Rigidbody2D _rigidbody;

    public override void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        SetCooldownColors(true);
    }

    public override void Trigger(CharacterController playerAffecting)
    {
        Debug.Log("Current rock speed: " + GetCurrentSpeed().ToString());
        if (GetCurrentSpeed() >= _minKillSpeed)
        {
            playerAffecting.Die(CauseOfDeath.Rock);
        }
    }

    private float GetCurrentSpeed()
    {
        return _rigidbody.velocity.sqrMagnitude;
    }
}

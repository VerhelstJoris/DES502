using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingRock : Trap
{
    [Header("Falling Rock-specific")]
    [SerializeField]  // TODO: add suitable range slider here!!
    [Tooltip("The minimum speed (velocity magnitude squared) for the rock to be travelling at in order to kill players.")]
    private float _minKillSpeed = 10;
    [SerializeField]  // TODO: add suitable range slider here!!
    [Tooltip("The minimum speed (velocity magnitude squared) for the rock to be travelling at in order to not be queued for deletion.")]
    private float _minSpeed = 3;
    [SerializeField] [Range(0, 1)]
    [Tooltip("How often should check to see the rock is moving slow enough to begin to expire?")]
    private float _cooldownStartCheckWaitDuration = 1;

    private Rigidbody2D _rigidbody;

    public override void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
        SetCooldownColors(true, true);
    }

    private void Start()
    {
        StartCoroutine(CheckIfCooldownShouldBegin());
    }

    public override void Trigger(CharacterController playerAffecting)
    {
        CheckIfKillsPlayer(playerAffecting);
    }

    private void OnCollisionEnter2D(Collider2D col)
    {
        CharacterController playerAffecting = col.gameObject.GetComponent<CharacterController>();
        CheckIfKillsPlayer(playerAffecting);
    }

    private void CheckIfKillsPlayer(CharacterController playerAffecting)
    {
        //Debug.Log("Current rock speed: " + GetCurrentSpeed().ToString());
        if (GetCurrentSpeed() >= _minKillSpeed)
        {
            playerAffecting.Die(CauseOfDeath.Rock);
        }
    }

    private float GetCurrentSpeed()
    {
        return _rigidbody.velocity.sqrMagnitude;
    }

    private IEnumerator CheckIfCooldownShouldBegin()
    {
        WaitForSeconds delay = new WaitForSeconds(_cooldownStartCheckWaitDuration);
        while (!_onCooldown)
        {
            if (GetCurrentSpeed() < _minSpeed)
            {
                BeginCooldownTimer();
            }
            yield return delay;
        }
    }

    public override void OnCooldownTimerEnded()
    {
        Destroy(gameObject);
    }

    public void AddKnockback(Vector3 forcePosition, Vector2 direction, float magnitude)
    {
        _rigidbody.AddForceAtPosition(direction * magnitude, forcePosition);
        StopCooldownTimer();
        StartCoroutine(CheckIfCooldownShouldBegin());
    }
}

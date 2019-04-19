using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringTrap : Trap
{
    [Header("Spring Trap Specific")]
    [SerializeField] [Tooltip("What direction is the player knocked back when triggered.")]
    private Vector2 _knockbackDirection = new Vector2(0, 1);
    [SerializeField] [Range(0, 10000)]
    [Tooltip("How much knockback force is applied to the player when triggered.")]
    private float _knockbackForce = 1000;

    public override void Trigger(CharacterController playerAffecting)
    {
        // apply knockback
        playerAffecting.AddKnockback(_knockbackDirection * _knockbackForce);
        //playerAffecting.RecieveHit(_knockbackDirection * _knockbackForce, 0);
        Debug.Log("TRIGGERING");
        //BeginCooldownTimer();
    }
}

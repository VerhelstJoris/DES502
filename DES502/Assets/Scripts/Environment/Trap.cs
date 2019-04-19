using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trap : MonoBehaviour
{
    [Header("Base Trap Class")]
    // when would we use this???
    [SerializeField] [Tooltip("If the trap has a cooldown period or if it is always active.")]
    private bool _constant = false;
    [SerializeField] [Range(0, 10)] 
    [Tooltip("How long does the cooldown between triggers last for.")]
    private float _cooldownTimer = 4;

    private bool _onCooldown = false;

    public abstract void Trigger(CharacterController playerAffecting);
    // legacy virtual methods, ignore
    public virtual void Reset(){}
    public virtual void Activate(){}
    public virtual void Deactivate(){}

    public void BeginCooldownTimer()
    {
        _onCooldown = true;
        Invoke("OnCooldownTimerEnded", _cooldownTimer);
    }

    private void OnCooldownTimerEnded()
    {
        _onCooldown = false;
    }

    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (!_onCooldown)
        {
            if (col.tag == "Player")
            {
                //Debug.Log("OBJECT ENTERED");
                Trigger(col.GetComponent<CharacterController>());
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trap : MonoBehaviour
{
    [Header("Base Trap Class")]
    [SerializeField] [Tooltip("If the trap has a cooldown period or if it is always active.")]
    private bool _constant = false;
    [SerializeField] [Range(0, 10)] 
    [Tooltip("How long does the cooldown between triggers last for.")]
    private float _cooldownTimer = 4;

    private bool _onCooldown = false;

    private void BeginCooldownTimer()
    {
        _onCooldown = true;
        Invoke("OnCooldownTimerEnded", _cooldownTimer);
    }

    private void OnCooldownTimerEnded()
    {
        _onCooldown = false;
    }

    //just for inheritance sake
    public abstract void Reset();
    public abstract void Activate();
    public abstract void Deactivate();
}

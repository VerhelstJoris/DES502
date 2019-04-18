using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trap : MonoBehaviour
{
    [Header("Base Trap Class")]
    [SerializeField] [Tooltip("If the trap has a cooldown period or if it is always active.")]
    private bool _constant = false;

    //just for inheritance sake
    public abstract void Reset();
    public abstract void Activate();
    public abstract void Deactivate();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trap : MonoBehaviour
{
    [Header("Base Trap Class")]
    [SerializeField] [Tooltip("Reference to the global trap data scriptable object.")]
    private GlobalTrapData _globalTrapData;
    // when would we use this???
    [SerializeField] [Tooltip("If the trap has a cooldown period or if it is always active.")]
    private bool _constant = false;
    [SerializeField] [Range(0, 10)] 
    [Tooltip("How long does the cooldown between triggers last for.")]
    private float _cooldownTimer = 4;

    private static Color DEFAULT_SPRITE_COLOR = new Color(1, 1, 1, 1);

    private bool _onCooldown = false;
    private SpriteRenderer _spriteRenderer;

    public abstract void Trigger(CharacterController playerAffecting);
    // legacy virtual methods, ignore
    public virtual void Reset(){}
    public virtual void Activate(){}
    public virtual void Deactivate(){}

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void BeginCooldownTimer()
    {
        // Call this at the end of children's Trigger methods
        _onCooldown = true;
        SetSpriteColor(_globalTrapData._cooldownSpriteColor);
        Invoke("OnCooldownTimerEnded", _cooldownTimer);
    }

    private void OnCooldownTimerEnded()
    {
        _onCooldown = false;
        SetSpriteColor(DEFAULT_SPRITE_COLOR);
    }

    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (_constant || !_onCooldown)
        {
            if (col.tag == "Player")
            {
                //Debug.Log("OBJECT ENTERED");
                Trigger(col.GetComponent<CharacterController>());
            }
        }
    }

    private void SetSpriteColor(Color newColor)
    {
        _spriteRenderer.color = newColor;
    }
}

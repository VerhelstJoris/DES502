using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Trap : MonoBehaviour
{
    [Header("Base Trap Class")]
    [SerializeField] [Tooltip("Reference to the global trap data scriptable object.")]
    private GlobalTrapData _globalTrapData;
    [SerializeField] [Tooltip("If the trap has a cooldown period or if it is always active.")]
    private bool _constant = false;
    [SerializeField] [Range(0, 10)] 
    [Tooltip("How long does the cooldown between triggers last for.")]
    private float _cooldownTimer = 4;

    [HideInInspector]
    public Animator _animator;
    [HideInInspector]
    public SpriteRenderer _spriteRenderer;

    private static Color DEFAULT_SPRITE_COLOR = new Color(1, 1, 1, 1);
    private static Color TRANSPARENT_SPRITE_COLOR = new Color(0, 0, 0, 0);

    private bool _onCooldown = false;
    private List<CharacterController> playersOverlapping = new List<CharacterController>();
    private bool _queueActive = false;
    private bool _isCooldownBlinking = false;
    private Color[] _cooldownColors = new Color[2];
    private bool _blink = true;

    public abstract void Trigger(CharacterController playerAffecting);
    // legacy virtual methods, ignore
    public virtual void Reset(){}
    public virtual void Activate(){}
    public virtual void Deactivate(){}

    public virtual void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        SetCooldownColors();
    }

    public void BeginCooldownTimer()
    {
        // Call this at the end of children's Trigger methods
        _onCooldown = true;
        SetSpriteColor(_onCooldown);
        Invoke("OnCooldownTimerEnded", _cooldownTimer);
        float cooldownBlinkingTimer = _cooldownTimer - _globalTrapData._colorBlinkDuration;
        //Invoke("BeginCooldownBlinking", cooldownBlinkingTimer);
        InvokeRepeating("CooldownBlink", cooldownBlinkingTimer, _globalTrapData._colorBlinkWaitDuration);
    }

    private void OnCooldownTimerEnded()
    {
        _onCooldown = false;
        CancelInvoke("CooldownBlink");
        SetSpriteColor(_onCooldown);
    }

    public virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            //Debug.Log("OBJECT ENTERED");
            CharacterController playerOverlapping = col.GetComponent<CharacterController>();
            if (_constant)
            {
                //Debug.Log("BYPASSING TRIGGER QUEUE");
                Trigger(playerOverlapping);
            }
            else
            {
                playersOverlapping.Add(playerOverlapping);
                if (!_queueActive)
                {
                    StartCoroutine(QueueTrigger());
                }
            }
        }
    }

    public virtual void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player" && !_constant)
        {
            //Debug.Log("OBJECT EXITED");
            CharacterController playerExited = col.GetComponent<CharacterController>();
            playersOverlapping.Remove(playerExited);
        }
    }

    private void SetSpriteColor(bool isOnCooldown)
    {
        bool isActive = !isOnCooldown;
        _spriteRenderer.color = _cooldownColors[isActive.GetHashCode()];
    }

    private IEnumerator QueueTrigger()
    {
        _queueActive = true;
        WaitForSeconds delay = new WaitForSeconds(_globalTrapData._overlapQueueWaitDuration);
        while (playersOverlapping.Count > 0)
        {
            if (!_onCooldown)
            {
                foreach (CharacterController p in playersOverlapping)
                {
                    Trigger(p);
                    _queueActive = false;
                }
            }
            yield return delay;
        }
        _queueActive = false;
    }

    // TODO: legacy - depricated
    /*
    private void BeginCooldownBlinking()
    {
        //Debug.Log("BEGIN COOLDOWN BLINKING");
        if (!_isCooldownBlinking)
        {
            StartCoroutine(CooldownBlinking());
        }
    }
    */

    // TODO: legacy - depricated
    /*
    private IEnumerator CooldownBlinking()
    {
        _isCooldownBlinking = true;
        WaitForSeconds delay = new WaitForSeconds(_globalTrapData._colorBlinkWaitDuration);
        Color[] blinkColors = new Color[2];
        blinkColors[0] = _globalTrapData._cooldownSpriteColor;
        blinkColors[1] = DEFAULT_SPRITE_COLOR;
        bool blink = false;
        while (_onCooldown)
        {
            blink = !blink;
            SetSpriteColor(blinkColors[blink.GetHashCode()]);
            yield return delay;
        }
        _isCooldownBlinking = false;
    }
    */

    private void CooldownBlink()
    {
        if (_onCooldown)
        {
            _blink = !_blink;
            SetSpriteColor(_blink);
        }
    }

    public void SetCooldownColors(bool isBlinkOffTransparent = false)
    {
        //_cooldownColors = new Color[2];
        if (isBlinkOffTransparent)
        {
            _cooldownColors[0] = TRANSPARENT_SPRITE_COLOR;
        }
        else
        {
            _cooldownColors[0] = _globalTrapData._cooldownSpriteColor;
        }
        _cooldownColors[1] = DEFAULT_SPRITE_COLOR;
    }
}

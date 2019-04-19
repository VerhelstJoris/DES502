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

    private static Color DEFAULT_SPRITE_COLOR = new Color(1, 1, 1, 1);

    private bool _onCooldown = false;
    private SpriteRenderer _spriteRenderer;
    private List<CharacterController> playersOverlapping = new List<CharacterController>();
    private bool _queueActive = false;

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
        if (col.tag == "Player")
        {
            //Debug.Log("OBJECT ENTERED");
            CharacterController playerOverlapping = col.GetComponent<CharacterController>();
            if (_constant)
            {
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

    private void SetSpriteColor(Color newColor)
    {
        _spriteRenderer.color = newColor;
    }

    private IEnumerator QueueTrigger()
    {
        _queueActive = true;
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
            // TODO: does this need to be checked every tick? could just change to a really small value
            yield return null;
        }
        _queueActive = false;
    }
}

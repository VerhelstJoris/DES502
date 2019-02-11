using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{

    [SerializeField]
    private Trap _trap;

    [SerializeField][Range(0, 10.0f)]
    private float _activeDuration;

    [SerializeField][Range(0, 10.0f)]
    private float _cooldownDuration;

    [SerializeField]
    private Sprite _deactivatedSprite;

    [SerializeField]
    private Sprite _activatedSprite;

    private bool _onCooldown;
    private bool _active;
    private float _cooldownTimer;
    private float _activeTimer;

    private SpriteRenderer _renderer;


    // Start is called before the first frame update
    void Start()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        _trap.Reset();
    }

    void FixedUpdate()
    {
        //cooldown timer
        if (_onCooldown)
        {
            _cooldownTimer += Time.deltaTime;

            if(_cooldownTimer >= _cooldownDuration)
            {
                _onCooldown = false;
                Debug.Log("Cooldown expired");
            }
        }

        //active timer
        if (_active)
        {
            _activeTimer += Time.deltaTime;

            if(_activeTimer >= _activeDuration)
            {
                _active = false;
                _onCooldown = true;
                Deactivate();
            }
        }  
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if(col.tag == "Weapon")
        {
            if(!_active && !_onCooldown)
            {
                Activate();
            }
        }
    }

    void Activate()
    {
        _active = true;
        _activeTimer = 0.0f;
        _cooldownTimer = 0.0f;

        _renderer.sprite = _activatedSprite;
        _trap.Activate();
    }

    void Deactivate()
    {
        _trap.Deactivate();
        _renderer.sprite = _deactivatedSprite;
    }

}

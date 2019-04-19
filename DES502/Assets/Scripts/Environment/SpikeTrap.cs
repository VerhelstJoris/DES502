using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : Trap
{
    private SpriteRenderer _renderer;

    [SerializeField][Tooltip ("time it takes for it to pop-up after the inital lever has been activated. leaves room for animation later down the line") ]
    private float _activationDuration;

    [SerializeField]
    [Tooltip("time it takes for it to go down after the inital lever resets. leaves room for animation later down the line")]
    private float _deactivationDuration;

    [SerializeField]
    private bool _activeWhileActivating, _activeWhileDeactivating;

    [Header("Camera Shake")]
    [SerializeField] [Range(0, 2)]
    [Tooltip("Max distance for camera shake from origin.")]
    private float _cameraShakeIntensity = 0.5f;
    [SerializeField] [Range(0, 2)]
    [Tooltip("Max distance for camera shake from origin.")]
    private float _cameraShakeDuration = 0.25f;

    private bool _active=true;
    private bool _activating = false;
    private bool _deactivating = false;
    private float _timer = 0.0f;
    private Vector3 _startPos,_endPos;
    private CameraShake _cameraShake;

    void Awake()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        _startPos = this.transform.position;
        _endPos = transform.position +( transform.up * -0.5f);
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }

    void Update()
    {
        //timers
        if(_activating)
        {
            _timer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, _startPos, _timer / _activationDuration);
            //transform.position += transform.up * 0.5f * _activationDuration;
            if (_timer >= _activationDuration)
            {
                _timer = 0.0f;
                _active = true;
                _activating = false;

            }
        }

        if(_deactivating)
        {
            _timer += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, _endPos, _timer / _deactivationDuration);
            //transform.position += transform.up * -0.5f * _deactivationDuration;
            if (_timer >= _deactivationDuration)
            {
                _timer = 0.0f;
                _active = false;
                _deactivating = false;
            }
        }
    }

    public override void OnTriggerEnter2D(Collider2D col)
    {
        if (_active)
        {
            if (col.tag == "Player")
            {
                Trigger(col.GetComponent<CharacterController>());
            }
        }
    }

    public override void Trigger(CharacterController playerAffecting)
    {
        playerAffecting.Die(CauseOfDeath.Spikes);
        _cameraShake.BeginShake(_cameraShakeIntensity, _cameraShakeDuration);
    }

    public void Activate()
    {
        if (_activeWhileActivating)
        {
            _active = true;
        }
        _activating = true;
        _renderer.enabled = true;
        //Debug.Log("Spike Activating");
    }

    public void Deactivate()
    {
        if(!_activeWhileDeactivating)
        {
            _active = false;
        }
        _deactivating = true;
        //Debug.Log("Spike Deactivating");
    }

    public void Reset()
    {
        transform.position = _endPos;
        _active = false;
    }
}

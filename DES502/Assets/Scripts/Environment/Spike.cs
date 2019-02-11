using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : Trap
{

    private SpriteRenderer _renderer;

    [SerializeField][Tooltip ("time it takes for it to pop-up after the inital lever has been activated. leaves room for animation later down the line") ]
    private float _activationDuration;

    [SerializeField]
    [Tooltip("time it takes for it to go down after the inital lever resets. leaves room for animation later down the line")]
    private float _deactivationDuration;

    [SerializeField]
    private bool _activeWhileActivating, _activeWhileDeactivating;


    private bool _active=true;

    private bool _activating = false;
    private bool _deactivating = false;

    private float _timer = 0.0f;

    private Vector3 _startPos,_endPos;

    void Awake()
    {
        _renderer = this.GetComponent<SpriteRenderer>();
        _startPos = this.transform.position;
        _endPos = transform.position +( transform.up * -0.5f);

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

    void OnTriggerEnter2D(Collider2D col)
    {
        if (_active)
        {
            if (col.tag == "Player")
            {
                col.GetComponent<CharacterController>().Die();
            }
        }
    }

    public override void Activate()
    {
        //base.Activate();
        if (_activeWhileActivating)
        {
            _active = true;
        }
        _activating = true;
        _renderer.enabled = true;

        //Debug.Log("Spike Activating");
    }



    public override void Deactivate()
    {
        //base.Deactivate();
        if(!_activeWhileDeactivating)
        {
            _active = false;
        }

        _deactivating = true;
        //Debug.Log("Spike Deactivating");
    }

    public override void Reset()
    {
        //base.Reset();
        transform.position = _endPos;
        _active = false;
    }
}

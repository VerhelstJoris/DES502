using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelRotationtrap : Trap
{
    enum RotationType { Timed, Additive, Interval };


    [Header("General")]
    //[SerializeField][Tooltip("Timed: adds x degrees and then goes back after, Additive adds x degrees after every time the button is activated, Interval is automatic after a set amount of time")]
    //private RotationType _rotationType;

    [SerializeField] [Tooltip("For always active turn on, this then works without a lever")]
    private bool _alwaysActive = false;

    [SerializeField] [Tooltip("how much the level rotates in degrees, use intervals of 90 plz")]
    private float _rotationAmount;

    [SerializeField] [Tooltip("How long does it take the level to rotate to it's next state")]
    private float _rotationDuration=0.3f;

    [SerializeField] [Tooltip("How long does it before the level rotates again/resets")]
    private float _rotationInterval;


    private bool _active = false;
    private bool _currentlyRotating = false;

    private float _activeTimer = 0.0f;
    private float _rotationTimer = 0.0f;
    private float _targetAngle = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(_active || _alwaysActive)
        {
            if (!_currentlyRotating)
            {
                _activeTimer += Time.deltaTime;
            }

            if(_activeTimer>= _rotationInterval)
            {
                Rotate();
                _activeTimer = 0.0f;
            }


        }


        if(_currentlyRotating)
        {
            _rotationTimer += Time.deltaTime;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(transform.rotation.x, transform.rotation.y, _targetAngle), _rotationTimer / _rotationDuration);


            if (_rotationTimer > _rotationDuration )
            {
                _currentlyRotating = false;
                _rotationTimer = 0.0f;
            }
        }
    }

    private void Rotate()
    {
        _currentlyRotating = true;

        _targetAngle = transform.eulerAngles.z  + _rotationAmount;

    }


    public override void Activate()
    {
        //base.Activate();
        _active = true;

        //Debug.Log("Spike Activating");
    }

    public override void Deactivate()
    {
        //base.Deactivate();
        _active = false;

        //Debug.Log("Spike Deactivating");
    }

    public override void Reset()
    {
        //base.Reset();
       
    }

}

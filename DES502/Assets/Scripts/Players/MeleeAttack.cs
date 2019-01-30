﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour {

    private BoxCollider2D _collider;
    private CharacterController _charController;


    [SerializeField]
    public HitboxID _hitboxID;


    private float _launchAmount;
    private Vector2 _defaultLaunchVector;

    // Use this for initialization
    void Awake () {
        _collider = this.GetComponent<BoxCollider2D>();
        _charController = this.GetComponentInParent<CharacterController>();

    }

    private void Start()
    {
        float launchAngle;
        Vector2 defaultDirection = new Vector2(0, 0);

        switch (_hitboxID)
        {
            case HitboxID.Up:
                launchAngle = _charController._UpAttackLaunchAngle;
                _launchAmount = _charController._UpAttackLaunchSize;
                defaultDirection = new Vector2(0, 1);
                break;
            case HitboxID.Down:
                launchAngle = _charController._DownAttackLaunchAngle;
                _launchAmount = _charController._DownAttackLaunchSize;
                defaultDirection = new Vector2(0, -1);
                break;
            case HitboxID.Side:
                launchAngle = _charController._SideAttackLaunchAngle;
                _launchAmount = _charController._SideAttackLaunchSize;
                defaultDirection = new Vector2(1, 0);
                break;
            default:
                break;
        }


        _defaultLaunchVector = defaultDirection;
    }

    // Update is called once per frame
    void Update ()
    {
		
	}

    void OnTriggerEnter2D(Collider2D col)
    {
        Vector2 launchVector=_defaultLaunchVector;
        if (!_charController._FacingRight)
        {
            _defaultLaunchVector.x *= -1;
        }

        if (col.tag=="Player")
        {
            Debug.Log("Player");
            
            col.GetComponent<Rigidbody2D>().AddForce(launchVector * _launchAmount);
        }

        if(col.tag=="Weapon")
        {
            //parry
        }

    }
}

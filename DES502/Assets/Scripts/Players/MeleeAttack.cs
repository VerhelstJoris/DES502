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
    private float _stunDuration;

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

                defaultDirection.x = Mathf.Cos(Mathf.Deg2Rad * (launchAngle+90));
                defaultDirection.y = Mathf.Sin(Mathf.Deg2Rad * (launchAngle+90));

                _stunDuration = _charController._UpAttackStunDuration;
                break;
            case HitboxID.Down:
                launchAngle = _charController._DownAttackLaunchAngle;

                defaultDirection.x = Mathf.Cos(Mathf.Deg2Rad * (launchAngle + 270));
                defaultDirection.y = Mathf.Sin(Mathf.Deg2Rad * (launchAngle + 270));

                _stunDuration = _charController._DownAttackStunDuration;

                break;
            case HitboxID.Side:
                launchAngle = _charController._SideAttackLaunchAngle;

                defaultDirection.x = Mathf.Cos(Mathf.Deg2Rad * launchAngle);
                defaultDirection.y = Mathf.Sin(Mathf.Deg2Rad * launchAngle);

                _stunDuration = _charController._SideAttackStunDuration;

                break;
            default:
                break;
        }

        defaultDirection.Normalize();
        _defaultLaunchVector = defaultDirection;
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        Vector2 launchVector=_defaultLaunchVector;
        if (!_charController._FacingRight)
        {
            launchVector.x *= -1;
        }

        if (col.tag=="Player")
        {
            //Debug.Log("Player");
            col.GetComponent<CharacterController>().RecieveHit(launchVector * _launchAmount,
               _stunDuration);
        }

        if (col.tag=="Weapon")
        {
            //parry
        }

    }

    public void SetLaunchAmount(float amount)
    {
        _launchAmount = amount;
    }
}

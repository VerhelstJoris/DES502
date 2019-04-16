using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{
    private float _launchAmount;
    private Vector2 _defaultLaunchVector;
    private float _stunDuration;
    private bool _movingRight;
    private PlayerID _owner;
    private Rigidbody2D _rb;
    private bool _currentlyDropping = false;
    private float _dropTimer;
    private float _dropDuration;
    private float _dropGravityScale;
    private CameraShake _cameraShake;

    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }

    public void Inititalize(PlayerID owner, Vector2 direction, float launchAmount, float stunduration, float dropduration, int launchspeed, float dropgravityscale)
    {
        _owner = owner;
        _defaultLaunchVector = direction;
        _launchAmount = launchAmount;
        _stunDuration = stunduration;
        _dropDuration = dropduration;
        _dropTimer = _dropDuration;
        _dropGravityScale = dropgravityscale;

        // Magic number - change to const?
        _rb.AddForce(direction * launchspeed);

        if (direction.x <= 0)
        {
            // Multiply the player's x local scale by -1.
            Vector3 theScale = transform.localScale;
            theScale.x *= -1;
            transform.localScale = theScale;
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        Vector2 launchVector = _defaultLaunchVector;
        if (col.tag == "Player")
        {
            CharacterController collidedPlayer = col.GetComponent<CharacterController>();
            if (collidedPlayer._PlayerID != _owner)
            {
                collidedPlayer.RecieveHit(launchVector * _launchAmount, _stunDuration);
                // this assumes that all players have the same camera shake values
                float[] cameraShakeValues = collidedPlayer.GetCameraShakeValues();
                _cameraShake.BeginShake(cameraShakeValues[2], cameraShakeValues[3]);
            }
        }
        else if (col.IsTouchingLayers(GetKillLayers()))
        {
            Destroy(gameObject);
        }

        // what does this do???
        if (col.tag == "Weapon")
        {
        }

    }

    void FixedUpdate()
    {
        if (!_currentlyDropping)
        {
            if (_dropTimer <= 0)
            {
                _rb.gravityScale = _dropGravityScale;
            }
            _dropTimer -= Time.deltaTime;
        }
    }

    private int GetKillLayers()
    {
        // returns the layer index for every layer mask that isn't UI
        // TODO: change this to work via serialized layermasks if this section ever needs to be altered
        return ~(LayerMask.NameToLayer("UI"));
    }
}

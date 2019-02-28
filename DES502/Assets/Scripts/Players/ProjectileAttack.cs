using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{

    private float _launchAmount;
    private Vector2 _defaultLaunchVector;
    private float _stunDuration;
    private bool _movingRight;
    PlayerID _owner;
    Rigidbody2D _rb;
    private bool _currentlyDropping = false;
    private float _dropTimer;
    private float _dropDuration;


    private void Awake()
    {
        _rb = this.GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0;
    }

    public void Inititalize(PlayerID owner, Vector2 direction, float launchAmount, float stunduration, float dropduration)
    {
        _owner = owner;
        _defaultLaunchVector = direction;
        _launchAmount = launchAmount;
        _stunDuration = stunduration;
        _dropDuration = dropduration;
        _dropTimer = _dropDuration;

        // Magic number - change to const?
        _rb.AddForce(direction * 1500);

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
            if (col.GetComponent<CharacterController>()._PlayerID != _owner)
            {
                col.GetComponent<Rigidbody2D>().AddForceAtPosition(launchVector * _launchAmount, col.transform.position);
                col.GetComponent<CharacterController>().Stun(_stunDuration);
            }
        }
        else
        {
            Destroy(gameObject);
        }

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
                _rb.gravityScale = 1;
            }
            _dropTimer -= Time.deltaTime;
        }
    }
}

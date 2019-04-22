using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowBolt : MonoBehaviour
{
    [SerializeField] [Tooltip("Speed (vector magnitude) of the crossbow bolt.")]
    private float _speed = 5;

    private SpriteRenderer _spriteRenderer;
    private Rigidbody2D _rigidbody;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public void Activate(bool facingRight)
    {
        // convert the facing right bool into 1 and -1 respectively
        int directionFacing = facingRight.GetHashCode() * 2 - 1;
        //Debug.Log("Bolt direction: " + directionFacing.ToString());
        _spriteRenderer.flipX = facingRight;
        Vector2 boltVelocity = Vector2.right * directionFacing * _speed;
        _rigidbody.velocity = boltVelocity;
    }
}

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
        //directionFacing = 1;
        //Debug.Log("Bolt direction: " + directionFacing.ToString());
        //_spriteRenderer.flipX = facingRight;
        transform.localScale = new Vector3(directionFacing * -1, 1, 1);
        Vector2 boltVelocity = Vector2.right * directionFacing * _speed;
        Debug.Log("Bolt velocity: " + boltVelocity.ToString());
        _rigidbody.velocity = boltVelocity;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            // kill player
            //Debug.Log("KILL PLAYER");
            CharacterController playerAffecting = col.gameObject.GetComponent<CharacterController>();
            playerAffecting.Die(CauseOfDeath.Crossbow);
        }
        // destroy self on collding with anything else
        else if (col.gameObject.layer == LayerMask.NameToLayer("Default"))
        {
            Destroy(gameObject);
        }
    }
}

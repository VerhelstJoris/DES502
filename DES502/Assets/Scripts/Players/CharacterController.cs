using UnityEngine;
using UnityEngine.Events;


public class CharacterController : MonoBehaviour
{
    enum PlayerID { Player1, Player2, Player3, Player4 };
    enum PlayerState { Idle, Jumping, Running, Dead };

    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform _ceilingCheck;

    const float k_groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool _grounded;            // Whether or not the player is grounded.
    const float k_CeilingRadius = .2f; // Radius of the overlap circle to determine if the player can stand up
    private Rigidbody2D _Rigidbody2D;
    private bool _FacingRight = true;  // For determining which way the player is currently facing.

    [SerializeField]
    PlayerID _PlayerID;

    
    [Header("Player Movement")]

    [Range(0, 150.0f)] [SerializeField] float _moveSpeed;
    [Range(0, .3f)] [SerializeField] private float _movementSmoothing = .05f;

    //jump related
    [Header("Jump")]

    [SerializeField] private bool _airControl = false;

    [Range(0, 150.0f)] [SerializeField] [Tooltip("How much force is added to the player every frame until the jumptime runs out")]
    float _jumpForce = 5.0f;
    [Range(0, 400.0f)] [SerializeField][Tooltip("Initial force applied to the player when he first presses jump")]
    float _intialJumpForce = 25.0f;
    [Range(0, 2.0f)] [SerializeField][Tooltip("How long the player can keep ascending in his jump in seconds")]
    float _jumpTime = 1.5f;


    float _jumpTimeCounter;

    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }


    //private

    float _horizontalMove = 0.0f;
    bool _jumpKeyDown = false;
    bool _jumping = false;
    string _inputSuffix;

    private Vector3 _Velocity = Vector3.zero;

    PlayerState _playerState = PlayerState.Idle;



    //private

    private void Awake()
    {
        _Rigidbody2D = this.GetComponent<Rigidbody2D>();

        switch (_PlayerID)
        {
            case PlayerID.Player1:
                _inputSuffix = "_P1";
                break;
            case PlayerID.Player2:
                _inputSuffix = "_P2";
                break;
            case PlayerID.Player3:
                _inputSuffix = "_P3";
                break;
            case PlayerID.Player4:
                _inputSuffix = "_P4";
                break;
            default:
                break;
        }



        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

    }

    private void FixedUpdate()
    {
        //grounded checks
        bool wasGrounded = _grounded;
        _grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck.position, k_groundedRadius, _whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                _grounded = true;
                if (!wasGrounded)
                {
                    OnLandEvent.Invoke();
                }
            }
        }

        HandlePlayerInput();
        Move(_horizontalMove,_jumpKeyDown);

    }


    public void Move(float move, bool jump)
    {
   

        //only control the player if grounded or airControl is turned on
        if (_grounded || _airControl)
        {

            // Move the character by finding the target velocity
            Vector3 targetVelocity = new Vector2(move * 10f, _Rigidbody2D.velocity.y);
            // And then smoothing it out and applying it to the character
            _Rigidbody2D.velocity = Vector3.SmoothDamp(_Rigidbody2D.velocity, targetVelocity, ref _Velocity, _movementSmoothing);


            //looking in the right direction
            if (move > 0 && !_FacingRight)
            {
                Flip();
            }
            else if (move < 0 && _FacingRight)
            {
                Flip();
            }
        }

        if (_grounded && jump)
        {
            _jumping = true;
            _jumpTimeCounter = _jumpTime;
            //_Rigidbody2D.velocity = Vector2.up * _intialJumpForce;
            _Rigidbody2D.AddForce(Vector2.up * _intialJumpForce);
        }

        if(jump && _jumping)
        {
            if(_jumpTimeCounter>0)
            {
                //_Rigidbody2D.velocity = Vector2.up * _jumpForce;
                _Rigidbody2D.AddForce(Vector2.up * _jumpForce);

                _jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                _jumping = false;
            }
        }

        if (!jump)
            _jumping = false;

    }


    private void Flip()
    {
        // Switch the way the player is labelled as facing.
        _FacingRight = !_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void HandlePlayerInput()
    {

        _horizontalMove = Input.GetAxisRaw("Horizontal" + _inputSuffix) * Time.fixedDeltaTime * _moveSpeed;

        if (Input.GetButtonDown("Jump" + _inputSuffix))
        {
            _jumpKeyDown = true;
        }
        else if (Input.GetButtonUp("Jump" + _inputSuffix))
        {
            _jumpKeyDown = false;
        }
    }

}
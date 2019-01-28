using UnityEngine;
using UnityEngine.Events;


public class CharacterController : MonoBehaviour
{
    enum PlayerID { Player1, Player2, Player3, Player4 };
    enum PlayerState { Idle, Jumping, Running, Dead };

    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform _ceilingCheck;

    [SerializeField] private Transform _leftAttackObject;
    [SerializeField] private Transform _rightAttackObject;
    [SerializeField] private Transform _downAttackObject;
    [SerializeField] private Transform _upAttackObject;

    const float k_groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool _grounded;            
    private Rigidbody2D _Rigidbody2D;
    private bool _FacingRight = true;  

    [SerializeField]
    PlayerID _PlayerID;

    
    [Header("Player Movement")]

    [Range(0, 150.0f)] [SerializeField] float _moveSpeed;
    [Range(0, .3f)] [SerializeField] private float _movementSmoothing = .05f;

    //JUMP RELATED
    //------------------------------------
    [Header("Jump")]

    [SerializeField] private bool _airControl = false;

    [Range(0, 150.0f)] [SerializeField] [Tooltip("How much force is added to the player every frame until the jumptime runs out")]
    float _jumpForce = 5.0f;
    [Range(0, 400.0f)] [SerializeField][Tooltip("Initial force applied to the player when he first presses jump")]
    float _intialJumpForce = 25.0f;
    [Range(0, 2.0f)] [SerializeField][Tooltip("How long the player can keep ascending in his jump in seconds")]
    float _jumpTime = 1.5f;


    float _jumpTimeCounter;


    //ATTACK RELATED
    //-----------------------------------
    [Header("Attacks")]
    private BoxCollider2D _leftAttackCollider, _rightAttackCollider, _downAttackCollider, _upAttackCollider;

    private float _attackDuration;
    private float _attackCooldown;

    [Header("Upwards Attack")]

    public Vector2 _UpAttackSize;
    public Vector2 _UpAttackOffset;
    [SerializeField] private float _upAttackDuration;

    [Header("Downwards Attack")]

    public Vector2 _DownAttackSize;
    public Vector2 _DownAttackOffset;
    [SerializeField] private float _downAttackDuration;

    [Header("Side Attacks")]

    public Vector2 _SideAttackSize;
    public Vector2 _SideAttackOffset;
    [SerializeField] private float _sideAttackDuration;


    //EVENTS RELATED
    //-----------------------------------
    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }


    //private

    float _horizontalMove = 0.0f;
    float _horizontalInput = 0.0f;
    float _verticalInput = 0.0f;
    bool _jumpKeyDown = false;
    bool _attackKeyDown = false;
    bool _jumping = false;
    string _inputSuffix;

    private Vector3 _Velocity = Vector3.zero;

    PlayerState _playerState = PlayerState.Idle;




    private void Awake()
    {
        _Rigidbody2D = this.GetComponent<Rigidbody2D>();

        //proper input 
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

        //attack
        _leftAttackCollider = _leftAttackObject.GetComponent<BoxCollider2D>();
        _rightAttackCollider = _rightAttackObject.GetComponent<BoxCollider2D>();
        _upAttackCollider = _upAttackObject.GetComponent<BoxCollider2D>();
        _downAttackCollider = _downAttackObject.GetComponent<BoxCollider2D>();


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

        //Movement
        HandlePlayerInput();
        Move(_horizontalMove,_jumpKeyDown);

        //Attacking
        Attack();
    }


    private void Move(float move, bool jump)
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

    private void Attack()
    {
        if (_attackKeyDown)
        {

            float absHorizontal = Mathf.Abs(_horizontalInput);
            float absVertical = Mathf.Abs(_verticalInput);

            //attack in the direction you're facing
            if (_horizontalInput == 0 && _verticalInput == 0)
            {
                if (_FacingRight)
                {
                    //RIGHT ATTACK
                }
                else
                {
                    //LEFT ATTACK
                }
            }
            else
            if (absVertical >= absHorizontal)
            {
                if(_verticalInput <0)
                {
                    //DOWN ATTACK
                }
                else
                {
                    //UP ATTACK
                }
            }
            else if(absVertical < absHorizontal)
            {
                if (_horizontalInput < 0)
                {
                    //LEFT ATTACK
                }
                else
                {
                    //RIGHT ATTACK
                }
            }


        }
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
        _horizontalInput = Input.GetAxisRaw("Horizontal" + _inputSuffix);
        _horizontalMove = _horizontalInput * Time.fixedDeltaTime * _moveSpeed;
        _verticalInput = Input.GetAxisRaw("Vertical" + _inputSuffix);

        if (Input.GetButtonDown("Jump" + _inputSuffix))
        {
            _jumpKeyDown = true;
        }
        else if (Input.GetButtonUp("Jump" + _inputSuffix))
        {
            _jumpKeyDown = false;
        }

        if(Input.GetButtonDown("Attack" + _inputSuffix))
        {
            _attackKeyDown = true;
        }
        else
        {
            _attackKeyDown = false;
        }
    }

}
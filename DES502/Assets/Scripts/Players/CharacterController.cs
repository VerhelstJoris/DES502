using UnityEngine;
using UnityEngine.Events;


public class CharacterController : MonoBehaviour
{
    

    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform _ceilingCheck;

    [SerializeField] private Transform _sideAttackObject;
    [SerializeField] private Transform _downAttackObject;
    [SerializeField] private Transform _upAttackObject;

    const float k_groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool _grounded;            
    private Rigidbody2D _Rigidbody2D;
    [HideInInspector]
    public bool _FacingRight = true;

    [Header("General")]
    [SerializeField]
    private float _deathDuration;
    [SerializeField]
    private RespawnPoint _respawnPoint;

    [SerializeField]
    PlayerID _PlayerID;

    //GENERAL MOVEMENT
    //------------------------------------
    [Header("Player Movement")]

    [Range(0, 150.0f)] [SerializeField] float _moveSpeed;
    [Range(0, .3f)] [SerializeField] private float _movementSmoothing = .05f;


    float _horizontalMove = 0.0f;
    float _horizontalInput = 0.0f;
    float _verticalInput = 0.0f;


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
    bool _jumpKeyDown = false;
    bool _jumpKeyDownAlready = false;
    bool _jumping = false;

    //ATTACK RELATED
    //-----------------------------------
    [Header("Attacks")]
    [SerializeField][Tooltip("How long the you can't attack after the attack ends")] private float _attackCooldownDuration;

    private BoxCollider2D _sideAttackCollider, _downAttackCollider, _upAttackCollider;

    [Header("Upwards Attack")]

    public Vector2 _UpAttackSize;
    public Vector2 _UpAttackOffset;
    [SerializeField] private float _upAttackDuration;
    [SerializeField] [Range(-60.0f, 60.0f)] public float _UpAttackLaunchAngle;
    [SerializeField] [Range(0.0f, 2000.0f)] public float _UpAttackLaunchSize;
    [SerializeField][Range(0.0f, 1.5f)] public float _UpAttackStunDuration;

    [Header("Downwards Attack")]

    public Vector2 _DownAttackSize;
    public Vector2 _DownAttackOffset;
    [SerializeField] private float _downAttackDuration;
    [SerializeField] [Range(-60.0f, 60.0f)] public float _DownAttackLaunchAngle;
    [SerializeField] [Range(0.0f, 2000.0f)] public float _DownAttackLaunchSize;
    [SerializeField] [Range(0.0f, 1.5f)] public float _DownAttackStunDuration;

    [Header("Side Attacks")]

    public Vector2 _SideAttackSize;
    public Vector2 _SideAttackOffset;
    [SerializeField] private float _sideAttackDuration;
    [SerializeField] [Range(-60.0f, 60.0f)] public float _SideAttackLaunchAngle;
    [SerializeField] [Range(0.0f, 2000.0f)] public float _SideAttackLaunchSize;
    [SerializeField] [Range(0.0f, 1.5f)] public float _SideAttackStunDuration;

    bool _attackKeyDown = false;
    bool _attacking = false;
    bool _AttackOnCooldown = false;
    AttackType _currentAttack = AttackType.None;

    private float _attackTimer = 0.0f;
    private float _attackCooldownTimer = 0.0f;

    bool _stunned = false;
    float _stunnedTimer=0.0f;
    float _stunnedDuration = 0.0f;

    //EVENTS RELATED
    //-----------------------------------
    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }    

    //random

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
        _sideAttackCollider = _sideAttackObject.GetComponent<BoxCollider2D>();
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
        // halt movement if attacking while on the ground
        if (_attacking && _grounded)
        {
            _horizontalMove = 0;
        }
        else
        {
            _horizontalMove = _horizontalInput * Time.fixedDeltaTime * _moveSpeed;
        }
        Move(_horizontalMove,_jumpKeyDown);

        //Attacking
        if (_attackKeyDown)
        {
            StartAttack();
        }

        AttackTick();

        //being stunned
        if (_stunned)
        {
            StunTick();
        }
    }

    private void Move(float move, bool jump)
    {
   
        //only control the player if grounded or airControl is turned on
        if ((_grounded || (_airControl)) && !_stunned)
        {

            // Move the character by finding the target velocity
            float magicNumber = 10f;  // ???
            Vector3 targetVelocity = new Vector2(move * magicNumber, _Rigidbody2D.velocity.y);
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

        // this will trigger multiple times because the player will still be close enough to the ground to pass the check of being grounded
        if (_grounded && jump && !_jumpKeyDownAlready)
        {
            _jumping = true;
            _jumpTimeCounter = _jumpTime;
            _Rigidbody2D.AddForce(Vector2.up * _intialJumpForce);

            Debug.Log("Started Jump");
        }

        if(jump && _jumping)
        {
            //Debug.Log("Jumping");


            if (_jumpTimeCounter>0)
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

        // make sure jump key checks will only return true once per press
        if (jump)
        {
            _jumpKeyDownAlready = true;
        }
        else
        {
            _jumpKeyDownAlready = false;
        }

    }

    private void StartAttack()
    {
        if (!_attacking && !_AttackOnCooldown)
        {

            float absHorizontal = Mathf.Abs(_horizontalInput);
            float absVertical = Mathf.Abs(_verticalInput);


            _attacking = true;

            if (absVertical > absHorizontal)
            {
                if(_verticalInput <0)
                {
                    if(!_grounded)
                    {
                        //DOWN ATTACK
                        _currentAttack = AttackType.Down;
                        _downAttackObject.GetComponent<SpriteRenderer>().enabled = true;
                        _downAttackCollider.enabled = true;
                    }
                    else
                    {
                        _attacking = false;
                    }
                }
                else
                {
                    //UP ATTACK
                    _currentAttack = AttackType.Up;
                    _upAttackObject.GetComponent<SpriteRenderer>().enabled = true;
                    _upAttackCollider.enabled = true;
                }
            }
            else if(absHorizontal >= absVertical)
            {
                //SIDE ATTACK
                _currentAttack = AttackType.Side;
                _sideAttackObject.GetComponent<SpriteRenderer>().enabled = true;
                _sideAttackCollider.enabled = true;
            }
        }
    }

    private void AttackTick()
    {
        //attack cooldown
        if(_AttackOnCooldown)
        {
            _attackCooldownTimer += Time.deltaTime;

            if(_attackCooldownTimer >= _attackCooldownDuration)
            {
                _attackCooldownTimer = 0.0f;
                _AttackOnCooldown = false;
            }
        }

        //attack timer
        if(_attacking)
        {
            _attackTimer += Time.deltaTime;
            bool attackReset = false;


            //reset after attack finishes
            switch (_currentAttack)
            {
                //collider specific changes
                case AttackType.Side:
                    if (_attackTimer > _sideAttackDuration)
                    {
                        attackReset = true;
                        _sideAttackObject.GetComponent<SpriteRenderer>().enabled = false;
                        _sideAttackCollider.enabled = false;
                    }
                    break;
                case AttackType.Up:
                    if (_attackTimer > _upAttackDuration)
                    {
                        attackReset = true;
                        _upAttackObject.GetComponent<SpriteRenderer>().enabled = false;
                        _upAttackCollider.enabled = false;
                    }
                    break;
                case AttackType.Down:
                    if (_attackTimer > _downAttackDuration)
                    {
                        attackReset = true;
                        _downAttackObject.GetComponent<SpriteRenderer>().enabled = false;
                        _downAttackCollider.enabled = false;
                    }
                    break;
                case AttackType.None:
                    break;
                default:
                    break;
            }

            //general reset
            if (attackReset)
            {
                _attackTimer = 0.0f;
                _attacking = false;
                _currentAttack = AttackType.None;
                _AttackOnCooldown = true;
            }
        }
        //hitting something
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
        _verticalInput = Input.GetAxisRaw("Vertical" + _inputSuffix);

        if (Input.GetButtonDown("Jump" + _inputSuffix))
        {
            _jumpKeyDown = true;
        }
        else if (Input.GetButtonUp("Jump" + _inputSuffix))
        {
            _jumpKeyDown = false;
        }

        if (Input.GetButtonDown("Attack" + _inputSuffix))
        {
            _attackKeyDown = true;
        }
        else
        {
            _attackKeyDown = false;
        }
    }

    public void Stun(float duration)
    {
        _stunned = true;
        _stunnedDuration = duration;
        this.GetComponent<SpriteRenderer>().color = Color.red;
    }

    private void StunTick()
    {
        _stunnedTimer += Time.deltaTime;

        if(_stunnedTimer>=_stunnedDuration)
        {
            _stunned = false;
            _stunnedTimer = 0.0f;
            this.GetComponent<SpriteRenderer>().color = Color.white;

        }
    }

    public void Die()
    {
        Debug.Log("Died");
        _respawnPoint.GetComponent<RespawnPoint>().Respawn();
        this.transform.position = _respawnPoint.transform.position;
    }

}

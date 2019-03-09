﻿using UnityEngine;
using UnityEngine.Events;


public class CharacterController : MonoBehaviour
{
    [HideInInspector]
    public GameManager _GameManager;

    private Animator _animator;
    private Rigidbody2D _rigidbody;
    private BoxCollider2D _collider;
    [HideInInspector]
    public PlayerTag _PlayerTag;
    [HideInInspector]
    public PlayerUI _PlayerUI;


    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform _ceilingCheck;
    [SerializeField] private Transform _projectileSpawn;

    [SerializeField] private Transform _sideAttackObject;
    [SerializeField] private Transform _downAttackObject;
    [SerializeField] private Transform _upAttackObject;

    const float k_groundedRadius = .2f; // Radius of the overlap circle to determine if grounded
    private bool _grounded;
    private bool _hitCeiling;  // used to cancel jumping early if hitting a ceiling
    const float k_ceilingHitRadius = .1f; // Radius of the overlap circle to determine if grounded
    [HideInInspector]
    public bool _FacingRight = true;

    [Header("General")]
    [SerializeField] [Tooltip("Death duration will always be the same across all player characters")]
    public static float RespawnDuration = 1.5f;

    public PlayerID _PlayerID;
    public TeamID _TeamID;

    //GENERAL MOVEMENT
    //------------------------------------
    [Header("Player Movement")]

    [Range(0, 150.0f)] [SerializeField] float _moveSpeed;
    [Range(0, .3f)] [SerializeField] private float _movementSmoothing = .05f;


    float _horizontalMove = 0.0f;
    float _horizontalInput = 0.0f;
    float _verticalInput = 0.0f;

    bool _running = false;

    //JUMP RELATED
    //------------------------------------
    [Header("Jump")]

    [SerializeField] private bool _airControl = false;

    [Range(0, 2.0f)] [SerializeField] [Tooltip("How high (in grid units) will the minimum jump height reach?")]
    float _minJumpHeight = 0.5f;
    [Range(0, 1.0f)] [SerializeField] [Tooltip("How long (in seconds) does it take to reach the minimum jump height?")]
    float _minJumpTime = 0.2f;
    [Range(0, 5.0f)] [SerializeField] [Tooltip("How high (in grid units) will the maximum jump height reach?")]
    float _maxJumpHeight = 3.0f;
    [SerializeField] [Tooltip("Should fall speed be clamped?")]
    bool _shouldClampFallSpeed = true;
    [Range(-50.0f, 0)] [SerializeField] [Tooltip("How fast should fall speed be clamped to?")]
    float _maxFallSpeed = -20.0f;

    float _jumpTimeCounter;
    bool _jumpKeyDown = false;
    bool _jumpKeyDownAlready = false;
    bool _jumping = false;
    float _jumpVelocity = 0.0f;
    float _maxJumpTime = 0.0f;

    //ATTACK RELATED
    //-----------------------------------
    [Header("Attacks")]
    [SerializeField] [Tooltip("How long that you can't attack after the attack ends")] private float _attackCooldownDuration;

    private BoxCollider2D _sideAttackCollider, _downAttackCollider, _upAttackCollider;

    [Header("Upwards Attack")]

    public Vector2 _UpAttackSize;
    public Vector2 _UpAttackOffset;
    [SerializeField] private float _upAttackDuration;
    [SerializeField] [Range(-60.0f, 60.0f)] public float _UpAttackLaunchAngle;
    [SerializeField] [Range(0.0f, 2000.0f)] public float _UpAttackMaxLaunchSize;
    [SerializeField] [Range(0.0f, 2000.0f)] public float _UpAttackMinLaunchSize;
    [SerializeField] [Range(0.0f, 1.5f)] public float _UpAttackStunDuration;
    [SerializeField] [Range(0.0f, 2.5f)] public float _UpAttackHoldDuration;

    [Header("Downwards Attack")]

    public Vector2 _DownAttackSize;
    public Vector2 _DownAttackOffset;
    [SerializeField] private float _downAttackDuration;
    [SerializeField] [Range(-60.0f, 60.0f)] public float _DownAttackLaunchAngle;
    [SerializeField] [Range(0.0f, 2000.0f)] public float _DownAttackMaxLaunchSize;
    [SerializeField] [Range(0.0f, 2000.0f)] public float _DownAttackMinLaunchSize;
    [SerializeField] [Range(0.0f, 1.5f)] public float _DownAttackStunDuration;
    [SerializeField] [Range(0.0f, 2.5f)] public float _DownAttackHoldDuration;

    [Header("Side Attacks")]

    public Vector2 _SideAttackSize;
    public Vector2 _SideAttackOffset;
    [SerializeField] private float _sideAttackDuration;
    [SerializeField] [Range(-60.0f, 60.0f)] public float _SideAttackLaunchAngle;
    [SerializeField] [Range(0.0f, 2000.0f)] public float _SideAttackMaxLaunchSize;
    [SerializeField] [Range(0.0f, 2000.0f)] public float _SideAttackMinLaunchSize;
    [SerializeField] [Range(0.0f, 1.5f)] public float _SideAttackStunDuration;
    [SerializeField] [Range(0.0f, 2.5f)] public float _SideAttackHoldDuration;

    bool _attackKeyDown = false;
    bool _chargingAttack = false;
    bool _attacking = false;
    bool _attackOnCooldown = false;
    AttackType _currentAttack = AttackType.None;

    private float _attackTimer = 0.0f;
    private float _attackCooldownTimer = 0.0f;
    private float _attackChargeTimer=0.0f;

    bool _stunned = false;
    float _stunnedTimer = 0.0f;
    float _stunnedDuration = 0.0f;

    //PROJECTILE ATTACK related
    //----------------------------------
    [Header("Projectile Attack")]
    [SerializeField] [Range(0.0f, 5.0f)] [Tooltip("Duration in which you can't fire another projectile")] private float _projectileCooldownDuration = 2.5f;
    [SerializeField] [Range(0.0f, 1.5f)] [Tooltip("Duration for which opponents hit are stunned")] private float _projectileStunDuration = 0.25f;
    [SerializeField] [Range(0.0f, 1000.0f)] [Tooltip("By how much oppents hit are launched")] private float _projectileLaunchAmount = 200.0f;
    [SerializeField] [Range(0.0f, 0.5f)] [Tooltip("Duration inbetween pressing the button and the projectile firing")] private float _projectileStartupDuration = 0.5f;
    [SerializeField] [Range(0.0f, 0.2f)] [Tooltip("Duration before projectiles start to be affected by gravity")] private float _projectileDropDuration = 0.1f;
    [SerializeField] [Range(0, 2000)] [Tooltip("Launch force to apply to projectiles")] private int _projectileLaunchSpeed = 1600;
    [SerializeField] [Range(0.0f, 3.0f)] [Tooltip("Gravity scale to use when projectiles start to drop")] private float _projectileGravityScale = 1.0f;

    bool _specialAttackKeyDown = false;
    bool _specialAttacking = false;

    private bool _projectileFiring = false;
    private bool _projectileOnCooldown = false;
    private float _projectileCooldownTimer = 0.0f;
    private float _projectileFiringTimer = 0.0f;


    //EVENTS RELATED
    //-----------------------------------
    [Header("Events")]
    [Space]

    public UnityEvent OnLandEvent;

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool> { }

    //GAMEMODE RELATED
    //-----------------------------------
    [HideInInspector]
    public int _AmountOfStocks;

    [HideInInspector]
    public int _AmountOfDeaths;
    //random

    string _inputSuffix;

    private Vector3 _Velocity = Vector3.zero;

    public PlayerState _PlayerState = PlayerState.Idle;

    public void Initialize(PlayerData data)
    {
        _PlayerID = data.Id;
        _AmountOfStocks = data.Stocks;
        _TeamID = data.TeamId;
        _AmountOfDeaths = data.Deaths;

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


        //player UI
        _GameManager.CreatePlayerUI(this);
        _GameManager.AddPlayer(this);

    }

    private void Awake()
    {
        _rigidbody = this.GetComponent<Rigidbody2D>();
        _animator = this.GetComponent<Animator>();
        _GameManager = FindObjectOfType<GameManager>();
        _collider = this.GetComponent<BoxCollider2D>();


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

        ConfigureJump(_minJumpHeight, _minJumpTime, _maxJumpHeight);


    }

    private void FixedUpdate()
    {
        _grounded = IsGrounded();
        if (!_grounded)
        {
            CheckIfLandingOnHead();
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
        if (_attackKeyDown || _specialAttackKeyDown)
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
            float magicNumber = 10f;  // this directly affects move speed, refactor?
            Vector3 targetVelocity = new Vector2(move * magicNumber, _rigidbody.velocity.y);

            // And then smoothing it out and applying it to the character
            _rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, targetVelocity, ref _Velocity, _movementSmoothing);



            if(_grounded && move!=0 && !_running)
            {
                _animator.SetBool("Running", true);
                _running = true;
                _collider.size = new Vector2(0.75f, 0.65f);
            }
            else if(_grounded && move ==0 && _horizontalInput==0 && _running)
            {
                _animator.SetBool("Running", false);
                _running = false;
                _collider.size = new Vector2(0.5f, 0.9f);
            }

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

        // should we start jumping?
        if (_grounded && jump && !_jumpKeyDownAlready)
        {
        // _grounded will still return true as you begin to jump!
            _jumping = true;
            _jumpTimeCounter = _maxJumpTime;
            //Debug.Log("Started Jump");
            _animator.SetBool("Jumping", true);

        }

        // currently jumping
        if (jump && _jumping)
        {
            if (_jumpTimeCounter >= 0)
            {
                if (!IsHittingCeiling())
                {
                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, _jumpVelocity);
                }
                else
                {
                    _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
                    _jumping = false;
                }
                _jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                _jumping = false;
            }
        }

        // Don't clamp this during hit stun?
        if (_shouldClampFallSpeed) // && _rigidbody.velocity.y < 0)
        {
            // We create this multiple times, could optimise by pooling together the y velocity value changes then create the new rigidbody value only once
            _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, Mathf.Max(_rigidbody.velocity.y, _maxFallSpeed));
            //Debug.Log("_rigidbody.velocity.y = " + _rigidbody.velocity.y);
        }

        if (!jump)
            _jumping = false;
            // TODO: Ensure minimum jump height reached

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
        //MELEE
        if (_attackKeyDown)
        {
            if (!_attacking && !_attackOnCooldown)
            {

                float absHorizontal = Mathf.Abs(_horizontalInput);
                float absVertical = Mathf.Abs(_verticalInput);

                _attacking = true;

                
                if (absVertical > absHorizontal)
                {
                    if (_verticalInput < 0)
                    {
                        if (!_grounded)
                        {
                            //DOWN ATTACK
                            //_currentAttack = AttackType.Down;
                           
                        }
                        else
                        {
                            _attacking = false;
                            _currentAttack = AttackType.None;
                        }
                    }
                    else
                    {
                        //UP ATTACK
                        _currentAttack = AttackType.Up;
                        
                    }
                }
                else if (absHorizontal >= absVertical)
                {
                    //SIDE ATTACK
                    _currentAttack = AttackType.Side; 
                }


                //decide if attack is immediatly released or charged
                if(_currentAttack != AttackType.None)
                {
                    //can't charge Attack while in mid-air
                    if(!_grounded)
                    {
                        ReleaseAttack();
                    }
                    else
                    {
                        _chargingAttack = true;
                        this.GetComponent<SpriteRenderer>().color = new Color(0, 1.0f, 0);
                    }
                }
            }
        }


        //SPECIAL ATTACK
        if (_specialAttackKeyDown)
        {
            if (!_projectileOnCooldown)
            {
                _projectileFiring = true;
            }
        }
       
    }

    private void ReleaseAttack()
    {
        _chargingAttack = false;
        this.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f);


        switch (_currentAttack)
        {
            //collider specific changes
            case AttackType.Side:
                _sideAttackObject.GetComponent<SpriteRenderer>().enabled = true;
                _sideAttackCollider.enabled = true;
                break;
            case AttackType.Up:
                _upAttackObject.GetComponent<SpriteRenderer>().enabled = true;
                _upAttackCollider.enabled = true;
                break;
            case AttackType.Down:
                _downAttackObject.GetComponent<SpriteRenderer>().enabled = true;
                _downAttackCollider.enabled = true;
                break;
            case AttackType.None:
                break;
            default:
                break;
        }
    }

    private void AttackTick()
    {
        //attack cooldown
        if (_attackOnCooldown)
        {
            _attackCooldownTimer += Time.deltaTime;

            if (_attackCooldownTimer >= _attackCooldownDuration)
            {
                _attackCooldownTimer = 0.0f;
                _attackOnCooldown = false;
            }
        }

        //charging attack
        if(_chargingAttack)
        {
            Debug.Log("CHARGING");
            _attackChargeTimer += Time.deltaTime;

            switch (_currentAttack)
            {
                //collider specific changes
                case AttackType.Side:
                    if (_attackChargeTimer > _SideAttackHoldDuration)
                    {
                        ReleaseAttack();
                        _attackChargeTimer = 0.0f;
                        _chargingAttack = false;
                    }
                    break;
                case AttackType.Up:
                    if (_attackChargeTimer > _UpAttackHoldDuration)
                    {
                        ReleaseAttack();
                        _attackChargeTimer = 0.0f;
                        _chargingAttack = false;
                    }
                    break;
                case AttackType.Down:
                    if (_attackChargeTimer > _DownAttackHoldDuration)
                    {
                        ReleaseAttack();
                        _attackChargeTimer = 0.0f;
                        _chargingAttack = false;
                    }
                    break;
                case AttackType.None:
                    break;
                default:
                    break;
            }
        }


        //attack timer
        if (_attacking && !_chargingAttack)
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
                _attackOnCooldown = true;
            }
        }


        //projectile cooldown
        if (_projectileOnCooldown)
        {
            _projectileCooldownTimer += Time.deltaTime;

            if (_projectileCooldownTimer >= _projectileCooldownDuration)
            {
                _projectileOnCooldown = false;
                _projectileCooldownTimer = 0.0f;
            }
        }


        if(_projectileFiring)
        {
            // is this startup on a projectile?
            _projectileFiringTimer += Time.deltaTime;
            if (_projectileFiringTimer >= _projectileStartupDuration)
            {
                
                //actually create the projectile
                Vector2 direction = new Vector2(1, 0);
                if (!_FacingRight)
                {
                    direction.x = -1;
                }

                ObjectFactory.CreateProjectile(_PlayerID, _projectileSpawn.transform.position, direction, _projectileLaunchAmount, _projectileStunDuration, _projectileDropDuration, _projectileLaunchSpeed, _projectileGravityScale);

                _projectileFiring = false;
                _projectileOnCooldown = true;
                _projectileFiringTimer = 0.0f;

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
        else if (Input.GetButtonUp("Attack" + _inputSuffix))
        {
            _attackKeyDown = false;

            if (_chargingAttack)
            {
                ReleaseAttack();
            }
        }

        if(Input.GetButtonDown("SpecialAttack" + _inputSuffix))
        {
            _specialAttackKeyDown = true;
        }
        else
        {
            _specialAttackKeyDown = false;


        }
    }

    public void Stun(float duration)
    {
        _stunned = true;
        _stunnedDuration = duration;
        this.GetComponent<SpriteRenderer>().color = Color.red;
        _animator.SetBool("Stunned", true);

    }

    private void StunTick()
    {
        _stunnedTimer += Time.deltaTime;

        if(_stunnedTimer>=_stunnedDuration)
        {
            _stunned = false;
            _stunnedTimer = 0.0f;
            this.GetComponent<SpriteRenderer>().color = Color.white;
            _animator.SetBool("Stunned", false);

        }
    }

    public void Die()
    {
        _PlayerState = PlayerState.Dead;
        _AmountOfStocks--;
        _AmountOfDeaths++;

        PlayerData data;
        data.Id = _PlayerID;
        data.Stocks = _AmountOfStocks;
        data.TeamId = _TeamID;
        data.Deaths = _AmountOfDeaths;

        if (_PlayerTag)
        {
            Destroy(_PlayerTag.gameObject);
        }

        _PlayerUI.UpdateStockText(_AmountOfStocks);
        _GameManager.PlayerDeath(this);

        if ( _AmountOfStocks > 0 || _GameManager._WinCondition != GameWinCondition.STOCKS)
        {
            RespawnPoint point = _GameManager.FindBestRespawnPoint(_PlayerID);

            point.Activate(data);
        }

        Destroy(gameObject);

    }

    private void ConfigureJump(float min_height, float min_time, float max_height)
    {
        // calculate values from supplied arguments
        float delta = 1.0f / 8;  // based on physics velocity update ticks but this might be wrong...
        float gravity = (2 * min_height) / (2 * min_time);
        float jumpVelocity = Mathf.Sqrt(2 * gravity * min_height);
        float maxTime = Mathf.Sqrt((2 * max_height) / (gravity + jumpVelocity));
        // apply values to member variables to be used in jump/gravity calculation
        //Physics2D.gravity = new Vector2(0, -gravity * delta);
        // still innacurate, but feels more accurate without multiplying by delta
        Physics2D.gravity = new Vector2(0, -gravity);
        _jumpVelocity = jumpVelocity;
        _maxJumpTime = maxTime;
    }

    private bool IsHittingCeiling()
    {
        // Check to see if the character is currently hitting a ceiling
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_ceilingCheck.position, k_ceilingHitRadius, _whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                return true;
            }
        }
        return false;  // return false if we haven't already returned true
    }

    private bool IsGrounded()
    {
        bool wasGrounded = _grounded;
        bool grounded = false;
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck.position, k_groundedRadius, _whatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                if (!wasGrounded)
                {
                    OnLandEvent.Invoke();
                    _animator.SetBool("Jumping", false);
                    //_projectileOnCooldown = false;
                    //_projectileCooldownTimer = 0.0f;
                }
            }
        }
        return grounded;
    }

    private void CheckIfLandingOnHead()
    {
        // This whole way of checking seems horribly unoptimal - should be first on the table if it comes to optimising code
        int playerLayerMask = LayerMask.GetMask("Player");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck.position, k_groundedRadius, playerLayerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "Player")
            {
                //if (colliders[i].gameObject.GetComponent<CharacterController>()._PlayerTag != _PlayerTag)
                if (colliders[i].gameObject != this.gameObject)
                {
                    Debug.Log("COLLIDED WITH PLAYER");
                }
            }
            //_rigidbody.AddForce(Vector2.up * 100);
            /*
            if (colliders[i].gameObject != gameObject)
            {
                grounded = true;
                if (!wasGrounded)
                {
                    OnLandEvent.Invoke();
                    _animator.SetBool("Jumping", false);
                    //_projectileOnCooldown = false;
                    //_projectileCooldownTimer = 0.0f;
                }
            }
            */
        }
    }
}

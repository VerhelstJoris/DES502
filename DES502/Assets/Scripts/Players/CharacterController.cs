using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class CharacterController : MonoBehaviour
{
    [HideInInspector]
    public GameManager _GameManager;
    [HideInInspector]
    public PlayerTag _PlayerTag;
    [HideInInspector]
    public PlayerUI _PlayerUI;
    [SerializeField]
    private RuntimeAnimatorController[] _rabbitAnimators, _foxAnimators;

    private Rigidbody2D _rigidbody;
    private Animator _animator;
    private BoxCollider2D _collider;
    private AudioSource _source;

    [SerializeField] private LayerMask _whatIsGround;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private Transform _ceilingCheck;
    [SerializeField] private Transform _projectileSpawn;

    [SerializeField] private Transform _sideAttackObject;
    [SerializeField] private Transform _downAttackObject;
    [SerializeField] private Transform _upAttackObject;

    const float k_groundedRadius = .25f; // Radius of the overlap circle to determine if grounded
    private bool _grounded;
    private bool _hitCeiling;  // used to cancel jumping early if hitting a ceiling
    const float k_ceilingHitRadius = .1f; // Radius of the overlap circle to determine if grounded
    [HideInInspector]
    public bool _FacingRight = true;
    private bool _previousFacingRight = true;

    [Header("General")]
    [SerializeField] [Tooltip("Death duration will always be the same across all player characters")]
    public static float RespawnDuration = 1.5f;

    public PlayerID _PlayerID;
    public TeamID _TeamID;
    public CharacterID _CharID;
    public ControllerID _ControllerID;
    public int _SkinID;

    //SOUND RELATED
    //--------------------------------------

    [Header("Sound Related")]
    [SerializeField]
    public List<DeathSettings> _deathSettings;

    [SerializeField]
    public KeyValuePair<CauseOfDeath, AudioClip> list;
    
    [SerializeField]
    private PlayerAudioScriptableObject _rabbitAudioObject, _foxAudioObject;
    private PlayerAudioScriptableObject _audioScriptableObject;


    //GENERAL MOVEMENT
    //------------------------------------
    [Header("Player Movement")]
    [Range(0, 150.0f)] [SerializeField] float _moveSpeed;
    [Range(0, .3f)] [SerializeField] private float _movementSmoothing = .05f;
    [SerializeField] private bool _useTurnaround = true;
    [Range(0, .5f)] [SerializeField] private float _turnaroundDuration = .05f;

    private float _horizontalMove = 0.0f;
    private float _horizontalInput = 0.0f;
    private float _verticalInput = 0.0f;
    private bool _running = false;
    private float _turnaroundTimer;
    private bool _isTurningAround = false;

    //JUMP RELATED
    //------------------------------------
    [Header("Jump")]
    [SerializeField] private bool _airControl = false;
    [Range(0, 2.0f)] [SerializeField] [Tooltip("How high (in grid units) will the minimum jump height reach?")]
    private float _minJumpHeight = 0.5f;
    [Range(0, 1.0f)] [SerializeField] [Tooltip("How long (in seconds) does it take to reach the minimum jump height?")]
    private float _minJumpTime = 0.2f;
    [Range(0, 20.0f)] [SerializeField] [Tooltip("How high (in grid units) will the maximum jump height reach?")]
    private float _maxJumpHeight = 3.0f;
    [SerializeField] [Tooltip("Should fall speed be clamped?")]
    private bool _shouldClampFallSpeed = true;
    [Range(-50.0f, 0)] [SerializeField] [Tooltip("How fast should fall speed be clamped to?")]
    private float _maxFallSpeed = -20.0f;

    private float _jumpTimeCounter;
    private bool _jumpKeyDown = false;
    private bool _jumpKeyDownAlready = false;
    private bool _jumping = false;
    private float _jumpVelocity = 0.0f;
    private float _maxJumpTime = 0.0f;

    // These values aren't as important to fine tweak as regular jumping
    [Header("Player Stomp")]
    [Range(0, 1000)] [SerializeField] [Tooltip("How much force should be added to the player while bouncing?")]
    int _playerStompJumpHeight = 500;
    [Range(0, 1000)] [SerializeField] [Tooltip("How much force should be added to the player being bounced on?")]
    int _playerStompKnockbackHeight = 500;
    [Range(0f, 2f)] [SerializeField] [Tooltip("How long should the player being bounced on be stunned for?")]
    float _playerStompStunDuration = 0.5f;
    [Range(0f, 0.5f)] [SerializeField] [Tooltip("How long should the cooldown be inbetween stomps? (Note: this is to prevent the check returning true multiple times for the same stomp - make this value as low as it will go without causing issues)")]
    float _playerStompCooldownDuration = 0.2f;
    float _playerStompCooldownTimer;
    bool _playerStompOnCooldown = false;

    //ATTACK RELATED
    //-----------------------------------
    [Header("Attacks")]
    [SerializeField] [Tooltip("How long that you can't attack after the attack ends")]
    private float _attackCooldownDuration;

    private BoxCollider2D _sideAttackCollider, _downAttackCollider, _upAttackCollider;
    private MeleeAttack _sideAttack, _downAttack, _upAttack;

    [SerializeField] [Range(0, 2)]
    [Tooltip("Max distance for camera shake from origin.")]
    private float _cameraShakeIntensityMelee = 0.5f;
    [SerializeField] [Range(0, 2)]
    [Tooltip("Max distance for camera shake from origin.")]
    private float _cameraShakeDurationMelee = 0.25f;

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

    private bool _attackKeyDown = false;
    private bool _chargingAttack = false;
    private bool _attacking = false;
    private bool _attackOnCooldown = false;
    private bool _attackWindupFinished = false;
    private AttackType _currentAttack = AttackType.None;
    private float _attackResetBackupTimer = 0.0f;
    private bool _attackAnimPlaying = false;

    private float _attackCooldownTimer = 0.0f;
    private float _attackChargeTimer=0.0f;
    private bool _stunned = false;

    //PROJECTILE ATTACK RELATED
    //----------------------------------
    [Header("Projectile Attack")]
    [SerializeField] [Range(0.0f, 5.0f)] [Tooltip("Duration in which you can't fire another projectile")] 
    private float _projectileCooldownDuration = 2.5f;
    [SerializeField] [Range(0.0f, 1.5f)] [Tooltip("Duration for which opponents hit are stunned")] 
    private float _projectileStunDuration = 0.25f;
    [SerializeField] [Range(0.0f, 1000.0f)] [Tooltip("By how much oppents hit are launched")]
    private float _projectileLaunchAmount = 200.0f;
    [SerializeField] [Range(0.0f, 0.5f)] [Tooltip("Duration inbetween pressing the button and the projectile firing")]
    private float _projectileStartupDuration = 0.5f;
    [SerializeField] [Range(0.0f, 0.2f)] [Tooltip("Duration before projectiles start to be affected by gravity")]
    private float _projectileDropDuration = 0.1f;
    [SerializeField] [Range(0, 2000)] [Tooltip("Launch force to apply to projectiles")]
    private int _projectileLaunchSpeed = 1600;
    [SerializeField] [Range(0.0f, 3.0f)] [Tooltip("Gravity scale to use when projectiles start to drop")] 
    private float _projectileGravityScale = 1.0f;
    [SerializeField] [Range(0, 2)]
    [Tooltip("Max distance for camera shake from origin.")]
    private float _cameraShakeIntensityProjectile = 0.5f;
    [SerializeField] [Range(0, 2)]
    [Tooltip("Max distance for camera shake from origin.")]
    private float _cameraShakeDurationProjectile = 0.25f;

    private bool _specialAttackKeyDown = false;
    private bool _specialAttacking = false;
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
    string _inputSuffix;
    private Vector3 _Velocity = Vector3.zero;
    public PlayerState _PlayerState = PlayerState.Idle;

    // POWERUP RELATED
    //-------------------------------------
    // effects
    [HideInInspector]
    public bool _controlsReversed = false;
    [HideInInspector]
    public float _moveSpeedMultiplier = 1;
    [HideInInspector]
    public bool _rooted = false;
    [HideInInspector]
    public bool _shielded = false;
    [HideInInspector]
    public bool _meleeInstantKill = false;

    

    public void Initialize(PlayerData data)
    {
        _PlayerID = data.Id;
        _AmountOfStocks = data.Stocks;
        _TeamID = data.TeamId;
        _AmountOfDeaths = data.Deaths;
        _CharID = data.charID;
        _SkinID = data.skinID;
        //proper input + animator
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
        switch (_CharID)
        {
            case CharacterID.Fox:
                _animator.runtimeAnimatorController = _foxAnimators[_SkinID];
                _audioScriptableObject = _foxAudioObject;
                break;
            case CharacterID.Rabbit:
                _animator.runtimeAnimatorController = _rabbitAnimators[_SkinID];
                _audioScriptableObject = _rabbitAudioObject;
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
        _source = this.GetComponent<AudioSource>();

        //proper input + animator
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

        switch (_CharID)
        {
            case CharacterID.Fox:
                _animator.runtimeAnimatorController = _foxAnimators[_SkinID];
                break;
            case CharacterID.Rabbit:
                _animator.runtimeAnimatorController = _rabbitAnimators[_SkinID];
                break;
        }

        //attack
        _sideAttackCollider = _sideAttackObject.GetComponent<BoxCollider2D>();
        _upAttackCollider = _upAttackObject.GetComponent<BoxCollider2D>();
        _downAttackCollider = _downAttackObject.GetComponent<BoxCollider2D>();

        _sideAttack = _sideAttackObject.GetComponent<MeleeAttack>();
        _upAttack = _upAttackObject.GetComponent<MeleeAttack>();
        _downAttack = _downAttackObject.GetComponent<MeleeAttack>();

        if (OnLandEvent == null)
            OnLandEvent = new UnityEvent();

        ConfigureJump(_minJumpHeight, _minJumpTime, _maxJumpHeight);
        //_defaultGravityValue = Physics2D.gravity;

        //setting animation length
        //get length of animation clip
        RuntimeAnimatorController ac = _animator.runtimeAnimatorController;    //Get Animator controller
        for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
        {
            if (ac.animationClips[i].name == "Attack_Side")        //If it has the same name as your clip
            {
                float sideAttackDurationAnim = ac.animationClips[i].length;
                _animator.SetFloat("Attack_Side_Speed", sideAttackDurationAnim / _sideAttackDuration);
            }

            if (ac.animationClips[i].name == "Attack_Up")        //If it has the same name as your clip
            {
                float upAttackDurationAnim = ac.animationClips[i].length;
                _animator.SetFloat("Attack_Up_Speed", upAttackDurationAnim / _upAttackDuration);
            }
        }

    }

    private void FixedUpdate()
    {
        _grounded = IsGrounded();
        // TODO: change to only check while falling
        if (!_grounded && !_playerStompOnCooldown)
        {
            CheckIfLandingOnHead();
        }
        if (_playerStompOnCooldown)
        {
            _playerStompCooldownTimer -= Time.deltaTime;
            if (_playerStompCooldownTimer <= 0)
            {
                _playerStompOnCooldown = false;
            }
        }

        //Movement
        if(_PlayerState != PlayerState.Dead)
        {
            HandlePlayerInput();
            // halt movement if attacking while on the ground
            if (_attacking && _grounded)
            {
                _horizontalMove = 0;
            }
            else
            {
                _horizontalMove = _horizontalInput * ((_controlsReversed.GetHashCode() * 2 - 1) * -1)
                    * _moveSpeed * _moveSpeedMultiplier * Time.fixedDeltaTime;
            }
            Move(_horizontalMove, _jumpKeyDown);
        }

        //Attacking
        if (_attackKeyDown || _specialAttackKeyDown)
        {
            StartAttack();
        }

        AttackTick();
    }

    private void Move(float move, bool jump)
    {

        if (_PlayerState != PlayerState.Dead)
        {
            // Walking
            // only control the player if grounded or airControl is turned on
            if ((_grounded || (_airControl)) && !_stunned)
            {

                // Move the character by finding the target velocity
                float magicNumber = 10f;  // this directly affects move speed, refactor?
                Vector3 targetVelocity = new Vector2(move * magicNumber, _rigidbody.velocity.y);

                // And then smoothing it out and applying it to the character

                bool shouldMove = true;
                if (_attacking || _rooted)
                {
                    shouldMove = false;
                }

                if (_grounded && move != 0 && !_running)
                {
                    _animator.SetBool("Running", true);
                    _running = true;
                    _collider.size = new Vector2(0.75f, 0.65f);
                }
                else if (_grounded && move == 0 && _horizontalInput == 0 && _running)
                {
                    _animator.SetBool("Running", false);
                    _running = false;
                    _collider.size = new Vector2(0.5f, 0.9f);
                }

                // looking in the right direction
                if (move > 0)
                {
                    _FacingRight = true;
                }
                else if (move < 0)
                {
                    _FacingRight = false;
                }

                // should we turn around?
                if (_previousFacingRight != _FacingRight)
                {
                    Flip();
                }
                if (_useTurnaround && _grounded)
                {
                    // turnaround timer
                    if (_isTurningAround)
                    {
                        if (!IsTurnaroundTimerExpired())
                        {
                            shouldMove = false;
                        }
                    }
                }
                if (!shouldMove)
                {
                    targetVelocity = new Vector2(0, _rigidbody.velocity.y);
                }
                // move the player
                _rigidbody.velocity = Vector3.SmoothDamp(_rigidbody.velocity, targetVelocity, ref _Velocity, _movementSmoothing);
            }

            // Jumping
            // should we start jumping?
            if (_grounded && jump && !_jumpKeyDownAlready && !_rooted)
            {
                // _grounded will still return true as you begin to jump!
                _jumping = true;
                _jumpTimeCounter = 0;
                //Debug.Log("Started Jump");
                _animator.SetBool("Jumping", true);

            }

            // currently jumping
            if (jump && _jumping)
            {
                if (_jumpTimeCounter < _maxJumpTime)
                {
                    //_currentlyFloating = false;
                    if (!IsHittingCeiling())
                    {
                        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, GetJumpYVelocity());
                    }
                    else
                    {
                        _rigidbody.velocity = new Vector2(_rigidbody.velocity.x, 0);
                        _jumping = false;
                    }
                    _jumpTimeCounter += Time.deltaTime;
                }
                else
                {
                    //Debug.Log("ENDING JUMP");
                    _jumping = false;
                }
                //Debug.Log("Y Velocity: " + _rigidbody.velocity.y.ToString());
            }
            
            /*
            // apply correct gravity scale
            if (IsFalling())
            {
                SetGravityScale(_fallingGravityScale);
            }
            else
            {
                SetGravityScale(_normalGravityScale);
            }
            */

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
                    if (_verticalInput > 0)
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
                        _animator.SetBool("Attacking_Up", true);

                    }
                }
                else if (absHorizontal >= absVertical)
                {
                    //SIDE ATTACK
                    _currentAttack = AttackType.Side;
                    _animator.SetBool("Attacking_Side", true);

                }


                //decide if attack is immediatly released or charged
                if (_currentAttack != AttackType.None)
                {
                    //can't charge Attack while in mid-air
                    if(!_grounded)
                    {
                        if (_attackWindupFinished)
                        {
                            ReleaseAttack();
                        }
                    }
                    else
                    {
                        //_chargingAttack = true;
                        //this.GetComponent<SpriteRenderer>().color = new Color(0, 1.0f, 0);
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
        _animator.SetBool("Attack_Charged", true);


        _chargingAttack = false;
        this.GetComponent<SpriteRenderer>().color = new Color(1.0f, 1.0f, 1.0f);


        switch (_currentAttack)
        {
            //collider specific changes
            case AttackType.Side:
                _sideAttackObject.GetComponent<SpriteRenderer>().enabled = true;
                //_sideAttackCollider.enabled = true;
                _sideAttack.SetLaunchAmount(_SideAttackMinLaunchSize + ((_attackChargeTimer / _SideAttackHoldDuration) * (_SideAttackMaxLaunchSize - _SideAttackMinLaunchSize)));
                _sideAttack.Activate();
                break;
            case AttackType.Up:
                _upAttackObject.GetComponent<SpriteRenderer>().enabled = true;
                //_upAttackCollider.enabled = true;
                _upAttack.SetLaunchAmount(_UpAttackMinLaunchSize + ((_attackChargeTimer / _UpAttackHoldDuration) * (_UpAttackMaxLaunchSize - _UpAttackMinLaunchSize)));
                _upAttack.Activate();
                break;
            case AttackType.Down:
                _downAttackObject.GetComponent<SpriteRenderer>().enabled = true;
                //_downAttackCollider.enabled = true;
                _downAttack.SetLaunchAmount(_DownAttackMinLaunchSize +  ((_attackChargeTimer / _DownAttackHoldDuration) * (_DownAttackMaxLaunchSize-_DownAttackMinLaunchSize)));
                _downAttack.Activate();
                break;
            case AttackType.None:
                break;
            default:
                break;
        }

        _source.PlayOneShot(_audioScriptableObject.MissingMeleeClip);
        _attackAnimPlaying = true;
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
        if (_chargingAttack && _attackWindupFinished && !_attackKeyDown)
        {
            //releasing attack
            //Debug.Log("attack tap");
            ReleaseAttack();
        }

        if (_chargingAttack && _attackWindupFinished)
        {
            //Debug.Log("CHARGING");
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
                        //Debug.Log("RELEASE UP");
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

        //backup Timer
        if (_attackAnimPlaying)
        {
            _attackResetBackupTimer += Time.deltaTime;


            switch (_currentAttack)
            {
                //collider specific changes
                case AttackType.Side:
                    if (_attackResetBackupTimer >= _sideAttackDuration +0.3f)
                    {
                        ResetAttack();
                    }
                    break;
                case AttackType.Up:
                    if (_attackResetBackupTimer >= _upAttackDuration + 0.3f)
                    {
                        ResetAttack();
                    }
                    break;
            }

        }


        if (_projectileFiring)
        {
            // is this startup on a projectile?
            _projectileFiringTimer += Time.deltaTime;
            if (_projectileFiringTimer >= _projectileStartupDuration)
            {
                //actually create the projectile
                Debug.Log("Creating the projectile");
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

    private void ResetAttack()
    {
        _attackResetBackupTimer = 0.0f;
        _attacking = false;
        _attackOnCooldown = true;
        _attackWindupFinished = false;
        _attackAnimPlaying = false;
        _animator.SetBool("Attacking_Side", false);
        _animator.SetBool("Attacking_Up", false);

        _animator.Play("Idle");
        //reset after attack finishes
        switch (_currentAttack)
        {
            //collider specific changes
            case AttackType.Side:
                _sideAttackObject.GetComponent<SpriteRenderer>().enabled = false;
                //_sideAttackCollider.enabled = false;
                _sideAttack.Activate(false);
                break;
            case AttackType.Up:
                _upAttackObject.GetComponent<SpriteRenderer>().enabled = false;
                //_upAttackCollider.enabled = false;
                _upAttack.Activate(false);
                break;
            case AttackType.Down:
                 _downAttackObject.GetComponent<SpriteRenderer>().enabled = false;
                //_downAttackCollider.enabled = false;
                _downAttack.Activate(false);
                break;
            case AttackType.None:
                break;
            default:
                break;
        }
        _currentAttack = AttackType.None;
    }

    private void Flip()
    {

        // Switch the way the player is labelled as facing.
        _isTurningAround = true;
        _turnaroundTimer = _turnaroundDuration;
        _previousFacingRight = _FacingRight;
        //_FacingRight = !_FacingRight;

        // Multiply the player's x local scale by -1.
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    private void HandlePlayerInput()
    {
        if(_PlayerState != PlayerState.Dead)
        {
            _horizontalInput = Input.GetAxisRaw("Horizontal" + _inputSuffix);
            _verticalInput = Input.GetAxisRaw("Vertical" + _inputSuffix);
            // TODO: is there a reason why these are all else ifs and not just else?
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
            }
            //if (Input.GetButtonDown("SpecialAttack" + _inputSuffix))
            //{
            //    _specialAttackKeyDown = true;
            //}
            //else
            //{
            //    _specialAttackKeyDown = false;
            //}
            if (Input.GetButtonDown("Pause" + _inputSuffix))
            {
                _GameManager.PauseGame(_ControllerID);
            }
        }
    }

    private void Stun(float duration)
    {
        _stunned = true;
        Invoke("DisableStun", duration);
        this.GetComponent<SpriteRenderer>().color = Color.red;
        _animator.SetBool("Stunned", true);
    }

    private void DisableStun()
    {
        _stunned = false;
        this.GetComponent<SpriteRenderer>().color = Color.white;
        _animator.SetBool("Stunned", false);
    }

    public void Die(CauseOfDeath cause)
    {
        _PlayerState = PlayerState.Dead;
        _AmountOfStocks--;
        _AmountOfDeaths++;
        _animator.SetBool("Die", true);
        _rigidbody.bodyType = RigidbodyType2D.Static;
        DisablePowerups();

        for (int i = 0; i < _deathSettings.Count; i++)
        {
            if(_deathSettings[i].Cause == cause && _deathSettings[i].Clip != null)
            {
                _source.PlayOneShot(_deathSettings[i].Clip);
            }
        }
    }

    private void ConfigureJump(float min_height, float min_time, float max_height)
    {
        // calculate values from supplied arguments
        float delta = 1.0f / 8;  // based on physics velocity update ticks but this might be wrong...
        float gravity = (2 * min_height) / (2 * min_time);
        float jumpVelocity = Mathf.Sqrt(2 * gravity * min_height);
        //float maxTime = Mathf.Sqrt((2 * max_height) / (gravity + jumpVelocity));
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
        // TODO: Change this to a raycast upwards?
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

                    PlayFootStepSound();
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
        // Currently collides more than once!!
        int playerLayerMask = LayerMask.GetMask("Player");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(_groundCheck.position, k_groundedRadius, playerLayerMask);
        //Debug.Log(colliders.Length);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject.tag == "Player")
            {
                //if (colliders[i].gameObject.GetComponent<CharacterController>()._PlayerTag != _PlayerTag)
                if (colliders[i].gameObject != this.gameObject)
                {
                    //Debug.Log("STOMP COLLIDED WITH PLAYER");
                    _rigidbody.AddForce(Vector2.up * _playerStompJumpHeight);
                    // get colliding player and stun them
                    colliders[i].GetComponent<CharacterController>().RecieveHit(
                            Vector2.up * _playerStompKnockbackHeight, _playerStompStunDuration);
                    // put stomp on a cooldown to prevent this triggering again next tick
                    _playerStompCooldownTimer = _playerStompCooldownDuration;
                    _playerStompOnCooldown = true;
                }
            }
        }
    }

    public void WindupAnimationFinished()
    {
        //Debug.Log("WINDUP FINISHED");
        _attackWindupFinished = true;

        if(_attackKeyDown)
        {
            _chargingAttack = true;
            this.GetComponent<SpriteRenderer>().color = new Color(0, 1.0f, 0);
        }
        else
        {
            //Debug.Log("attack tap");
            ReleaseAttack();
        }
    }

    public void FootStepAnimation()
    {
        if (_grounded)
        {
            PlayFootStepSound();
        }
    }

    private void PlayFootStepSound()
    {
        _source.PlayOneShot(_audioScriptableObject.RunningClips[Random.Range(0, _audioScriptableObject.RunningClips.Length)]);
    }

    public void AttackAnimationFinished()
    {
        ResetAttack();
    }

    public void DeathAnimationFinished()
    {
        //Debug.Log("Death Anim finished");


        PlayerData data;
        data.Id = _PlayerID;
        data.Stocks = _AmountOfStocks;
        data.TeamId = _TeamID;
        data.Deaths = _AmountOfDeaths;
        data.charID = _CharID;
        data.controllerID = _ControllerID;
        data.skinID = _SkinID;

        if (_PlayerTag)
        {
            Destroy(_PlayerTag.gameObject);
        }

        _PlayerUI.UpdateStockText(_AmountOfStocks);
        _GameManager.PlayerDeath(this);

        if (_AmountOfStocks > 0 || _GameManager._WinCondition != GameWinCondition.STOCKS)
        {
            RespawnPoint point = _GameManager.FindBestRespawnPoint(_PlayerID);

            point.Activate(data);
        }
        Destroy(gameObject);
    }

    public void DisablePowerups()
    {
        // disable all powerup effects
        // much simpler solution than keeping track of what effect was active
        // hopefully this won't ever cause an issue
        _controlsReversed = false;
        _moveSpeedMultiplier = 1;
        _rooted = false;
        _shielded = false;
        _meleeInstantKill = false;
        _GameManager.OnPowerupExpired(_PlayerID);
        // set colour back to default
        this.GetComponent<SpriteRenderer>().color = Color.white;
    }

    // rename this?
    public void RecieveHit(Vector2 knockbackVelocity, float stunDuration, bool meleeHit = false)
    {
        //Debug.Log("HIT RECIEVED");
        if (_shielded)  // block hit if shield active
        {
            _shielded = false;

            //play sound for shielded??
        }
        else  // recieve hit as normal
        {
            if (meleeHit && _meleeInstantKill)
            {
                Die(CauseOfDeath.Melee);
            }
            else
            {
                //col.GetComponent<Rigidbody2D>().AddForceAtPosition(launchVector*_launchAmount,col.transform.position);
                AddKnockback(knockbackVelocity);
                Stun(stunDuration);

                //add ui knockback
            }
            _source.PlayOneShot(_audioScriptableObject.GettingHitClip);
        }
    }

    public void AddKnockback(Vector2 knockbackVelocity)
    {
        //Debug.Log("INFLICTING KNOCKBACK");
        _rigidbody.AddForceAtPosition(knockbackVelocity, transform.position);
    }

    public void OnPowerupCollected(float effectTime, Sprite powerupHUDSprite, Color modulateColor,
            string powerupName, Vector3 powerupPosition, bool isFirstTarget)
    {
        Invoke("DisablePowerups", effectTime);
        this.GetComponent<SpriteRenderer>().color = modulateColor;
        if (isFirstTarget)
        {
            _GameManager.OnPowerupCollected(_TeamID, powerupHUDSprite, powerupName, powerupPosition);
        }
    }

    // this needs to be a timer in FixedUpdate() rather than a coroutine/invoke as the timer can be reset back when another turnaround is triggered
    private bool IsTurnaroundTimerExpired()
    {
        _turnaroundTimer -= Time.deltaTime;
        //Debug.Log("_turnaroundTimer:" + _turnaroundTimer.ToString());
        if (_turnaroundTimer <= 0)
        {
            _isTurningAround = false;
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsFalling()
    {
        return _rigidbody.velocity.y < 0;
    }

    private float GetJumpYVelocity()
    {
        float timerPercent = _jumpTimeCounter / _maxJumpTime;
        float jumpYVelocity = Mathf.Lerp(_jumpVelocity, 0, timerPercent);
        return jumpYVelocity;
    }

    /*
    private float SmoothStop(float x, float power)
    {
        return 1 - Mathf.Pow((1 - x), power);
    }
    */

    // TODO: just change these to public members
    public float[] GetCameraShakeValues()
    {
        float[] cameraValues = new float[4];
        cameraValues[0] = _cameraShakeIntensityMelee;
        cameraValues[1] = _cameraShakeDurationMelee;
        cameraValues[2] = _cameraShakeIntensityProjectile;
        cameraValues[3] = _cameraShakeDurationProjectile;
        return cameraValues;
    }
}


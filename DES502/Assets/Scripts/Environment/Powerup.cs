using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum POWERUP_TYPES
    {
        TEST,
        REVERSE_CONTROLS,
        MOVE_SPEED
    }

    [Header("General")]
    [SerializeField] [Tooltip("What type of powerup is this?")]
    public POWERUP_TYPES _type;
    [Range(0.0f, 10.0f)] [SerializeField] [Tooltip("How long (in seconds) should the powerup effect last for?")]
    public float _effectTime = 5.0f;

    [Header("Powerup-specific")]
    [Range(0.0f, 5.0f)] [SerializeField] [Tooltip("Speed multiplier to use for speed powerup.")]
    public float _moveSpeedMultiplier = 2.5f;

    [Header("Sprite Animation")]
    [Range(0.0f, 0.5f)] [SerializeField] [Tooltip("How far (in units) should the sprite move from it's origin point?")]
    public float _maxYMovement = 0.2f;
    [Range(0.0f, 3.0f)] [SerializeField] [Tooltip("How fast should the sprite move?")]
    public float _animationSpeed = 1.5f;

    private GameObject _spriteObject;
    private float _animationTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        _spriteObject = transform.GetChild(0).gameObject;
        Debug.Log(_spriteObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateSpritePosition();
    }

    void UpdateSpritePosition()
    {
        _animationTimer += Time.deltaTime * _animationSpeed;
        /*
        if (_animationTimer > 10)
        {
            _animationTimer -= 10;
        }
        */
        float newYPos = Mathf.Sin(_animationTimer) * _maxYMovement;
        //Debug.Log(newYPos);
        _spriteObject.transform.localPosition = new Vector2(0, newYPos);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Destroy powerup if player collides with it
        //Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "Player")
        {
            ActivateEffect(other.gameObject.GetComponent<CharacterController>());
            Destroy(gameObject);
        }
    }

    void ActivateEffect(CharacterController player)
    {
        switch (_type)
        {
            case (POWERUP_TYPES.TEST):
                break;
            case (POWERUP_TYPES.REVERSE_CONTROLS):
                // TODO: change to activate on the enemy team
                player._controlsReversed = true;
                break;
            case (POWERUP_TYPES.MOVE_SPEED):
                player._moveSpeedMultiplier = _moveSpeedMultiplier;
                break;
        }
        player.StartPowerupTimer(_effectTime);
    }
}

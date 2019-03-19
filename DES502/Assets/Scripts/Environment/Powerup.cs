﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum POWERUP_TYPES
    {
        REVERSE_CONTROLS,
        MOVE_SPEED,
        ROOT,
        SHIELD,
        MELEE_INSTANT_KILL,
        RANDOM  // Needs to be last for the random powerup to work
    }

    // int is converted to bool
    // bool "target friendly"
    // POWERUP_TARGETS must remain in the same order as POWERUP_TYPES
    // RANDOM is unnecersary here
    private static bool[] POWERUP_TARGETS = {
        false,
        true,
        false,
        true,
        false};  // simpler to target the taker rather than the giver

    /*
    private enum POWERUP_TARGETS
    {
        REVERSE_CONTROLS = 0,
        MOVE_SPEED = 1,
        ROOT = 0,
        SHIELD = 1,
        MELEE_INSTANT_KILL = 0  // simpler to target the taker rather than the giver
    }
    */

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
        //Debug.Log(_spriteObject.name);
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
            ActivatePowerup(other.gameObject.GetComponent<CharacterController>());
        }
    }

    private void ActivatePowerup(CharacterController player)
    {
        Debug.Log("powerup type: " + _type);
        if (_type == POWERUP_TYPES.RANDOM)
        {
            // Activate a random powerup effect that isn't this one
            // get the amount of powerup types minus random
            // for this to work, RANDOM must remain the last index of POWERUP_TYPES
            //Debug.Log("RANDOM POWERUP COLLECTED")
            Random.InitState(System.Environment.TickCount);
            int powerupTypes = System.Enum.GetValues(typeof(POWERUP_TYPES)).Length - 1;
            //Debug.Log("powerupTypes: " + powerupTypes);
            POWERUP_TYPES powerupToActivate = (POWERUP_TYPES)Random.Range(0, powerupTypes);
            Debug.Log("random powerupToActivate: " + powerupToActivate);
            GetPowerupTargets(player, powerupToActivate);
        }
        else
        {
            GetPowerupTargets(player, _type);
        }
        Destroy(gameObject);
    }

    private void ApplyEffect(CharacterController player, POWERUP_TYPES powerup)
    {
        switch (powerup)
        {
            case (POWERUP_TYPES.REVERSE_CONTROLS):
                // TODO: change to activate on the enemy team
                player._controlsReversed = true;
                // apply visual effect
                break;
            case (POWERUP_TYPES.MOVE_SPEED):
                // TODO: change to activate on the friendly team
                player._moveSpeedMultiplier = _moveSpeedMultiplier;
                // apply visual effect
                break;
            case (POWERUP_TYPES.ROOT):
                // TODO: change to activate on the enemy team
                player._rooted = true;
                break;
            case (POWERUP_TYPES.SHIELD):
                // TODO: change to activate on the friendly team
                player._shielded = true;
                break;
            case (POWERUP_TYPES.MELEE_INSTANT_KILL):
                // TODO: change to activate on the enemy team
                player._meleeInstantKill = true;
                break;
            // RANDOM is no longer called in the effect stage
            /*
            case (POWERUP_TYPES.RANDOM):
                // Activate a random powerup effect that isn't this one
                // get the amount of powerup types minus random
                // for this to work, RANDOM must remain the last index of POWERUP_TYPES
                int powerupTypes = System.Enum.GetValues(typeof(POWERUP_TYPES)).Length - 1;
                //Debug.Log("powerupTypes: " + powerupTypes);
                POWERUP_TYPES powerupToActivate = (POWERUP_TYPES)Random.Range(0, powerupTypes);
                //Debug.Log("powerupToActivate: " + powerupToActivate);
                ApplyEffect(player, powerupToActivate);
                break;
            */
        }
        player.StartPowerupTimer(_effectTime);
    }

    private void GetPowerupTargets(CharacterController player, POWERUP_TYPES powerup)
    {
        List<CharacterController> totalPlayers = GameObject.Find("GameManager").GetComponent<GameManager>()._characterControllers;
        int playerTeamIndex = GetTeamIndex(player);
        foreach(CharacterController p in totalPlayers)
        {
            int pTeamIndex = GetTeamIndex(p);
            int powerupTypeIndex = System.Array.IndexOf(POWERUP_TYPES.GetValues(_type.GetType()), powerup);
            //Debug.Log("powerupTypeIndex: " + powerupTypeIndex);
            bool powerupTargetFriendly = POWERUP_TARGETS[powerupTypeIndex];
            if ((playerTeamIndex == pTeamIndex && powerupTargetFriendly)
                    || (playerTeamIndex != pTeamIndex && !powerupTargetFriendly))
            {
                // do effect here!!
                ApplyEffect(p, powerup);
            }
        }
    }

    private int GetTeamIndex(CharacterController player)
    {
        return (int)player._TeamID;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    private CharacterController _controller;

    enum PlayerID { Player1, Player2, Player3, Player4 };
    public enum PlayerState {Idle, Jumping, Running, Dead };


    [SerializeField]
    PlayerID _PlayerID;

    [SerializeField]
    [Range(0, 150.0f)] float _moveSpeed;
    [SerializeField]
    [Range(0, 1400.0f)] float _jumpForce = 400f;


    //public
    [HideInInspector]
    public PlayerState _PlayerState = PlayerState.Idle;


    //private

    float _horizontalMove =0.0f;
    float _verticalMove = 0.0f;
    bool _jumpKeyDown = false;
    string _inputSuffix;

    private void Awake()
    {
        _controller = this.GetComponent<CharacterController>();

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
    }

    // Use this for initialization
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update () {

    }

    private void FixedUpdate()
    {
        HandlePlayerInput();

        _controller.Move(_horizontalMove, _jumpKeyDown);
    }

    private void HandlePlayerInput()
    {

        _horizontalMove = Input.GetAxisRaw("Horizontal"+ _inputSuffix) * Time.fixedDeltaTime * _moveSpeed;

        if (Input.GetButtonDown("Jump"+_inputSuffix))
        {
            _jumpKeyDown = true;
        }
        else if (Input.GetButtonUp("Jump" + _inputSuffix))
        {
            _jumpKeyDown = false;
        }
    }

  
}

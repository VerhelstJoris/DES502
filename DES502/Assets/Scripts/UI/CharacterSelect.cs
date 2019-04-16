using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    [SerializeField]
    private Text _text;

    [SerializeField]
    private Text _playerNametext;
    [SerializeField]
    private Text _subtext;

    [SerializeField]
    private Sprite[] _rabbitImages;
    [SerializeField]
    private Sprite[] _foxImages;

    [SerializeField]
    private Image _characterImageDisplay;

    MainMenu _mainMenu;

    bool _playerJoined = false;
    bool _characterSelected = false;
    bool _rabbitImageSelected = true;
    bool _nextSelected = false;
    bool _ready = false;
    string _inputSuffix;
    int _currenSkinSelected = 0;

    //DATA TO PASS on
    [SerializeField]
    private ControllerID _controllerID;
    CharacterID _charID;

    public PlayerDataScriptableObject _Data;

    // Start is called before the first frame update
    void Start()
    {
        //proper input
        switch (_controllerID)
        {
            case ControllerID.Controller1:
                _inputSuffix = "_P1";
                _playerNametext.text = "Player 1";
                break;
            case ControllerID.Controller2:
                _inputSuffix = "_P2";
                _playerNametext.text = "Player 2";
                break;
            case ControllerID.Controller3:
                _inputSuffix = "_P3";
                _playerNametext.text = "Player 3";
                break;
            case ControllerID.Controller4:
                _inputSuffix = "_P4";
                _playerNametext.text = "Player 4";
                break;
            default:
                break;
        }


        _playerNametext.enabled = false;
        _characterImageDisplay.enabled = false;
        _subtext.enabled = false;

        _Data.Active = false;
        _Data.SkinID = 0;
        _Data.CharacterID = CharacterID.Rabbit;
        _mainMenu = FindObjectOfType<MainMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        HandlePlayerInput();
    }

    private void HandlePlayerInput()
    {
        
        if (Input.GetButtonDown("Submit" + _inputSuffix))
        {
            //JOINING
            if (!_playerJoined)
            {
                _playerJoined = true;
                //Debug.Log(_inputSuffix + " PlayerJoined");
                _text.enabled = false;
                _playerNametext.enabled = true;
                _subtext.enabled = true;
                _characterImageDisplay.enabled = true;
                _characterImageDisplay.sprite = _rabbitImages[0];

                _mainMenu.AddPlayer();
            }

            //SELECTED CHARACTER
            else if (_playerJoined && !_characterSelected)
            {
                //Debug.Log(_inputSuffix + " Character selected");

                _subtext.text = "Select a Character Skin";

                if (_rabbitImageSelected)
                {
                    _charID = CharacterID.Rabbit;
                    _characterImageDisplay.sprite = _rabbitImages[0];
                }
                else
                {
                    _charID = CharacterID.Fox;
                    _characterImageDisplay.sprite = _foxImages[0];
                }

                _characterSelected = true;
            }

            //SELECTED A SKIN
            else if (_playerJoined && _characterSelected)
            {
                _characterImageDisplay.enabled = false;
                _text.text = "READY";
                _text.enabled = true;
                _subtext.enabled = false;
                _ready = true;


                
                _Data.CharacterID = _charID;
                _Data.ControllerID = _controllerID;
                _Data.PlayerID = _mainMenu.GetNextPlayerID();
                _Data.SkinID = _currenSkinSelected;
                _Data.Active = true;

                Debug.Log("CHARACTER READIED UP");
                Debug.Log("CONTROLLER ID: " + _Data.ControllerID.ToString()  + " PLAYER ID: " + _Data.PlayerID + " CHARACTER ID: " + _Data.CharacterID.ToString() + " SKIN ID: " + _Data.SkinID);

                _mainMenu.PlayerReady();

            }

        }


        //SELECTING LEFT- RIGHT
        //=====================================================================

        float _horizontalInput = Input.GetAxisRaw("Horizontal" + _inputSuffix);

        if(_horizontalInput==0.0f)
        {
            _nextSelected = false;
        }

        //CHARACTERS
        if (_playerJoined && !_nextSelected && !_characterSelected)
        {
            if (_horizontalInput >= 0.5f || _horizontalInput <= -0.5f)
            {
                _nextSelected = true;

                if (_rabbitImageSelected)
                {
                    _characterImageDisplay.sprite = _foxImages[0];
                    _rabbitImageSelected = false;
                }
                else
                {
                    _characterImageDisplay.sprite = _rabbitImages[0];
                    _rabbitImageSelected = true;
                }
            }

            //Debug.Log("Selected next character");
        }

        //CHARACTER SKINS
        else if (_playerJoined && !_nextSelected && _characterSelected)
        {

            if (_horizontalInput >= 0.5f)
            {
                _nextSelected = true;

                if (_rabbitImageSelected)
                {
                    _currenSkinSelected = (_currenSkinSelected + 1) % _rabbitImages.Length;
                    _characterImageDisplay.sprite = _rabbitImages[_currenSkinSelected];
                }
                else
                {
                    _currenSkinSelected = (_currenSkinSelected + 1) % _foxImages.Length;
                    _characterImageDisplay.sprite = _foxImages[_currenSkinSelected];
                }

            }
            else if (_horizontalInput <= -0.5f)
            {
                _nextSelected = true;

                _currenSkinSelected--;



                if (_rabbitImageSelected)
                {
                    if (_currenSkinSelected <= 0)
                    {
                        _currenSkinSelected = _rabbitImages.Length-1;
                    }
                    _characterImageDisplay.sprite = _rabbitImages[_currenSkinSelected];
                }
                else
                {
                    if (_currenSkinSelected <= 0)
                    {
                        _currenSkinSelected = _foxImages.Length-1;
                    }
                    _characterImageDisplay.sprite = _foxImages[_currenSkinSelected];
                }
            }

        }
       

    }
}

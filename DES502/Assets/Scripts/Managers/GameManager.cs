using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public GameModeScriptableObject _GMScriptableObject;

    private RespawnPoint[] _respawnPoints;
    private CharacterController[] _playerCharacters;
    private PlayerUI[] _playerUIs;
    private Canvas _canvas;

    private int _playerAmount = 2;
    private TeamSetup _teamSetup;
    private GameWinCondition _winCondition;


    private float _gameTimerLeft;
    private int _startingStocksPerPlayer;

    private void Awake()
    {
        //GM will NOT be created if not in scene
        //GM removes scene duplicates
        //GM has global access
        //GM NOT Kept across scene loads
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else 
        {
            _instance = this;
        }

        _teamSetup = _GMScriptableObject.TeamSetup;
        _winCondition = _GMScriptableObject.GameWinCondition;
        _playerAmount = _GMScriptableObject.PlayerAmount;


        //stocks of timer
        if (_winCondition == GameWinCondition.STOCKS)
        {
            _startingStocksPerPlayer = _GMScriptableObject.AmountOfStocks;
        }
        else if(_winCondition == GameWinCondition.TIME)
        {
            _gameTimerLeft = _GMScriptableObject.AmountOfTime;
        }


        Debug.Log(_teamSetup.ToString());
        Debug.Log(_winCondition.ToString());
        Debug.Log(_playerAmount);
    }

    void Start()
    {
        _respawnPoints = FindObjectsOfType<RespawnPoint>();
        _playerCharacters = FindObjectsOfType<CharacterController>();
        _playerUIs = FindObjectsOfType<PlayerUI>();
        _canvas = FindObjectOfType<Canvas>();

        //destroy all playercharacters and UI's present in the scene
        for (int i = 0; i < _playerCharacters.Length; i++)
        {
            Destroy(_playerCharacters[i].gameObject);
            _playerCharacters[i] = null;
        }

        for (int i = 0; i < _playerUIs.Length; i++)
        {
            Destroy(_playerUIs[i].gameObject);
            _playerUIs[i] = null;
        }


        
        for (int i = 0; i < _respawnPoints.Length; i++)
        {
            _respawnPoints[i]._ActiveTimeBeforeRespawn = CharacterController.RespawnDuration;
        }

        //spawn in the players
        for (int i = 0; i < _playerAmount; i++)
        {
            PlayerData data;
            data.Id = (PlayerID)i;
            data.Stocks = _startingStocksPerPlayer;

            _respawnPoints[i].Activate(data);
        }
    }

    public void AddPlayer(CharacterController player)
    {
        for (int i = 0; i < _playerCharacters.Length; i++)
        {
            if(_playerCharacters[i]._PlayerID == player._PlayerID)
            {
                _playerCharacters[i] = player;
                _playerCharacters[i]._GameManager = this;
                break;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_winCondition==GameWinCondition.TIME)
        {
            Mathf.Clamp(_gameTimerLeft -= Time.deltaTime,0.0f,1000000);

            if(_gameTimerLeft <= 0.0f)
            {

            }
        }
    }

    public RespawnPoint FindBestRespawnPoint(PlayerID playerID)
    {
        int respawnID=0;
        float longestDistanceAll = 0.0f;


        for (int i = 0; i < _respawnPoints.Length; i++)
        {
            float closestPlayerDistance = float.MaxValue ;

            if (!_respawnPoints[i]._Active)
            {

                for (int j = 0; j < _playerCharacters.Length; j++)
                {

                    if (_playerCharacters[j]._PlayerState != PlayerState.Dead && !_respawnPoints[i]._Active)
                    {
                        float distance = Vector3.Distance(_respawnPoints[i].transform.position, _playerCharacters[j].transform.position);

                        if (distance < closestPlayerDistance)
                        {
                            closestPlayerDistance = distance;
                        }
                    }

                    //after the last one
                    if (j == _playerCharacters.Length - 1)
                    {
                        if (closestPlayerDistance > longestDistanceAll)
                        {
                            respawnID = i;
                            longestDistanceAll = closestPlayerDistance;
                        }
                    }
                }
            }

        }

        return _respawnPoints[respawnID];

    }

    public void CreatePlayerUI(CharacterController character)
    {

        if (_playerUIs.Length != 0)
        {
            for (int i = 0; i < _playerUIs.Length; i++)
            {

                if (_playerUIs[i] != null && _playerUIs[i]._PlayerID == character._PlayerID)
                {
                    character._PlayerUI = _playerUIs[i];
                    return;
                }
            }
        }

        var ui = ObjectFactory.CreatePlayerUI(character, _playerAmount, _winCondition);
        ui.transform.SetParent(_canvas.transform,false);
    }

}

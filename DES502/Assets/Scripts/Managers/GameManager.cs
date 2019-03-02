﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public GameModeScriptableObject _GMScriptableObject;

    private RespawnPoint[] _respawnPoints;
    //private CharacterController[] _playerCharacters = new CharacterController[4];
    private List<CharacterController> _characterControllers = new List<CharacterController>();
    private PlayerUI[] _playerUIs;
    private Canvas _canvas;

    [HideInInspector]
    public int _PlayerAmount = 2;
    [HideInInspector]
    public TeamSetup _TeamSetup;
    [HideInInspector]
    public GameWinCondition _WinCondition;

    public float _GameTimerLeft=0.0f;
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

        _TeamSetup = _GMScriptableObject.TeamSetup;
        _WinCondition = _GMScriptableObject.GameWinCondition;
        _PlayerAmount = _GMScriptableObject.PlayerAmount;


        //stocks of timer
        if (_WinCondition == GameWinCondition.STOCKS)
        {
            _startingStocksPerPlayer = _GMScriptableObject.AmountOfStocks;
        }
        else if(_WinCondition == GameWinCondition.TIME)
        {
            _GameTimerLeft = _GMScriptableObject.AmountOfTime;
        }


        Debug.Log(_TeamSetup.ToString());
        Debug.Log(_WinCondition.ToString());
        Debug.Log("PLAYERAMOUNT: " + _PlayerAmount);
    }

    void Start()
    {
        _respawnPoints = FindObjectsOfType<RespawnPoint>();

        _playerUIs = FindObjectsOfType<PlayerUI>();
        _canvas = FindObjectOfType<Canvas>();

        //_playerCharacters = FindObjectsOfType<CharacterController>();

        ////destroy all playercharacters and UI's present in the scene
        //for (int i = 0; i < _playerCharacters.Length; i++)
        //{
        //    if (_playerCharacters[i] != null)
        //    {
        //        Destroy(_playerCharacters[i].gameObject);
        //        _playerCharacters[i] = null;
        //    }
        //}

        for (int i = 0; i < _playerUIs.Length; i++)
        {
            Destroy(_playerUIs[i].gameObject);
            _playerUIs[i] = null;
        }

        //create a canvas if necessary
        if (_canvas == null)
        {
            GameObject g = new GameObject();
            _canvas = g.AddComponent<Canvas>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        //create TIMER UI
        if(_WinCondition==GameWinCondition.TIME)
        {
            var timer = ObjectFactory.CreateTimerUI(this, _GameTimerLeft);
            timer.transform.SetParent(_canvas.transform, false);

        }

        for (int i = 0; i < _respawnPoints.Length; i++)
        {
            _respawnPoints[i]._ActiveTimeBeforeRespawn = CharacterController.RespawnDuration;
        }

        //spawn in the players
        for (int i = 0; i < _PlayerAmount; i++)
        {
            PlayerData data;
            data.Id = (PlayerID)i;
            data.Stocks = _startingStocksPerPlayer;
            data.TeamId = (TeamID)(i % 2);
            _respawnPoints[i].Activate(data);
        }
    }

    public void AddPlayer(CharacterController player)
    {
        bool playerFound = false;

        player._GameManager = this;


        for (int i = 0; i < _characterControllers.Count; i++)
        {
            if(_characterControllers[i]._PlayerID == player._PlayerID)
            {
                _characterControllers[i] = player;
                playerFound = true;
                break;
            }
        }

        if(!playerFound)
        {
            _characterControllers.Add(player);
        }

        Debug.Log("Player Added: " + player._PlayerID.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        if(_WinCondition==GameWinCondition.TIME)
        {
            Mathf.Clamp(_GameTimerLeft -= Time.deltaTime,0.0f,1000000.0f);

            if(_GameTimerLeft <= 0.0f)
            {
                EndGame();
            }
        }
    }

    public RespawnPoint FindBestRespawnPoint(PlayerID playerID)
    {
        int respawnID=0;
        float longestDistanceAll = 0.0f;

        //Debug.Log("Amount of respawnpoints: " + _respawnPoints.Length);
        //Debug.Log("Amount of players: " + _characterControllers.Count);


        for (int i = 0; i < _respawnPoints.Length; i++)
        {
            float closestPlayerDistance = float.MaxValue ;

            if (!_respawnPoints[i]._Active)
            {

                for (int j = 0; j < _characterControllers.Count; j++)
                {

                    if (_characterControllers[j]._PlayerState != PlayerState.Dead && !_respawnPoints[i]._Active)
                    {
                        float distance = Vector3.Distance(_respawnPoints[i].transform.position, _characterControllers[j].transform.position);

                        if (distance < closestPlayerDistance)
                        {
                            closestPlayerDistance = distance;
                        }
                    }

                    //after the last one
                    if (j == _characterControllers.Count - 1)
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

        var ui = ObjectFactory.CreatePlayerUI(character, _PlayerAmount, _WinCondition, _TeamSetup);
        ui.transform.SetParent(_canvas.transform,false);
    }

    private void EndGame()
    {
        Debug.Log("GAME ENDED");
    }



}

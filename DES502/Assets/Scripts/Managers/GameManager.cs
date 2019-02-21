﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private RespawnPoint[] _respawnPoints;
    private CharacterController[] _playerCharacters;

    public int PlayersToSpawn = 2;

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
    }

    //private void OnDestroy()
    //{
    //    if (this == _instance)
    //    {
    //        _instance = null;
    //    }
    //}

    void Start()
    {
        _respawnPoints = FindObjectsOfType<RespawnPoint>();
        _playerCharacters = FindObjectsOfType<CharacterController>();

        for (int i = 0; i < _respawnPoints.Length; i++)
        {
            _respawnPoints[i]._ActiveTimeBeforeRespawn = CharacterController.RespawnDuration;
        }

        //spawn in the players
        for (int i = 0; i < PlayersToSpawn; i++)
        {
            _respawnPoints[i].Activate( (PlayerID)i );
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


}

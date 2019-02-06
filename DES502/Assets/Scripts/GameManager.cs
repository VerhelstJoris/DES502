using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    private RespawnPoint[] _respawnPoints;
    private CharacterController[] _playerCharacters;

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

        for (int i = 0; i < _playerCharacters.Length ; i++)
        {
            _playerCharacters[i]._GameManager = this;
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

            for (int j = 0; j < _playerCharacters.Length; j++)
            {

                if(_playerCharacters[j]._PlayerState!=PlayerState.Dead)
                {
                    float distance = Vector3.Distance(_respawnPoints[i].transform.position, _playerCharacters[j].transform.position);

                    if (distance < closestPlayerDistance)
                    {
                        closestPlayerDistance = distance;
                    }
                }


                //after the last one
                if(j==_playerCharacters.Length-1)
                {
                    if(closestPlayerDistance > longestDistanceAll)
                    {
                        respawnID = i;
                        longestDistanceAll = closestPlayerDistance;
                    }
                }
            }

        }

        return _respawnPoints[respawnID];

    }


}

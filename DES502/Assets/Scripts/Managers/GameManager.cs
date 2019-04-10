using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    public GameModeScriptableObject _GMScriptableObject;

    private RespawnPoint[] _respawnPoints;
    public List<CharacterController> _characterControllers = new List<CharacterController>();

    //UI
    private List<PlayerUI> _playerUIs = new List<PlayerUI>();
    private Canvas _canvas;
    private GameEndMenu _gameEndPanel;
    private TimerUI _timerUI;
    private EventSystem _eventSystem;

    //GameMode stuff
    [HideInInspector]
    public int _PlayerAmount = 2;
    [HideInInspector]
    public TeamSetup _TeamSetup;
    [HideInInspector]
    public GameWinCondition _WinCondition;

    //PLAYERDATAS
    public PlayerDataScriptableObject[] _PlayerDataObjects;

    public float _GameTimerLeft=0.0f;

    //stocks
    private int _startingStocksPerPlayer;
    private int _playersWithStocksLeft;

    //time
    private int _team1Deaths=0, _team2Deaths=0;

    // powerup spawning
    [Header("Powerup Spawning")]
    public GameObject _powerupPrefab;
    private PowerupSpawnPoint[] _powerupSpawnPoints;
    [Range(0, 30)]
    public int _minPowerupSpawnCooldown = 8;
    [Range(0, 30)]
    public int _maxPowerupSpawnCooldown = 15;
    private float _powerupSpawnTimer;

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
        _playersWithStocksLeft = _PlayerAmount;

        //stocks of timer
        if (_WinCondition == GameWinCondition.STOCKS)
        {
            _startingStocksPerPlayer = _GMScriptableObject.AmountOfStocks;
        }
        else if(_WinCondition == GameWinCondition.TIME)
        {
            _GameTimerLeft = _GMScriptableObject.AmountOfTime;
        }


       //Debug.Log(_TeamSetup.ToString());
       //Debug.Log(_WinCondition.ToString());
       //Debug.Log("PLAYERAMOUNT: " + _PlayerAmount);

    }

    void Start()
    {
        Random.seed = System.Environment.TickCount;
        _respawnPoints = FindObjectsOfType<RespawnPoint>();
        _powerupSpawnPoints = FindObjectsOfType<PowerupSpawnPoint>();

        var playerUIsInScene = FindObjectsOfType<PlayerUI>();
        _canvas = FindObjectOfType<Canvas>();
        _gameEndPanel = FindObjectOfType<GameEndMenu>();
        if(_gameEndPanel)
        {
            _gameEndPanel.gameObject.SetActive(false);
        }

        _timerUI = FindObjectOfType<TimerUI>();
        if (_timerUI && _WinCondition != GameWinCondition.TIME)
        {
            _timerUI.gameObject.SetActive(false);
        }
        else
        {
            _timerUI.Initialize(this, _GameTimerLeft);
        }


        _eventSystem = FindObjectOfType<EventSystem>();

        var playersInScene = FindObjectsOfType<CharacterController>();

        //destroy all playercharacters and UI's present in the scene
        for (int i = 0; i < playersInScene.Length; i++)
        {
            if (playersInScene[i] != null)
            {
                Destroy(playersInScene[i].gameObject);
                playersInScene[i] = null;
            }
        }

        for (int i = 0; i < playerUIsInScene.Length; i++)
        {
            Destroy(playerUIsInScene[i].gameObject);
            playerUIsInScene[i] = null;
        }

        //create a canvas if necessary
        if (_canvas == null)
        {
            GameObject g = new GameObject();
            g.name = "Canvas";
            _canvas = g.AddComponent<Canvas>();
            g.AddComponent<CanvasScaler>();
            g.AddComponent<GraphicRaycaster>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            g.AddComponent<LoadOnClick>();
        }

        //create end game panel
        if(_gameEndPanel == null)
        {
            _gameEndPanel =  ObjectFactory.CreateGameEndMenu(this, _canvas);
            _gameEndPanel.transform.SetParent(_canvas.transform, false);
            _gameEndPanel.gameObject.SetActive(false);
        }

        //create TIMER UI
        if(_WinCondition==GameWinCondition.TIME && _timerUI==null)
        {
            var timer = ObjectFactory.CreateTimerUI(this, _GameTimerLeft);
            timer.transform.SetParent(_canvas.transform, false);

        }
        
        //set respawnTime
        for (int i = 0; i < _respawnPoints.Length; i++)
        {
            _respawnPoints[i]._ActiveTimeBeforeRespawn = CharacterController.RespawnDuration;
            _respawnPoints[i]._GM = this;
        }

        //spawn in the players
        //for (int i = 0; i < _PlayerAmount; i++)
        //{
        //    PlayerData data;
        //    data.Id = (PlayerID)i;
        //    data.Stocks = _startingStocksPerPlayer;
        //    data.TeamId = (TeamID)(i % 2);
        //    data.Deaths = 0;
        //    data.charID = (CharacterID)(i % 2);
        //    data.controllerID = ControllerID.Controller1;
        //    _respawnPoints[i].Activate(data);
        //}

        for (int i = 0; i < _PlayerDataObjects.Length; i++)
        {
            if(_PlayerDataObjects[i].Active)
            {
                PlayerData data;
                data.Id = _PlayerDataObjects[i].PlayerID;
                data.controllerID = _PlayerDataObjects[i].ControllerID;
                data.charID = _PlayerDataObjects[i].CharacterID;
                data.Stocks = _startingStocksPerPlayer;
                data.TeamId = (TeamID)(i % 2);
                data.Deaths = 0;
                data.skinID = _PlayerDataObjects[i].SkinID;

                _respawnPoints[i].Activate(data);
                Debug.Log("Spawn Player");
            }
        }
        ResetPowerupSpawnTimer();
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

    }

    public void PlayerDeath(CharacterController player)
    {
        //END GAME?
        if (_WinCondition == GameWinCondition.STOCKS)
        {
          
            if (player._AmountOfStocks == 0)
            {
                
                _playersWithStocksLeft--;
                //only 1 player with lives left
                if (_playersWithStocksLeft <= 1)
                {
                    EndGame();
                }
                
            }

        }
        else if (_WinCondition == GameWinCondition.TIME && _TeamSetup == TeamSetup.TEAM)
        {
            if(player._TeamID == TeamID.Team1)
            {
                _team1Deaths++;
            }
            else
            {
                _team2Deaths++;
            }
        }
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
        PowerupSpawnTimerTick();
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

        if (_playerUIs.Count != 0)
        {
            for (int i = 0; i < _playerUIs.Count; i++)
            {

                if (_playerUIs[i] != null && _playerUIs[i]._PlayerID == character._PlayerID)
                {
                    character._PlayerUI = _playerUIs[i];
                    return;
                }
            }
        }


        var ui = ObjectFactory.CreatePlayerUI(character, _PlayerAmount, _WinCondition, _TeamSetup, character._CharID );
        ui.transform.SetParent(_canvas.transform,false);
        _playerUIs.Add(ui);
    }

    private void EndGame()
    {
        Debug.Log("GAME ENDED");
        Time.timeScale = 0;
        _gameEndPanel.gameObject.SetActive(true);

        if (_WinCondition == GameWinCondition.STOCKS)
        {

            for (int i = 0; i < _characterControllers.Count; i++)
            {
                if(_characterControllers[i]._AmountOfStocks >=0)
                {
                    if (_TeamSetup == TeamSetup.TEAM)
                    {
                        _gameEndPanel.SetWinningTeam(_characterControllers[i]._TeamID);  
                    }
                    else if(_TeamSetup == TeamSetup.FFA)
                    {
                        _gameEndPanel.SetWinningPlayer(_characterControllers[i]._PlayerID);
                    }
                }
            }
            
        }
        else if (_WinCondition == GameWinCondition.TIME)
        {
            if (_TeamSetup == TeamSetup.TEAM)
            {
                if (_team1Deaths == _team2Deaths)
                {
                    //TIE
                    _gameEndPanel.SetTie();
                }
                else if (_team1Deaths < _team2Deaths)
                {
                    //TEAM 1 WINS
                    _gameEndPanel.SetWinningTeam(TeamID.Team1);
                }
                else
                {
                    //TEAM 2 WINS
                    _gameEndPanel.SetWinningTeam(TeamID.Team2);
                }
            }
            else
            {
                //find player with least deaths

                int lowestDeaths = int.MaxValue;
                int lowestID=0;
                bool tie = false;
                

                for (int i = 0; i < _characterControllers.Count; i++)
                {
                    if(_characterControllers[i]._AmountOfDeaths < lowestDeaths)
                    {
                        lowestDeaths = _characterControllers[i]._AmountOfDeaths;
                        lowestID = i;
                        tie = false;
                    }
                    else if (_characterControllers[i]._AmountOfDeaths == lowestDeaths)
                    {
                        tie = true;
                    }

                }

                if (tie)
                {
                    _gameEndPanel.SetTie();
                }
                else
                {
                    _gameEndPanel.SetWinningPlayer(_characterControllers[lowestID]._PlayerID);
                }
            }
        }
    }

    private void ResetPowerupSpawnTimer()
    {
        _powerupSpawnTimer = Random.Range(_minPowerupSpawnCooldown, _maxPowerupSpawnCooldown);
    }

    private void SpawnPowerup()
    {
        // where are we spawning?
        List<PowerupSpawnPoint> validSpawnPoints = GetValidPowerupSpawnPoints();
        // make sure there is a valid spawn point first
        if (validSpawnPoints.Count > 0)
        {
            int spawnPointIndex = Random.Range(0, validSpawnPoints.Count - 1);
            PowerupSpawnPoint chosenSpawnPoint = validSpawnPoints[spawnPointIndex];
            // create the powerup
            GameObject spawnedPowerupObject = Instantiate(_powerupPrefab,
                    chosenSpawnPoint.gameObject.transform);
            Powerup spawnedPowerup = spawnedPowerupObject.GetComponent<Powerup>();
            spawnedPowerup.AssignType();
            spawnedPowerup._owningSpawnPoint = chosenSpawnPoint;
            chosenSpawnPoint._containsPowerup = true;
        }
    }

    private void PowerupSpawnTimerTick()
    {
        _powerupSpawnTimer -= Time.deltaTime;
        if (_powerupSpawnTimer <= 0)
        {
            SpawnPowerup();
            ResetPowerupSpawnTimer();
        }
    }

    private List<PowerupSpawnPoint> GetValidPowerupSpawnPoints()
    {
        List<PowerupSpawnPoint> validSpawnPoints = new List<PowerupSpawnPoint>();
        foreach (PowerupSpawnPoint sp in _powerupSpawnPoints)
        {
            if (!sp._containsPowerup)
            {
                validSpawnPoints.Add(sp);
            }
        }
        return validSpawnPoints;
    }

    public void OnPowerupCollected(TeamID teamToApply, Sprite powerupHUDSprite)
    {
        for (int i = 0; i < _playerUIs.Count; i++)
        {
            if (_playerUIs[i]._teamID == teamToApply)
            {
                _playerUIs[i].SetPowerupIcon(powerupHUDSprite);
            }
        }
    }

    public void OnPowerupExpired(TeamID teamToApply)
    {
        for (int i = 0; i < _playerUIs.Count; i++)
        {
            if (_playerUIs[i]._teamID == teamToApply)
            {
                _playerUIs[i].HidePowerupIcon();
            }
        }
    }
}

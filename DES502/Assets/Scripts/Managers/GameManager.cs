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
    private List<CharacterController> _characterControllers = new List<CharacterController>();

    //UI
    private List<PlayerUI> _playerUIs = new List<PlayerUI>();
    private Canvas _canvas;
    private GameEndMenu _gameEndPanel;
    private EventSystem _eventSystem;

    //GameMode stuff
    [HideInInspector]
    public int _PlayerAmount = 2;
    [HideInInspector]
    public TeamSetup _TeamSetup;
    [HideInInspector]
    public GameWinCondition _WinCondition;

    public float _GameTimerLeft=0.0f;

    //stocks
    private int _startingStocksPerPlayer;
    private int _playersWithStocksLeft;

    //time
    private int _team1Deaths=0, _team2Deaths=0;


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


        Debug.Log(_TeamSetup.ToString());
        Debug.Log(_WinCondition.ToString());
        Debug.Log("PLAYERAMOUNT: " + _PlayerAmount);
    }

    void Start()
    {
        _respawnPoints = FindObjectsOfType<RespawnPoint>();

        var playerUIsInScene = FindObjectsOfType<PlayerUI>();
        _canvas = FindObjectOfType<Canvas>();
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
        if(!_gameEndPanel)
        {
            _gameEndPanel =  ObjectFactory.CreateGameEndMenu(this, _canvas);
            _gameEndPanel.transform.SetParent(_canvas.transform, false);
            _gameEndPanel.gameObject.SetActive(false);
        }

        //create TIMER UI
        if(_WinCondition==GameWinCondition.TIME)
        {
            var timer = ObjectFactory.CreateTimerUI(this, _GameTimerLeft);
            timer.transform.SetParent(_canvas.transform, false);

        }

        //create event system
        if(_eventSystem)
        {
            Destroy(_eventSystem.gameObject);
        }

        _eventSystem = ObjectFactory.CreateEventSystem();
        

        //set respawnTime
        for (int i = 0; i < _respawnPoints.Length; i++)
        {
            _respawnPoints[i]._ActiveTimeBeforeRespawn = CharacterController.RespawnDuration;
            _respawnPoints[i]._GM = this;
        }

        //spawn in the players
        for (int i = 0; i < _PlayerAmount; i++)
        {
            PlayerData data;
            data.Id = (PlayerID)i;
            data.Stocks = _startingStocksPerPlayer;
            data.TeamId = (TeamID)(i % 2);
            data.Deaths = 0;
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


        var ui = ObjectFactory.CreatePlayerUI(character, _PlayerAmount, _WinCondition, _TeamSetup);
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

}

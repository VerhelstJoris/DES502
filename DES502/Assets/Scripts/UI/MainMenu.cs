﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameModeScriptableObject _GMScriptableObject;

    [SerializeField]
    private Dropdown _amountPlayersDropdown;
    [SerializeField]
    private Dropdown _teamSetupDropdown;
    [SerializeField]
    private Dropdown _winConditionDropdown;

    [SerializeField]
    private Button _nextButton;

    int _playerAmountJoined = 0;
    int _playerAmountReady = 0;
    // Start is called before the first frame update
    void Start()
    {
        _GMScriptableObject.PlayerAmount = 0;
        _GMScriptableObject.TeamSetup = TeamSetup.FFA;
        _GMScriptableObject.GameWinCondition = GameWinCondition.STOCKS;



        //add delegates here so we don't have to manually add them in the inspector
        _amountPlayersDropdown.onValueChanged.AddListener(delegate
        {
            //SetPlayerAmount(_amountPlayersDropdown);
        });

        _teamSetupDropdown.onValueChanged.AddListener(delegate
        {
            SetTeamMode(_teamSetupDropdown);
        });

        _winConditionDropdown.onValueChanged.AddListener(delegate
        {
            SetWinCondition(_winConditionDropdown);
        });

        //_nextButton.enabled = false;
        //_nextButton.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPlayerAmount(Dropdown dropdown)
    {
        _GMScriptableObject.PlayerAmount = dropdown.value + 2;
        Debug.Log("Player amount set " + _GMScriptableObject.PlayerAmount);
    }

    public void SetTeamMode(Dropdown dropdown)
    {
        _GMScriptableObject.TeamSetup = (TeamSetup)dropdown.value;
        Debug.Log(_GMScriptableObject.TeamSetup.ToString());
    }

    public void SetWinCondition(Dropdown dropdown)
    {
        _GMScriptableObject.GameWinCondition = (GameWinCondition)dropdown.value;
        Debug.Log(_GMScriptableObject.GameWinCondition.ToString());
    }

    public void AddPlayer()
    {
        _playerAmountJoined++;
        _GMScriptableObject.PlayerAmount=_playerAmountJoined;

        if(_playerAmountJoined!= _playerAmountReady)
        {
            //_nextButton.gameObject.SetActive(false);
            //_nextButton.enabled = false;


            //_nextButton.GetComponent<Image>().color = new Color(200, 200, 200);
        }
    }

    public void PlayerReady()
    {
        _playerAmountReady++;

        if (_playerAmountJoined == _playerAmountReady && _playerAmountReady > 1)
        {
            Debug.Log("Activate Button");
            //_nextButton.gameObject.SetActive(true);
            //_nextButton.enabled = true;
            //_nextButton.GetComponent<Image>().color = new Color(255, 255, 255);
        }

        //add the player data to its scriptable object
    }

    public PlayerID GetNextPlayerID()
    {
        Debug.Log("PlayerID: " + (PlayerID)_playerAmountReady);
        return (PlayerID)_playerAmountReady;
    }
}

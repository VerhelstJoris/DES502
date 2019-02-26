using System.Collections;
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


    // Start is called before the first frame update
    void Start()
    {
        _GMScriptableObject.PlayerAmount = 2;
        _GMScriptableObject.TeamSetup = TeamSetup.FFA;
        _GMScriptableObject.GameWinCondition = GameWinCondition.STOCKS;

        //add delegates here so we don't have to manually add them in the inspector
        _amountPlayersDropdown.onValueChanged.AddListener(delegate
        {
            SetPlayerAmount(_amountPlayersDropdown);
        });

        _teamSetupDropdown.onValueChanged.AddListener(delegate
        {
            SetTeamMode(_teamSetupDropdown);
        });

        _winConditionDropdown.onValueChanged.AddListener(delegate
        {
            SetWinCondition(_winConditionDropdown);
        });
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
}

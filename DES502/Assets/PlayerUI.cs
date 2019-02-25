using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerID _PlayerID;

    private int _amountOfStocks;

    [SerializeField]
    private Text _stocksText;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void Initialize(CharacterController character, int amountOfPlayers, GameWinCondition winCondition)
    {
        _PlayerID = character._PlayerID;
        _amountOfStocks = character._AmountOfStocks;

        this.transform.position = new Vector3(-350 + ((700.0f / (amountOfPlayers-1)) * (int)_PlayerID), 150, 0);

        if (winCondition == GameWinCondition.STOCKS)
        {
            _stocksText.text = _amountOfStocks.ToString();
        }
        else
        {
            _stocksText.text = "";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

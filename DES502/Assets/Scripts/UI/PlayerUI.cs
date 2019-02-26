using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerID _PlayerID;
    private TeamID _teamID;
    private int _amountOfStocks;

    [SerializeField]
    private Text _stocksText;
    private Image _bgImage;

    private void Awake()
    {
        _bgImage = this.GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    public void Initialize(CharacterController character, int amountOfPlayers, GameWinCondition winCondition, TeamSetup teamSetup)
    {
        _PlayerID = character._PlayerID;
        _teamID = character._TeamID;
        _amountOfStocks = character._AmountOfStocks;

        this.transform.position = new Vector3(-350 + ((700.0f / (amountOfPlayers-1)) * (int)_PlayerID), 150, 0);

        switch (_PlayerID)
        {
            case PlayerID.Player1:
                this.transform.position = new Vector3(-375 , 150, 0);
                break;
            case PlayerID.Player2:
                this.transform.position = new Vector3(375, 150, 0);
                break;
            case PlayerID.Player3:
                this.transform.position = new Vector3(-375, -150, 0);
                break;
            case PlayerID.Player4:
                this.transform.position = new Vector3(375, -150, 0);
                break;
            default:
                break;
        }


        if (winCondition == GameWinCondition.STOCKS)
        {
            _stocksText.text = _amountOfStocks.ToString();
        }
        else
        {
            _stocksText.text = "";
        }

        Color temp;

        if (teamSetup == TeamSetup.FFA)
        {
            PlayerHelpers.PlayerColorDictionary.TryGetValue(_PlayerID, out temp);
            _bgImage.color = temp;
        }
        else if (teamSetup == TeamSetup.TEAM)
        {
            PlayerHelpers.TeamColorDictionary.TryGetValue(_teamID, out temp);
            _bgImage.color = temp;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public PlayerID _PlayerID;
    private TeamID _teamID;
    private GameWinCondition _winCondition;
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
        Debug.Log(_teamID.ToString());

        _amountOfStocks = character._AmountOfStocks;
        _winCondition = winCondition;

        this.transform.position = new Vector3(-350 + ((700.0f / (amountOfPlayers-1)) * (int)_PlayerID), 150, 0);

        var rectTransform = GetComponent<RectTransform>();


        switch (_PlayerID)
        {
            case PlayerID.Player1:
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(0, 1);
                this.transform.position = new Vector3(50, -50, 0);

                break;
            case PlayerID.Player2:
                rectTransform.anchorMin = new Vector2(1, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                this.transform.position = new Vector3(-50, -50, 0);
                break;
            case PlayerID.Player3:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                this.transform.position = new Vector3(50, 50, 0);
                break;
            case PlayerID.Player4:
                rectTransform.anchorMin = new Vector2(1, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                this.transform.position = new Vector3(-50, 50, 0);
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

    public void UpdateStockText(int stocks)
    {
        if (_winCondition == GameWinCondition.STOCKS)
        {
            _amountOfStocks = stocks;
            _stocksText.text = _amountOfStocks.ToString();
        }

    }
}

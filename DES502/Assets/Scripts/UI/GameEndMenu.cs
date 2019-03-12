using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameEndMenu : MonoBehaviour
{
    private Canvas _canvas;

    [SerializeField]
    private Button _menuButton;

    [SerializeField]
    private Text _endText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Initialize(GameManager manager, Canvas canvas)
    {
        _canvas = canvas;
        //_menuButton.onClick.AddListener(LoadMainMenuOnClick);
        Debug.Log("Initialized");
    }

    void LoadMainMenuOnClick()
    {
        _canvas.GetComponent<LoadOnClick>().LoadScene(0);
    }

    public void SetWinningPlayer(PlayerID id)
    {
        _endText.text = id.ToString() + " Wins!";
    }

    public void SetWinningTeam(TeamID team)
    {
        
        _endText.text = team.ToString() + " Wins!";
        
    }

    public void SetTie()
    {
        _endText.text = "TIE!!!";
    }

}

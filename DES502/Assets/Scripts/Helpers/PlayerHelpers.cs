using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum HitboxID { Up, Down, Side };

public enum PlayerState { Idle, Jumping, Running, Dead };

public enum AttackType { Side, Up, Down, None };

public enum PlayerID { Player1, Player2, Player3, Player4 };

public enum ControllerID { Controller1, Controller2, Controller3, Controller4 };

public enum CharacterID { Rabbit, Fox};

public enum TeamID { Team1, Team2 };


public struct PlayerData
{
    public PlayerID Id;
    public ControllerID controllerID;
    public CharacterID charID;
    public int skinID;
    public int Stocks;
    public int Deaths;
    public TeamID TeamId;
};

public class PlayerHelpers : MonoBehaviour
{
    //PlayerColours
    public static Dictionary<PlayerID, Color> PlayerColorDictionary = new Dictionary<PlayerID, Color>()
    {
        {PlayerID.Player1 , new Color(1.0f, 0, 0, 1) },
        {PlayerID.Player2 , new Color(0, 1.0f, 0, 1) },
        {PlayerID.Player3 , new Color(0, 0, 1.0f, 1) },
        {PlayerID.Player4 , new Color(1.0f, 1.0f, 0, 1) },
    };

    public static Dictionary<TeamID, Color> TeamColorDictionary = new Dictionary<TeamID, Color>()
    {
        {TeamID.Team1 , new Color(1.0f, 0, 0, 1) },
        {TeamID.Team2 , new Color(0, 0, 1.0f, 1) },
    };

}


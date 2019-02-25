using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum HitboxID { Up, Down, Side };

public enum PlayerState { Idle, Jumping, Running, Dead };

public enum AttackType { Side, Up, Down, None };

public enum PlayerID { Player1, Player2, Player3, Player4 };

public struct PlayerData
{
    public PlayerID Id;
    public int Stocks;
}


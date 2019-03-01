using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="GameModeData", menuName ="Data/Gamemode", order = 1 )]
public class GameModeScriptableObject: ScriptableObject
{
    public int PlayerAmount = 2;
    public TeamSetup TeamSetup = (TeamSetup)0;
    public GameWinCondition GameWinCondition = (GameWinCondition)0;

    public int AmountOfStocks = 3;
    public float AmountOfTime = 180.0f;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerData", menuName ="Data/Player", order = 1 )]
public class PlayerDataScriptableObject: ScriptableObject
{
    public bool Active;
    public ControllerID ControllerID;   //which controllers
    public PlayerID PlayerID;           //
    public CharacterID CharacterID;     //rabbit fox
    public int SkinID;                  //WHICH SKIN
}

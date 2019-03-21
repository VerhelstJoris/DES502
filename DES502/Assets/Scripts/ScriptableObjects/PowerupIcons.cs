using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PowerupIconData", menuName = "Data/PowerupIcons", order = 1)]
public class PowerupIcons : ScriptableObject
{
    public List<PowerupData> powerups;
}

[System.Serializable]
public class PowerupData
{
    public string powerupName;
    public Sprite inGameCollectableSprite;
    //public Texture sidebarHUDTexture;
    public Sprite sidebarHUDSprite;
}

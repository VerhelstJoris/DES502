using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GlobalTrapDataAsset", menuName = "Data/Global Trap Data", order = 1)]
public class GlobalTrapData : ScriptableObject
{
    [Tooltip("What color should the sprite renderer use for traps that are currently on cooldown?")]
    public Color _cooldownSpriteColor;
    [Tooltip("How long (in seconds) before cooldown has ended should we start flashing between cooldown color and default?")]
    public float _colorBlinkDuration = 1.5f;
    [Range(0, 1)]
    [Tooltip("How long (in seconds) should we wait in between flip flopping from default sprite color and cooldown sprite color while blinking?")
    public float _colorBlinkWaitDuration = 0.2f;
}

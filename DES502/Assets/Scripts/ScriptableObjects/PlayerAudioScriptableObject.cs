using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="PlayerAudio", menuName ="Data/Audio", order = 1 )]
public class PlayerAudioScriptableObject: ScriptableObject
{
    public AudioClip GettingHitClip;
    public AudioClip[] RunningClips;
}

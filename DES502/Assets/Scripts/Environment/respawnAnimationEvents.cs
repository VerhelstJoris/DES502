using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class respawnAnimationEvents : MonoBehaviour
{
    private RespawnPoint _respawnPoint;

    void Start()
    {
        _respawnPoint = GetComponentInParent<RespawnPoint>();
    }

   void DoorOpened()
   {
        _respawnPoint.AnimDoorOpened();
   }

    void DoorClosed()
    {
        _respawnPoint.AnimDoorClosed();
    }
}

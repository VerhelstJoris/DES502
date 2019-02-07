using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{
    protected static ObjectFactory instance; // Needed
    public GameObject _PlayerPrefab;
   
    void Start()
    {
        instance = this;
    }


    public static CharacterController CreatePlayer(PlayerID id, Vector3 position)
    {
        var character = Object.Instantiate(instance._PlayerPrefab, Vector3.zero, Quaternion.identity).GetComponent<CharacterController>();
        character.Initialize(id);
        character.transform.position = position;
        return character;
    }
    

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{
    protected static ObjectFactory instance; // Needed
    public GameObject _PlayerPrefab;
    public GameObject _ProjectileAttackPrefab;
    public GameObject _PlayerTagPrefab;

    void Start()
    {
        instance = this;
    }


    public static CharacterController CreatePlayer(PlayerID id, Vector3 position)
    {
        var character = Object.Instantiate(instance._PlayerPrefab, Vector3.zero, Quaternion.identity).GetComponent<CharacterController>();
        character.Initialize(id);
        character.transform.position = position;

        CreatePlayerTag(character);

        return character;
    }

    public static PlayerTag CreatePlayerTag(CharacterController character)
    {
        var playerTag = Object.Instantiate(instance._PlayerTagPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerTag>();
        playerTag.Initialize(character);

        character._PlayerTag = playerTag;

        return playerTag;
    }



    public static ProjectileAttack CreateProjectile(PlayerID owner, Vector3 position, Vector2 direction, float launchAmount, float stunduration, float dropduration, int launchspeed, float dropgravityscale)
    {
        var projectile = Object.Instantiate(instance._ProjectileAttackPrefab, Vector3.zero, Quaternion.identity).GetComponent<ProjectileAttack>();

        projectile.Inititalize(owner, direction, launchAmount, stunduration, dropduration, launchspeed, dropgravityscale);
        projectile.transform.position = position;
        return projectile;
    }

}

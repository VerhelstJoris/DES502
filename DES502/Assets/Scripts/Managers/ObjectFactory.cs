using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{
    protected static ObjectFactory instance; // Needed
    public GameObject _PlayerPrefab;
    public GameObject _ProjectileAttackPrefab;

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
    
    public static ProjectileAttack CreateProjectile(PlayerID owner, Vector3 position, Vector2 direction, float launchAmount, float stunduration)
    {
        var projectile = Object.Instantiate(instance._ProjectileAttackPrefab, Vector3.zero, Quaternion.identity).GetComponent<ProjectileAttack>();

        projectile.Inititalize(owner, direction, launchAmount, stunduration);
        projectile.transform.position = position;
        return projectile;
    }

}
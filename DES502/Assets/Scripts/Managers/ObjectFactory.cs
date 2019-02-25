using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFactory : MonoBehaviour
{
    protected static ObjectFactory instance; // Needed
    public GameObject _PlayerPrefab;
    public GameObject _ProjectileAttackPrefab;
    public GameObject _PlayerTagPrefab;
    public GameObject _PlayerUIPrefab;

    void Start()
    {
        instance = this;
    }


    public static CharacterController CreatePlayer(PlayerData data, Vector3 position)
    {
        var character = Object.Instantiate(instance._PlayerPrefab, Vector3.zero, Quaternion.identity).GetComponent<CharacterController>();
        character.Initialize(data);
        character.transform.position = position;

        CreatePlayerTag(character);

        return character;
    }

    public static PlayerTag CreatePlayerTag(CharacterController character)
    {
        var playerUI = Object.Instantiate(instance._PlayerTagPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerTag>();
        playerUI.Initialize(character);


        return playerUI;
    }

    public static PlayerUI CreatePlayerUI(CharacterController character, int amountOfPlayers, GameWinCondition winCondition)
    {
        var playerUI = Object.Instantiate(instance._PlayerUIPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerUI>();
        playerUI.Initialize(character, amountOfPlayers, winCondition);

        return playerUI;
    }




    public static ProjectileAttack CreateProjectile(PlayerID owner, Vector3 position, Vector2 direction, float launchAmount, float stunduration)
    {
        var projectile = Object.Instantiate(instance._ProjectileAttackPrefab, Vector3.zero, Quaternion.identity).GetComponent<ProjectileAttack>();

        projectile.Inititalize(owner, direction, launchAmount, stunduration);
        projectile.transform.position = position;
        return projectile;
    }

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectFactory : MonoBehaviour
{
    protected static ObjectFactory instance; // Needed
    public GameObject _PlayerPrefab;
    public GameObject _ProjectileAttackPrefab;
    public GameObject _PlayerTagPrefab;
    public GameObject _PlayerUIPrefab;
    public GameObject _TimerUIPrefab;
    public GameObject _GameEndMenuPrefab;
    public GameObject _EventSystemPrefab;

    void Start()
    {
        instance = this;
    }


    public static CharacterController CreatePlayer(PlayerData data, Vector3 position, TeamSetup setup)
    {
        var character = Object.Instantiate(instance._PlayerPrefab, Vector3.zero, Quaternion.identity).GetComponent<CharacterController>();
        character.Initialize(data);
        character.transform.position = position;

        CreatePlayerTag(character, setup);

        return character;
    }

    public static PlayerTag CreatePlayerTag(CharacterController character, TeamSetup setup)
    {
        var playerUI = Object.Instantiate(instance._PlayerTagPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerTag>();
        playerUI.Initialize(character, setup);


        return playerUI;
    }

    public static PlayerUI CreatePlayerUI(CharacterController character, int amountOfPlayers, GameWinCondition winCondition, TeamSetup teamSetup, CharacterID charId)
    {
        var playerUI = Object.Instantiate(instance._PlayerUIPrefab, Vector3.zero, Quaternion.identity).GetComponent<PlayerUI>();
        playerUI.Initialize(character, amountOfPlayers, winCondition,teamSetup);

        character._PlayerUI = playerUI;
        return playerUI;
    }

    public static TimerUI CreateTimerUI(GameManager manager,float amountOfTime)
    {
        var timerUI = Object.Instantiate(instance._TimerUIPrefab, Vector3.zero, Quaternion.identity).GetComponent<TimerUI>();
        timerUI.Initialize(manager, amountOfTime);
        return timerUI;
    }

    public static GameEndMenu CreateGameEndMenu(GameManager manager, Canvas canvas)
    {
        var endMenu = Object.Instantiate(instance._GameEndMenuPrefab, Vector3.zero, Quaternion.identity).GetComponent<GameEndMenu>();
        endMenu.Initialize(manager,canvas);
        return endMenu;
    }

    public static EventSystem CreateEventSystem()
    {
        var system = Object.Instantiate(instance._EventSystemPrefab, Vector3.zero, Quaternion.identity).GetComponent<EventSystem>();
        return system;
    }


    public static ProjectileAttack CreateProjectile(PlayerID owner, Vector3 position, Vector2 direction, float launchAmount, float stunduration, float dropduration, int launchspeed, float dropgravityscale)
    {
        var projectile = Object.Instantiate(instance._ProjectileAttackPrefab, Vector3.zero, Quaternion.identity).GetComponent<ProjectileAttack>();

        projectile.Inititalize(owner, direction, launchAmount, stunduration, dropduration, launchspeed, dropgravityscale);
        projectile.transform.position = position;
        return projectile;
    }

}

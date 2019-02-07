using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    private Animator _animator;

    public bool _Active = false;
    bool _preRespawn = true;
    bool _doorOpened = false;
    bool _doorClosed = false;

    [SerializeField]
    private Transform _startingPoint;
    [SerializeField]
    public Transform _RespawnPoint;
    [SerializeField]
    private GameObject _door;

    private float _doorOpenDuration;
    [HideInInspector] public float _ActiveTimeBeforeRespawn = 1.0f;
    [SerializeField] private float _activeTimeAfterRespawn = 1.0f;

    private float _activeTimer;

    private CharacterController _charToRespawn=null;
    private PlayerID _IdToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        _animator = _door.GetComponent<Animator>();
        _door.transform.position = _startingPoint.transform.position;


        //get length of animation clip
        RuntimeAnimatorController ac = _animator.runtimeAnimatorController;    //Get Animator controller
        for (int i = 0; i < ac.animationClips.Length; i++)                 //For all animations
        {
            if (ac.animationClips[i].name == "DoorOpening")        //If it has the same name as your clip
            {
                _doorOpenDuration = ac.animationClips[i].length;
            }

       
        }

    }

    // Update is called once per frame
    void Update()
    {
        if(_Active)
        {
            if(_preRespawn)
            {
                _activeTimer += Time.deltaTime;

                _door.transform.position = Vector3.Lerp(_startingPoint.transform.position, _RespawnPoint.transform.position, _activeTimer/(_ActiveTimeBeforeRespawn-_doorOpenDuration));

          
                if(_doorOpened)
                {
                    _preRespawn = false;
                    _activeTimer = 0.0f;
                    _doorClosed = false;
                    Respawn();
                }

                if (_activeTimer >= (_ActiveTimeBeforeRespawn - _doorOpenDuration))
                {
                    _animator.SetBool("DoorTriggered", true);
                }
            }
            else
            {
                if(_doorClosed)
                {
                    _activeTimer += Time.deltaTime;
                    _door.transform.position = Vector3.Lerp(_RespawnPoint.transform.position, _startingPoint.transform.position, _activeTimer / _activeTimeAfterRespawn);

                    if (_activeTimer >= _activeTimeAfterRespawn)
                    {
                        _activeTimer = 0.0f;
                        _Active = false;
                    }

                }    
            }

        }
        
    }

    public void Activate(CharacterController player)
    {
        _charToRespawn = player;
        _IdToSpawn = player.GetComponent<CharacterController>()._PlayerID;

        _Active = true;
        _preRespawn = true;
        _activeTimer = 0.0f;
        _doorOpened = false;
        _doorClosed = false;
        _animator.SetBool("DoorClosed", false);

    }

    private void Respawn()
    {
        //_charToRespawn.transform.position = _RespawnPoint.transform.position;

        //CharacterController character = Instantiate<CharacterController>(_charToRespawn);
        CharacterController player = ObjectFactory.CreatePlayer(_IdToSpawn, _RespawnPoint.transform.position);

    }

    public void AnimDoorOpened()
    {
        //animation event via reference class
        _doorOpened = true;

        Debug.Log("Door Opened");
    }


    public void AnimDoorClosed()
    {        
        //animation event via reference class
        _doorClosed = true;
        _animator.SetBool("DoorTriggered", false);
        _animator.SetBool("DoorClosed", true);
        Debug.Log("Door Closed");
    }
}

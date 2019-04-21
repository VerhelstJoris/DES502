using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    [SerializeField]
    public HitboxID _hitboxID;

    private BoxCollider2D _collider;
    private CharacterController _charController;
    private float _launchAmount;
    private Vector2 _defaultLaunchVector;
    private float _stunDuration;
    private CameraShake _cameraShake;
    private List<Collider2D> _playerColliders = new List<Collider2D>();
    private bool _active;
    private List<FallingRock> _overlappingRocks = new List<FallingRock>();
    // Use this for initialization
    void Awake () {
        _collider = this.GetComponent<BoxCollider2D>();
        _charController = this.GetComponentInParent<CharacterController>();
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
    }

    private void Start()
    {
        float launchAngle;
        Vector2 defaultDirection = new Vector2(0, 0);

        switch (_hitboxID)
        {
            case HitboxID.Up:
                launchAngle = _charController._UpAttackLaunchAngle;

                defaultDirection.x = Mathf.Cos(Mathf.Deg2Rad * (launchAngle+90));
                defaultDirection.y = Mathf.Sin(Mathf.Deg2Rad * (launchAngle+90));

                _stunDuration = _charController._UpAttackStunDuration;
                break;
            case HitboxID.Down:
                launchAngle = _charController._DownAttackLaunchAngle;

                defaultDirection.x = Mathf.Cos(Mathf.Deg2Rad * (launchAngle + 270));
                defaultDirection.y = Mathf.Sin(Mathf.Deg2Rad * (launchAngle + 270));

                _stunDuration = _charController._DownAttackStunDuration;

                break;
            case HitboxID.Side:
                launchAngle = _charController._SideAttackLaunchAngle;

                defaultDirection.x = Mathf.Cos(Mathf.Deg2Rad * launchAngle);
                defaultDirection.y = Mathf.Sin(Mathf.Deg2Rad * launchAngle);

                _stunDuration = _charController._SideAttackStunDuration;

                break;
            default:
                break;
        }

        defaultDirection.Normalize();
        _defaultLaunchVector = defaultDirection;
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            if (!_playerColliders.Contains(col))
            {
                _playerColliders.Add(col);
            }
            
        }
        if (col.tag == "Rock")
        {
            FallingRock overlappingRock = col.gameObject.GetComponent<FallingRock>();
            if (!_overlappingRocks.Contains(overlappingRock))
            {
                _overlappingRocks.Add(overlappingRock);
            }
        }
        // does any of this even activate???
        if (_active)
        {

            Vector2 launchVector = _defaultLaunchVector;
            if (!_charController._FacingRight)
            {
                launchVector.x *= -1;
            }

            if (col.tag == "Player")
            {
                //Debug.Log("Player");
                CharacterController collidedPlayer = col.GetComponent<CharacterController>();
                collidedPlayer.RecieveHit(launchVector * _launchAmount,
                        _stunDuration, true);
                float[] cameraShakeValues = collidedPlayer.GetCameraShakeValues();
                _cameraShake.BeginShake(cameraShakeValues[0], cameraShakeValues[1]);
            }

            if (col.tag == "Weapon")
            {
                //parry
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            _playerColliders.Remove(col);
        }
        if (col.tag == "Rock")
        {
            FallingRock overlappingRock = col.gameObject.GetComponent<FallingRock>();
            _overlappingRocks.Remove(overlappingRock);
        }
    }

    public void SetLaunchAmount(float amount)
    {
        _launchAmount = amount;
    }

    public void Activate(bool active = true)
    {
        _active = active;

        if (active)
        {
            Vector2 launchVector = _defaultLaunchVector;
            if (!_charController._FacingRight)
            {
                launchVector.x *= -1;
            }
            for (int i = 0; i < _playerColliders.Count; i++)
            {
                if (_playerColliders[i].tag == "Player")
                {
                    //Debug.Log("Player");
                    CharacterController collidedPlayer = _playerColliders[i].GetComponent<CharacterController>();
                    collidedPlayer.RecieveHit(launchVector * _launchAmount,
                            _stunDuration, true);
                    float[] cameraShakeValues = collidedPlayer.GetCameraShakeValues();
                    _cameraShake.BeginShake(cameraShakeValues[0], cameraShakeValues[1]);
                }
            }
            foreach (FallingRock r in _overlappingRocks)
            {
                Debug.Log("HIT ROCK");
                r.AddKnockback(transform.position, launchVector, _launchAmount);
            }
        }
    }
}

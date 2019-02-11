using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class WeaponColliderEditor : MonoBehaviour
{


    private BoxCollider2D _collider;
    private CharacterController _charController;

    private HitboxID _hitboxID;


    private void Awake()
    {
        _collider = this.GetComponent<BoxCollider2D>();
        _charController = this.GetComponentInParent<CharacterController>();

        _hitboxID = this.GetComponent<MeleeAttack>()._hitboxID;

    }

    private void Start()
    {
        this.GetComponent<SpriteRenderer>().enabled = false;
        _collider.enabled = false;
    }

    // Update is called once per frame
    void Update ()
    {

        switch (_hitboxID)
        {
            case HitboxID.Up:
                _collider.size = _charController._UpAttackSize;
                //_collider.offset = _charController._UpAttackOffset;
                _collider.transform.localPosition = new Vector3(_charController._UpAttackOffset.x, _charController._UpAttackOffset.y, 0);
                break;
            case HitboxID.Down:
                _collider.size = _charController._DownAttackSize;
                //_collider.offset = _charController._DownAttackOffset;
                _collider.transform.localPosition = new Vector3(_charController._DownAttackOffset.x, _charController._DownAttackOffset.y, 0);

                break;
            case HitboxID.Side:
                _collider.size = new Vector2( _charController._SideAttackSize.y, _charController._SideAttackSize.x);
                //_collider.offset = _charController._SideAttackOffset;
                _collider.transform.localPosition = new Vector3(_charController._SideAttackOffset.x, _charController._SideAttackOffset.y, 0);

                break;
            default:
                break;
        }

    }
}




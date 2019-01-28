using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ColliderEditorChange : MonoBehaviour {

    enum HitboxId { Up, Down, Left, Right };

    private BoxCollider2D _collider;
    private CharacterController _charController;

    [SerializeField]
    HitboxId _hitBoxID = HitboxId.Up;

    private void Awake()
    {
        _collider = this.GetComponent<BoxCollider2D>();
        _charController = this.GetComponentInParent<CharacterController>();
    }

    // Update is called once per frame
    void Update ()
    {

        switch (_hitBoxID)
        {
            case HitboxId.Up:
                _collider.size = _charController._UpAttackSize;
                _collider.offset = _charController._UpAttackOffset;
                break;
            case HitboxId.Down:
                _collider.size = _charController._DownAttackSize;
                _collider.offset = _charController._DownAttackOffset;
                break;
            case HitboxId.Left:
                _collider.size = _charController._SideAttackSize;
                _collider.offset = new Vector2( - _charController._SideAttackOffset.x , _charController._SideAttackOffset.y);
                break;
            case HitboxId.Right:
                _collider.size = _charController._SideAttackSize;
                _collider.offset = _charController._SideAttackOffset;
                break;
            default:
                break;
        }

    }
}

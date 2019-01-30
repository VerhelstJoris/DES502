using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MeleeVectorEditor : MonoBehaviour {


    private CharacterController _charController;
    [SerializeField]
    HitboxID _hitboxID;


    private void Awake()
    {


        _charController = this.GetComponentInParent<CharacterController>();
    }

    // Use this for initialization
    void Start () {
        this.GetComponent<SpriteRenderer>().enabled = false;


    }

    // Update is called once per frame
    void Update () {

        switch (_hitboxID)
        {
            case HitboxID.Up:
                this.transform.eulerAngles = new Vector3(0, 0,90+ _charController._UpAttackLaunchAngle);
                break;
            case HitboxID.Down:
                this.transform.eulerAngles = new Vector3(0, 0, -90 + _charController._DownAttackLaunchAngle);
                break;
            case HitboxID.Side:
                this.transform.eulerAngles = new Vector3(0,0,_charController._SideAttackLaunchAngle);
                break;
            default:
                break;
        }
    }
}

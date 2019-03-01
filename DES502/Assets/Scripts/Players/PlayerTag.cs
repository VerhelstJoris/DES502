using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTag : MonoBehaviour
{
    private PlayerID _id;
    private CharacterController _character;
    private TextMesh _text;

    void Awake ()
    {
        _text = GetComponentInChildren<TextMesh>();
    }

    void Update()
    {
        if (_character)
        {
            Vector3 charPos = _character.transform.position;
            this.transform.position = new Vector3(charPos.x, charPos.y + 0.6f, charPos.z);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    public void Initialize(CharacterController character)
    {
        _character = character;
        _id = character._PlayerID;

        switch (_id)
        {
            case PlayerID.Player1:
                _text.text = "P1";
                break;
            case PlayerID.Player2:
                _text.text = "P2";
                break;
            case PlayerID.Player3:
                _text.text = "P3";
                break;
            case PlayerID.Player4:
                _text.text = "P4";
                break;
            default:
                break;
        }

        

        Color temp;
        PlayerHelpers.PlayerColorDictionary.TryGetValue(_id, out temp);
        _text.color = temp;
    }
}

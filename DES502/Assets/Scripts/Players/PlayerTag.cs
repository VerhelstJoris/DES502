using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTag : MonoBehaviour
{
    private PlayerID _id;
    private TeamID _team;
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

    public void Initialize(CharacterController character, TeamSetup setup)
    {
        _character = character;
        _id = character._PlayerID;
        _team = _character._TeamID;

        
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
        if (setup == TeamSetup.FFA)
        {
            PlayerHelpers.PlayerColorDictionary.TryGetValue(_id, out temp);
        }
        else
        {
            PlayerHelpers.TeamColorDictionary.TryGetValue(_team, out temp);
        }
        _text.color = temp;
    }
}

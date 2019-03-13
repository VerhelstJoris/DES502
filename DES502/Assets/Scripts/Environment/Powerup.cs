using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum POWERUP_TYPES
    {
        TEST
    }

    [SerializeField] [Tooltip("What type of powerup is this?")]
    public POWERUP_TYPES _type;

    [Header("Sprite Animation")]

    [Range(0.0f, 0.5f)] [SerializeField] [Tooltip("How far (in units) should the sprite move from it's origin point?")]
    public float _maxYMovement = 0.2f;
    [Range(0.0f, 3.0f)] [SerializeField] [Tooltip("How fast should the sprite move?")]
    public float _animationSpeed = 1.5f;

    private GameObject _spriteObject;
    private float _animationTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        _spriteObject = transform.GetChild(0).gameObject;
        Debug.Log(_spriteObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        _animationTimer += Time.deltaTime * _animationSpeed;
        /*
        if (_animationTimer > 10)
        {
            _animationTimer -= 10;
        }
        */
        float newYPos = Mathf.Sin(_animationTimer) * _maxYMovement;
        //Debug.Log(newYPos);
        _spriteObject.transform.localPosition = new Vector2(0, newYPos);
    }
}

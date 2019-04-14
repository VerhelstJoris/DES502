using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilBody : MonoBehaviour
{
    [SerializeField] private GameObject _parentBody;

    void OnTriggerEnter2D(Collider2D other)
    {
        _parentBody.GetComponent<RisingOil>().OnChildTriggerEnter2D(other);
    }

    public void SetSpriteColor(Color bodyColor)
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = bodyColor;
    }

    public void SetSize(int levelWidth, int levelHeight)
    {
        // Sets the scale of the body object from the size of the level
        float tilesCoveredByBaseSize = 2;
        float scaleX = levelWidth / tilesCoveredByBaseSize;
        float scaleY = levelHeight / tilesCoveredByBaseSize;
        transform.localScale = new Vector3(scaleX, scaleY, 1);
    }
}

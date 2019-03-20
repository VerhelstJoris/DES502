using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawnPoint : MonoBehaviour
{
    [Range(0f, 1f)] [Tooltip("Transparency percent to use for gizmo sprite, in 0-1 space.")]
    public float _spriteTransparency = 0.7f;
    private SpriteRenderer spriteRenderer;

    // Disabling SpriteRenderer method
    void OnValidate()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(1f, 1f, 1f, _spriteTransparency);
    }
    void Awake()
    {
        spriteRenderer.enabled = false;
    }

    // Gizmo method
    // Doesn't work as well as I'd like as you can't have it placed inbetween grid units
    /*
    [Tooltip("Sprite to use when drawing the gizmo. (set this to the random powerup sprite!)")]
    public Texture _powerupSprite;
    public static readonly Vector2 GRID_UNIT = new Vector2 (1f, -1f);

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 1f, 1f, _spriteTransparency);
        DrawSprite();
    }

    private void DrawSprite()
    {
        //Rect texRect = new Rect (transform.position, new Vector2 (1f, -1f));
        Vector2 position2D = new Vector2 (transform.position.x, transform.position.y);
        //Rect texRect = new Rect (transform.position, GRID_UNIT + (GRID_UNIT / 2));
        Rect texRect = new Rect (transform.position, GRID_UNIT);
        //if (!Application.isPlaying)
            Gizmos.DrawGUITexture(texRect, _powerupSprite);
    }
    */
}

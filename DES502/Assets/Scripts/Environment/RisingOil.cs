using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RisingOil : MonoBehaviour
{
    [SerializeField] [Tooltip("The color to be used for the body of the oil.")]
    private Color _bodyColor;

    const int TILE_SIZE = 250;

    [HideInInspector]
    public float _maxYPos;
    private Vector3Int _tilemapSize;

    void Awake()
    {
        SetMaxYPos();
    }

    // Start is called before the first frame update
    void Start()
    {
        _tilemapSize = GetTilemapSize();
        Debug.Log("_tilemapSize: " + _tilemapSize.ToString());
        SetBodySprite(_bodyColor);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void SetBodySprite(Color bodyColor)
    {
        Texture2D tex = CreateBodyTexture();
        Rect texRect = new Rect(0f, 0f, tex.width, tex.height);
        Vector2 texPivot = new Vector2(tex.width / 2, 0);
        Sprite bodySprite = Sprite.Create(tex, texRect, texPivot);
        GetComponent<SpriteRenderer>().sprite = bodySprite;
    }

    private Texture2D CreateBodyTexture()
    {
        Vector2Int texSize = CalculateTextureSize();
        /*
        Texture2D tex = new Texture2D(texSize.x, texSize.y);
        Color[] pixels = tex.GetPixels();
        // set all the pixels of the texture to be the specified body colour
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = _bodyColor;
        }
        tex.SetPixels(pixels);
        tex.Apply(true);
        */
        Texture2D tex = Texture2D.whiteTexture;
        //tex.Resize(texSize.x, texSize.y);
        //tex.Apply(true);
        return tex;
    }

    private Vector2Int CalculateTextureSize()
    {
        Vector2Int tilemapSize2D = new Vector2Int(_tilemapSize.x, _tilemapSize.y);
        return tilemapSize2D * TILE_SIZE;
    }

    private Vector3Int GetTilemapSize()
    {
        Tilemap tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        return tilemap.size;
    }

    private void SetMaxYPos()
    {
        GameObject gameManagerObject = transform.parent.gameObject;
        GameManager gameManager = gameManagerObject.GetComponent<GameManager>();
        _maxYPos = gameManager.GetMaxOilYPos();
    }
}

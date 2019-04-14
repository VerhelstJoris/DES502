using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RisingOil : MonoBehaviour
{
    [SerializeField]
    private GameObject _oilBodyGameObject;
    [SerializeField] [Tooltip("The color to be used for the body of the oil.")]
    private Color _bodyColor;

    const int TILE_SIZE = 250;

    [HideInInspector]
    public float _maxYPos;
    private Vector3Int _tilemapSize;
    private OilBody _oilBody;
    private GameManager _gameManager;

    void Awake()
    {
        _oilBody = _oilBodyGameObject.GetComponent<OilBody>();
        SetGameManager();
        SetMaxYPos();
    }

    void Start()
    {
        _tilemapSize = GetTilemapSize();
        Debug.Log("_tilemapSize: " + _tilemapSize.ToString());
        _oilBody.SetSpriteColor(_bodyColor);
        _oilBody.SetSize(_tilemapSize.x, _tilemapSize.y);
        SetInitialPosition(_tilemapSize);
    }

    // The following is for dynamic texture generation, which has been depricated as it does not work
    /*
    private void SetBodySprite(Color bodyColor)
    {
        // I have no idea why the following doesnt work or how to make it work, just add a big white square texture
        Texture2D tex = CreateBodyTexture(bodyColor);
        Rect texRect = new Rect(0f, 0f, tex.width, tex.height);
        Vector2 texPivot = new Vector2(tex.width / 2, 0);
        float pixelsPerUnit = (float)TILE_SIZE;
        Sprite bodySprite = Sprite.Create(tex, texRect, texPivot, 100000f);
        //SpriteRenderer spriteRenderer = _oilBody.GetComponent<SpriteRenderer>();
        //spriteRenderer.sprite = bodySprite;
        //spriteRenderer.color = bodyColor;
    }

    private Texture2D CreateBodyTexture(Color bodyColor)
    {
        //Vector2Int texSize = CalculateTextureSize();
        // this doesn't work!!!!!!!!
        Texture2D tex = new Texture2D(texSize.x, texSize.y);
        Color[] pixels = tex.GetPixels();
        // set all the pixels of the texture to be the specified body colour
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = _bodyColor;
        }
        tex.SetPixels(pixels);
        tex.Apply(true);
        // TODO: if it remains as just this line refactor out this function
        Texture2D tex = Texture2D.whiteTexture;
        //tex.SetPixel(1,1, bodyColor);
        //tex.Resize(texSize.x, texSize.y);  // this is very slow!!!!!!!
        //tex.Apply(true);
        return tex;
    }

    private Vector2Int CalculateTextureSize()
    {
        Vector2Int tilemapSize2D = new Vector2Int(_tilemapSize.x, _tilemapSize.y);
        return tilemapSize2D * TILE_SIZE;
    }
    */

    private Vector3Int GetTilemapSize()
    {
        Tilemap tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        return tilemap.size;
    }

    private void SetMaxYPos()
    {
        _maxYPos = _gameManager.GetMaxOilYPos();
    }

    public void OnChildTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<CharacterController>().Die();
        }
    }

    private void SetInitialPosition(Vector3Int tilemapSize)
    {
        transform.position = _gameManager.GetSpawnLocationPosition();
    }

    private void SetGameManager()
    {
        GameObject gameManagerObject = transform.parent.gameObject;
        _gameManager = gameManagerObject.GetComponent<GameManager>();
    }
}

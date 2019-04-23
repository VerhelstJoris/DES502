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
    [SerializeField] [Range(0, 1)] [Tooltip("Speed multiplier to use when moving the oil. This should be extremely small.")]
    private float _moveSpeedMultiplier = 0.5f;

    [SerializeField]
    private AudioClip _spawnClip;

    [Header("Camera Shake")]
    [SerializeField] [Range(0, 2)]
    [Tooltip("Max distance for camera shake from origin.")]
    private float _cameraShakeIntensity = 0.5f;
    [SerializeField] [Range(0, 2)]
    [Tooltip("Max distance for camera shake from origin.")]
    private float _cameraShakeDuration = 0.25f;

    const int TILE_SIZE = 250;

    private Vector3Int _tilemapSize;
    private OilBody _oilBody;
    private GameManager _gameManager;
    private CameraShake _cameraShake;
    private AudioSource _source;

    void Awake()
    {
        _oilBody = _oilBodyGameObject.GetComponent<OilBody>();
        SetGameManager();
        _cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
        _source = this.GetComponent<AudioSource>();
    }

    void Start()
    {
        _tilemapSize = GetTilemapSize();
        //Debug.Log("_tilemapSize: " + _tilemapSize.ToString());
        _oilBody.SetSpriteColor(_bodyColor);
        _oilBody.SetSize(_tilemapSize.x, _tilemapSize.y);
        SetInitialPosition(_tilemapSize);
        StartCoroutine(MoveOil(_gameManager.GetOilMaxHeightPosition(), _moveSpeedMultiplier));
    }

    private Vector3Int GetTilemapSize()
    {
        //Tilemap tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        Tilemap tilemap= FindObjectOfType<Tilemap>();
        return tilemap.size;
    }

    public void OnChildTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            other.GetComponent<CharacterController>().Die(CauseOfDeath.Oil);
            _cameraShake.BeginShake(_cameraShakeIntensity, _cameraShakeDuration);
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

    private IEnumerator MoveOil(Vector3 targetPosition, float speedMultiplier)
    {
        Vector3 startPosition = transform.position;
        float percentTravelled = 0;
        while (percentTravelled < 1)
        {
            // increment percent travelled, clamping it to max lerp weight value
            percentTravelled = Mathf.Min(percentTravelled + (Time.deltaTime * speedMultiplier), 1);
            //Debug.Log("Percent of Oil travelled: " + percentTravelled.ToString());
            // lerp position based on percent travelled
            transform.position = Vector3.Lerp(transform.position, targetPosition, percentTravelled);
            yield return null;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    [SerializeField]
    private RuntimeAnimatorController[] _rabbitAnimators, _foxAnimators;

    [HideInInspector]
    public GameManager _GM;
    [HideInInspector]
    public bool _Active = false;
    private Animator _animator;
    private PlayerData _playerData;
    private SpriteRenderer _renderer;

    void Awake()
    {
        _GM = GameObject.Find("GameManager").GetComponent<GameManager>();
        _animator = this.GetComponent<Animator>();
        _renderer = this.GetComponent<SpriteRenderer>();
        _renderer.enabled = true;
    }

    // Start is called before the first frame update
    void Start()
    {
    
    }

    public void Activate(PlayerData data)
    {
        _playerData = data;
        if(data.charID == CharacterID.Fox)
        {
            _animator.runtimeAnimatorController = _foxAnimators[data.skinID];
        }
        else
        {
            _animator.runtimeAnimatorController = _rabbitAnimators[data.skinID];
        }
        _renderer.enabled = true;
        _Active = true;
        //_animator.Play("Respawn",-1,0);
        _animator.SetBool("Active", true);
    }

    public void RespawnFinished()
    {
        _renderer.enabled = false;
        _Active = false;
        _animator.SetBool("Active", false);
    }

    public void RespawnPlayer()
    {
        ObjectFactory.CreatePlayer(_playerData, this.transform.position, _GM._TeamSetup);
    }

    public bool IsCloseToOil(float maxDistance)
    {
        // TODO: replace this with getting the layer bit from name instead of hard coding the value
        int hazardsLayer = 9;
        int layerMask = 1 << hazardsLayer;
        Collider2D[] overlappedHazards = Physics2D.OverlapCircleAll(transform.position, maxDistance, layerMask);
        foreach (Collider2D h in overlappedHazards)
        {
            if (h.gameObject.tag == "Oil")
            {
                return true;
                // we only care about oil, break early
            }
        }
        return false;
    }
}

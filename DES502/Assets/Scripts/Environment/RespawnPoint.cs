using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnPoint : MonoBehaviour
{
    private Animator _animator;
    [HideInInspector]
    public GameManager _GM;
 

    [SerializeField]
    private RuntimeAnimatorController[] _animators;

    public bool _Active = false;

    private PlayerData _playerData;
    private SpriteRenderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        
        _animator = this.GetComponent<Animator>();
        _renderer = this.GetComponent<SpriteRenderer>();
        _renderer.enabled = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    public void Activate(PlayerData data)
    {
        _playerData = data;
        
        _animator.runtimeAnimatorController = _animators[data.skinID];
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

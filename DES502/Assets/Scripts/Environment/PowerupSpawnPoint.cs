﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupSpawnPoint : MonoBehaviour
{
    [Range(0f, 1f)] [Tooltip("Transparency percent to use for gizmo sprite, in 0-1 space.")]
    public float _spriteTransparency = 0.7f;
    private SpriteRenderer spriteRenderer;
    [HideInInspector]
    public bool _containsPowerup = false;

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

    public void OnChildPowerupCollected()
    {
        _containsPowerup = false;
    }
}

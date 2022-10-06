using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[DisallowMultipleComponent]
public class MaterialBushHideController : MonoBehaviour, IBushHide
{
    [SerializeField] private Renderer _meshRenderer;
    private static readonly int Transparency = Shader.PropertyToID("_Transparency");
    private void Reset()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
    }


    public void SetHidePercentage(float percentage)
    {
        foreach (var material in _meshRenderer.materials)
        {
            material.SetFloat(Transparency, Mathf.Abs(percentage - 1f));
        }

        if (percentage > 0.999f)
        {
            if (_meshRenderer.enabled)
            {
                _meshRenderer.enabled = false;
            }
        }
        else
        {
            if (!_meshRenderer.enabled)
            {
                _meshRenderer.enabled = true;
            };
        }
    }
}

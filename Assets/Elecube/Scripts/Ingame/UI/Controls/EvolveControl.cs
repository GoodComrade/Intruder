using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolveControl : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void SetEvolveProgress(float progress)
    {
        _animator.SetFloat("progress", progress);
    }

    public void SetEnabled(bool on)
    {
        gameObject.SetActive(on);
    }
}

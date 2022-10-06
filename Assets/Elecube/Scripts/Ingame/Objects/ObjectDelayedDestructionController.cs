using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDelayedDestructionController : MonoBehaviour
{
    [SerializeField] private float _destructionDelay = 7;

    private void Start()
    {
        Invoke(nameof(Destroy), _destructionDelay);
    }

    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}

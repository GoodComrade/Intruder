using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    protected static SceneController _instance;

    protected void Awake()
    {
        _instance = this;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayUiController : MonoBehaviour
{
    private static GameplayUiController _instance;

    [SerializeField] private ControlsUIController _playerControls;
    private MonsterHealthProgressController _monsterHealthProgressController;
    private SpaceshipProgressController _spaceshipProgressController;

    private void Awake()
    {
        _instance = this;
        _monsterHealthProgressController = GetComponentInChildren<MonsterHealthProgressController>();
        _spaceshipProgressController = GetComponentInChildren<SpaceshipProgressController>();
    }

    public static GameplayUiController GetInstance()
    {
        return _instance;
    }
    public MonsterHealthProgressController GetMinsterHealthProgressVisual()
    {
        return _monsterHealthProgressController;
    }
    public SpaceshipProgressController GetSpaceshipProgressVisual()
    {
        return _spaceshipProgressController;
    }
    public ControlsUIController GetControls()
    {
        return _playerControls;
    }
}

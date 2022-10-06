using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayInputManager : Singleton<GameplayInputManager>
{
    private InputMaster _inputMaster;

    protected override void OnAwake()
    {
        _inputMaster = new InputMaster();
        _inputMaster.Enable();
    }
    
    public NetworkInputMaster GetNetworkInput()
    {
        return new NetworkInputMaster
        {
            move = _inputMaster.Player.Move.ReadValue<Vector2>(),
            aim = _inputMaster.Player.Aim.ReadValue<Vector2>(),
            shoot = _inputMaster.Player.Shoot.ReadValue<float>()> 0.5f,
            ability1 = _inputMaster.Player.Ability1.ReadValue<float>() > 0.5f,
            ability2 =  _inputMaster.Player.Ability2.ReadValue<float>() > 0.5f,
            ability3 =  _inputMaster.Player.Ability3.ReadValue<float>() > 0.5f,
            evolve =  _inputMaster.Player.Evolve.ReadValue<float>() > 0.5f
        };
    }
}

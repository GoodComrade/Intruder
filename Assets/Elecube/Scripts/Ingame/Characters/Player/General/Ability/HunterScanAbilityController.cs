using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HunterScanAbilityController : PlayerAbilityController
{
    [SerializeField] private GameObject _alertPrefab;
    [SerializeField] private GameObject _scanEffectPrefab;
    
    public override void DoAbility()
    {
        base.DoAbility();
        RPC_DoScan();
        foreach (var monster in CharactersController.GetInstance().GetMonsterPlayerObjects())
        {
            foreach (var pos in monster.GetAlertPositions())
            {
                Instantiate(_alertPrefab, pos, Quaternion.identity);
            }
        }
    }
    
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_DoScan(RpcInfo info = default)
    {
        Instantiate(_scanEffectPrefab, _intruderCharacterController.GetNetworkTransform().ReadPosition(), Quaternion.identity);
    }
}

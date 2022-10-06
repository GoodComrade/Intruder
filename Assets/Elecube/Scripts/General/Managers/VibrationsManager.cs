using System.Collections;
using System.Collections.Generic;
using Fusion;
using MoreMountains.NiceVibrations;
using UnityEngine;

public class VibrationsManager : Singleton<VibrationsManager>
{
    private const float DamageVibrationInterval = 0.25f;

    private double _lastDamageVibrationTime;
    
    public void VibrateStopAiming()
    {
        MMVibrationManager.TransientHaptic(0.5f, 0.7f);
    }

    public void ResolveVibrateCharacterHit(IntruderCharacterController source, IntruderCharacterController _character)
    {
        //workaround because of persistent _lastDamageVibrationTime
        if(_lastDamageVibrationTime > Time.timeSinceLevelLoad)
            _lastDamageVibrationTime = 0;
        if(_lastDamageVibrationTime + DamageVibrationInterval > Time.timeSinceLevelLoad)
            return;
        if (source.GetSide() != CharacterSide.PLAYER)
            return;
        //friendly fire
        if (!GameController.GetInstance().IsLocalPlayerMonster() && _character.GetCharacter().GetCharacterType() == CharacterType.WORKER)
            return;
        
        VibrateEnemyHit();
    }

    private void VibrateEnemyHit()
    {
        MMVibrationManager.TransientHaptic(1f, 0.7f);
        _lastDamageVibrationTime = Time.timeSinceLevelLoad;
    }
}

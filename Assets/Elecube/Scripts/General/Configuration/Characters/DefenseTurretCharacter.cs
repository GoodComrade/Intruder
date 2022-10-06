using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseTurretCharacter : Character
{
    public override CharacterType GetCharacterType()
    {
        return CharacterType.TURRET;
    }
}
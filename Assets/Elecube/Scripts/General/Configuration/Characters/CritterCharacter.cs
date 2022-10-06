using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Intruder/Character/Critter")]
public class CritterCharacter : Character
{
    public override CharacterType GetCharacterType()
    {
        return CharacterType.CRITTER;
    }
}

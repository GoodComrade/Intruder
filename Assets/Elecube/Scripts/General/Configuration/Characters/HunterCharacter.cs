using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Intruder/Character/Hunter")]
public class HunterCharacter : PlayerCharacter
{
    public override CharacterType GetCharacterType()
    {
        return CharacterType.HUNTER;
    }
}

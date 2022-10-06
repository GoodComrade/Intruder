using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpaceshipCharacter : Character
{
    public override CharacterType GetCharacterType()
    {
        return CharacterType.SPACESHIP;
    }
}
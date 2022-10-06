
using UnityEngine;

[CreateAssetMenu(menuName = "Intruder/Character/Monster")]
public class MonsterCharacter : PlayerCharacter
{
    public override CharacterType GetCharacterType()
    {
        return CharacterType.MONSTER;
    }
}

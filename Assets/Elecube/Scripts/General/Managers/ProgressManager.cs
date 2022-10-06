using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressManager : Singleton<ProgressManager>
{
    private bool _currentTeamIsMonster = true;

    public int GeCurrentCharacterPoints(Character character)
    {
        return 5;
    }

    public int GetPointsForUpgrade(int lvl)
    {
        return 10 * lvl;
    }

    public int GetCurrentCharacterLevel(Character character)
    {
        return 1;
    }

    public MonsterCharacter GetCurrentMonster()
    {
        return Characters.GetInstance().GetMonsters()[0];
    }
    
    public HunterCharacter GetCurrentHunter()
    {
        return Characters.GetInstance().GetHunters()[0];
    }
    
    public PlayerCharacter GetCurrentCharacter()
    {
        return GetCurrentTeamIsMonster() ? GetCurrentMonster() : GetCurrentHunter();
    }

    public void SetCurrentTeam(bool monster)
    {
        _currentTeamIsMonster = monster;
    }

    public bool GetCurrentTeamIsMonster()
    {
        return _currentTeamIsMonster;
    }
}

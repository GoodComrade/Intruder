using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Characters : ScriptableObject
{
    private static Characters _instance;

    [SerializeField] private SpaceshipCharacter _spaceship;
    [SerializeField] private DefenseTurretCharacter _defenseTurret;
    [SerializeField] private WorkerCharacter _worker;
    [SerializeField] private List<MonsterCharacter> _monsters;
    [SerializeField] private List<HunterCharacter> _hunters;
    [SerializeField] private List<CritterCharacter> _critters;

    public List<MonsterCharacter> GetMonsters()
    {
        return _monsters;
    }
    
    public List<HunterCharacter> GetHunters()
    {
        return _hunters;
    }

    public WorkerCharacter GetWorker()
    {
        return _worker;
    }

    public SpaceshipCharacter GetSpaceship()
    {
        return _spaceship;
    }
    
    public DefenseTurretCharacter GetDefenseTurret()
    {
        return _defenseTurret;
    }

    public Character GetCharacterById(ushort id)
    {
        if (_worker.GetCharacterId() == id)
            return _worker;
        if (_spaceship.GetCharacterId() == id)
            return _spaceship;
        if (_defenseTurret.GetCharacterId() == id)
            return _defenseTurret;
        foreach (var hunter in _hunters)
        {
            if (hunter.GetCharacterId() == id)
                return hunter;
        }
        foreach (var monster in _monsters)
        {
            if (monster.GetCharacterId() == id)
                return monster;
        }
        foreach (var critter in _critters)
        {
            if (critter.GetCharacterId() == id)
                return critter;
        }
        throw new Exception("Unknown character id - " + id);
    }

    public ushort GenerateCharacterId()
    {
        ushort id = 0;
        while (true)
        {
            id++;
            bool isDuplicate = false;
            if (_worker.GetCharacterId() == id)
                continue;
            if (_spaceship.GetCharacterId() == id)
                continue;
            if (_defenseTurret.GetCharacterId() == id)
                continue;
            
            foreach (var hunter in _hunters)
            {
                if (hunter.GetCharacterId() == id)
                {
                    isDuplicate = true;
                    continue;
                }
            }
            foreach (var monster in _monsters)
            {
                if (monster.GetCharacterId() == id)
                {
                    isDuplicate = true;
                    continue;
                }
            }
            foreach (var critter in _critters)
            {
                if (critter.GetCharacterId() == id)
                {
                    isDuplicate = true;
                    continue;
                }
            }
            if(isDuplicate)
                continue;
            return id;
        }
    }
    
    
    public static Characters GetInstance()
    {
        if (_instance == null)
        {
            var operation = Addressables.LoadAssetAsync<Characters>(
                "Assets/Elecube/Configuration/Characters/Characters.asset");
            _instance = operation.WaitForCompletion();
        }

        return _instance;
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEngine;


[OrderBefore(typeof(GameController))]
public class CharactersController : NetworkBehaviour
{
    private static CharactersController _instance;
    
    [Networked]
    [Capacity(6)] 
    private NetworkLinkedList<PlayerWrapperController> playerObjects { get; }
    private readonly List<IntruderCharacterController> _allCharacters = new List<IntruderCharacterController>();
    private PlayerWrapperController _localPlayer;
    private void Awake()
    {
        _instance = this;
    }

    public static CharactersController GetInstance()
    {
        return _instance;
    }

    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    public void RPC_AddPlayer(PlayerWrapperController player, RpcInfo info = default)
    {
        playerObjects.Add(player);
    }

    public List<IntruderCharacterController> GetAllCharacters()
    {
        return _allCharacters;
    }

    public void CheckForFallenCharacters()
    {
        for (int i = _allCharacters.Count -1; i >= 0; i--)
        {
            if (_allCharacters[i].GetCurrentHex().IsDestroyed())
            {
                _allCharacters[i].FallFromIsland();
            } 
            else if (_allCharacters[i] is CritterCharacterController controller && controller.GetCritterHex().IsDestroyed())
            {
                controller.FallFromIsland();
            }
        }
    }

    public void AddCharacter(IntruderCharacterController intruderCharacterController)
    {
        _allCharacters.Add(intruderCharacterController);
    }

    public void RemoveCharacter(IntruderCharacterController intruderCharacterController)
    {
        _allCharacters.Remove(intruderCharacterController);
    }
    
    public PlayerWrapperController GetPlayerObject(PlayerRef player)
    {
        foreach (var p in playerObjects)
        {
            if (p.Object.InputAuthority == player)
                return p;
        }
        throw new Exception("Unknown player char - " + player.PlayerId);
    }
    
    public List<PlayerWrapperController> GetMonsterPlayerObjects()
    {
        return playerObjects.ToList().FindAll(controller =>
            controller.GetCharacter().GetCharacterType() == CharacterType.MONSTER);
    }

    public bool IsPlayerMonster(PlayerRef player)
    {
        foreach (var monster in GetMonsterPlayerObjects())
        {
            if (monster.GetControllingPlayer() == player)
            {
                return true;
            }
        }
        return false;
    }

    public void SetLocalPlayer(PlayerWrapperController player)
    {
        _localPlayer = player;
    }

    public PlayerWrapperController GetLocalPlayer()
    {
        return _localPlayer;
    }
}

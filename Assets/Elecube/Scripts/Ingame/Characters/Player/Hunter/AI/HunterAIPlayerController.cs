using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HunterAIPlayerController : AIPlayerController
{
    private TickTimer _decisionTimer;

    protected override void DoDecision()
    {
        base.DoDecision();
    }

    protected override void DoMovementDecision()
    {
        base.DoMovementDecision();
        SetDestination(Vector3.MoveTowards(GetEnemyPlayers()[0].GetAlertPositions()[0], 
            _intruderCharacter.GetNetworkTransform().ReadPosition(), Random.Range(4f,7f))
        + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)));
    }

    protected override List<PlayerWrapperController> GetEnemyPlayers()
    {
        return CharactersController.GetInstance().GetMonsterPlayerObjects();
    }
}

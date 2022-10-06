using System.Numerics;
using Fusion;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;



[OrderAfter(typeof(GameController))]
public class CritterCharacterController : IntruderCharacterController
{
    private const float CritterFindTargetRange = 17f;
    [SerializeField] private CritterCharacter _character;

    [Networked] private HexController _critterHex { get; set; }

    private float _updateTargetTime;
    private CritterMovementController _movementController;
    private bool _hasTarget;
    private IntruderCharacterController _target;
    private Vector3 _startPosition;

    public void InitCritterNetworkState(HexController hex)
    {
        _critterHex = hex;
    }
    
    protected override void Awake()
    {
        base.Awake();
        _movementController = GetComponent<CritterMovementController>();
    }

    public override void Spawned()
    {
        base.Spawned();
        _startPosition = _critterHex.GetIsland().GetMonsterSpawnPoint();
        _movementController.TeleportToPosition(_startPosition);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(_dead)
            return;
        if(_updateTargetTime + (HasTarget() ? 1.5f : 0.66f) < Runner.SimulationTime)
            TargetFindingUpdate();
    }

    private void TargetFindingUpdate()
    {
        _updateTargetTime = Runner.SimulationTime;
        if (_hasTarget)
        {
            if(IsTargetValid(_target))
                return;
            TargetRemoved();
        }
        _hasTarget = FindTarget(out _target);
        if (_hasTarget)
        {
            NewTargetFound();
        }
    }

    private void TargetRemoved()
    {
        _target.RemoveOnDeathAction(TargetDied); 
        _target.SetAttackedByCritter(this, false);
    }

    private void NewTargetFound()
    {
        _target.AddOnDeathAction(TargetDied);
        _target.SetAttackedByCritter(this, true);
    }

    private void TargetDied(IntruderCharacterController target)
    {
        _hasTarget = false;
    }

    private bool FindTarget(out IntruderCharacterController target)
    {
        foreach (var character in CharactersController.GetInstance().GetAllCharacters())
        {
            if(!IsTargetValid(character))
                continue;
            if (character.GetCharacter().GetCharacterType() == CharacterType.HUNTER)
            {
                target = character;
                return true;
            }
            if (character.GetCharacter().GetCharacterType() == CharacterType.WORKER)
            {
                target = character;
                return true;
            }
        }
        target = null;
        return false;
    }

    private bool IsTargetValid(IntruderCharacterController character)
    {
        if(character.IsDead())
            return false;
        if(character.GetCurrentHex() != _critterHex)
            return false;
        if (Vector3.Distance(character.GetNetworkTransform().ReadPosition(), GetNetworkTransform().ReadPosition()) >
            CritterFindTargetRange)
            return false;
        return true;
    }
    
    public Vector3 GetStartPosition()
    {
        return _startPosition;
    }

    public bool HasTarget()
    {
       return _hasTarget;
    }

    public IntruderCharacterController GetTarget()
    {
        return _target;
    }
    
    public override Character GetCharacter()
    {
        return _character;
    }

    public HexController GetCritterHex()
    {
        return _critterHex;
    }
}

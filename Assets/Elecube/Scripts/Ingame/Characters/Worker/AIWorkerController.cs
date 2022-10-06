using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;
using Vector3 = UnityEngine.Vector3;

[OrderAfter(typeof(GameController))]
public class AIWorkerController : WorkerCharacterController, ICanChangeHealth
{
    private float currentStateStart { get; set; }
    private CrystalMiningSpot _currentCrystalTarget;
    private Vector3 _currentCollectionPoint;
    private AiWorkerMovementController _workerMovementController;
    private TickTimer _decideNextActionTimer;
    private short _crystalsMined;
    private IntruderCharacterController _fightTarget;

    protected override void Awake()
    {
        base.Awake();
        _workerMovementController = GetComponent<AiWorkerMovementController>();
        transform.SetParent(WorkersController.GetInstance().transform, true);
    }

    public override void Spawned()
    {
        base.Spawned();
        if (!Object.HasStateAuthority)
            return;
        
        _decideNextActionTimer = TickTimer.CreateFromSeconds(Runner,Random.Range(0f, 0.5f));
        WorkersController.GetInstance().AddWorker(this.GetComponent<AIWorkerController>());
    }

    private void DecideNextAction()
    {
        StopCurrentState();

        if (Random.Range(0, 100) < 84 && GoMineCrystal())
        {
            //going to mine
        }
        else
        {
            GoRecon();
        }
        
        _decideNextActionTimer = TickTimer.CreateFromSeconds(Runner,Random.Range(1f, 10f));
    }

    //added for randomness
    private void CheckRedecideAction()
    {
        if(CurrentState is WorkerState.MiningCrystal or WorkerState.ComingToCrystal or WorkerState.Fight)
            return;
        if(_decideNextActionTimer.ExpiredOrNotRunning(Runner))
            DecideNextAction();
    }

    public void ScareFromPosition(Vector3 scarePosition)
    {
        RPC_DoScare(scarePosition, GameplayConstants.MonsterScareDuration);
    }
    
    public void ScareToPosition(Vector3 scarePosition)
    {
        RPC_DoScare(GetNetworkTransform().ReadPosition() + (GetNetworkTransform().ReadPosition() - scarePosition).normalized * 10f, GameplayConstants.IslandDestructionTime);
    }
    
    [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
    private void RPC_DoScare(Vector3 scarePosition, float duration, RpcInfo info = default)
    {
        if(CurrentState == WorkerState.Fight)
            return;
        StopCurrentState();
        SetCurrentState(WorkerState.Scared);
        _workerMovementController.RunFromPosition(scarePosition);
        _decideNextActionTimer = TickTimer.CreateFromSeconds(Runner, duration);
    }

    private void StartFight(IntruderCharacterController _target)
    {
        StopCurrentState();
        SetCurrentState(WorkerState.Fight);
        _fightTarget = _target;
        _workerMovementController.GoToDestination(Vector3.Lerp(GetNetworkTransform().ReadPosition(),
            _target.GetNetworkTransform().ReadPosition(), 0.5f));
    }

    public override void SetAttackedByCritter(CritterCharacterController critter, bool on)
    {
        base.SetAttackedByCritter(critter, on);
        if (on && CurrentState != WorkerState.Fight)
        {
            StartFight(critter);
        }
        else if (!on)
        {
           StopFight(); 
        }
    }

    private void StopCurrentState()
    {
        switch (CurrentState)
        {
            case WorkerState.MiningCrystal:
                StopMiningCrystal();
                break;
            case WorkerState.ComingToCrystal:
                StopComingToCrystal();
                break;
            case WorkerState.Scared:
                StopScare();
                break;
        }
    }

    private void StopScare()
    {
        SetCurrentState(WorkerState.Idle);
        _workerMovementController.SetRunning(false);
    }

    private void ScareUpdate()
    {
        
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(CurrentState == WorkerState.Dead)
            return;
        if (!Object.HasStateAuthority)
            return;

        switch (CurrentState)
        {
            case WorkerState.MiningCrystal:
                MiningCrystalUpdate();
                break;
            case WorkerState.ComingToCrystal:
                ComingToCrystalUpdate();
                break;
            case WorkerState.Scared:
                ScareUpdate();
                break;
            case WorkerState.Fight:
                FightUpdate();
                break;
        }

        CheckRedecideAction();
    }

    protected override void SetCurrentState(WorkerState state)
    {
        base.SetCurrentState(state);
        currentStateStart = Runner.SimulationTime;
    }

    private void ComingToCrystalUpdate()
    {
        if (Vector3.Distance(_currentCrystalTarget.transform.position, GetMovingTransform().position) <
                 GameplayConstants.MiningRange)
        {
            StartMiningCrystal();
        }
    }

    private void MiningCrystalUpdate()
    {
        DoRevealAction();
        if (GetCurrentStateDuration() > GameplayConstants.MiningTime)
        {
            CollectCrystal();
            _decideNextActionTimer = TickTimer.CreateFromSeconds(Runner,Random.Range(0f, 1f));
        }
    }

    private float _lastFightUpdate;
    private void FightUpdate()
    {
        if(_lastFightUpdate + 1.213f > Runner.SimulationTime)
            return;
        _lastFightUpdate = Runner.SimulationTime;
        if (_fightTarget == null || _fightTarget.IsDead())
        {
            StopFight();
        }
        else
        {
            //_workerMovementController.RotateToTarget(_fightTarget.GetNetworkTransform().ReadPosition());   
        }
    }

    private void StopFight()
    {
        SetCurrentState(WorkerState.Idle);
        _decideNextActionTimer = TickTimer.CreateFromSeconds(Runner,Random.Range(0f, 1.2f));
    }

    private void StopComingToCrystal()
    {
        _currentCrystalTarget.UnsetWorker();
        SetCurrentState(WorkerState.Idle);
        _decideNextActionTimer = TickTimer.CreateFromSeconds(Runner,Random.Range(0f, 1.2f));
    }

    private void StartMiningCrystal()
    {
        SetCurrentState(WorkerState.MiningCrystal);
        _workerMovementController.FinishDestinationAndStop();
        _animationsController.SetMiningCrystal(true);
    }

    private void StopMiningCrystal()
    {
        _animationsController.SetMiningCrystal(false);
        _currentCrystalTarget.UnsetWorker();
        _currentCrystalTarget = null;
        SetCurrentState(WorkerState.Idle);
    }

    private void CollectCrystal()
    {
        _crystalsMined++;
        _currentCrystalTarget.MiningFinished();
        StopMiningCrystal();
    }

    private bool GoMineCrystal()
    {
        if(CrystalsController.GETInstance().GetCrystalToAIMine(this, out _currentCrystalTarget, GetMaxMineRange()))
        {
            _currentCrystalTarget.SetComingWorker(this);
            _workerMovementController.GoToDestination(_currentCrystalTarget.transform.position);
            SetCurrentState(WorkerState.ComingToCrystal);
            return true;
        }

        return false;
    }

    private float GetMaxMineRange()
    {
        if (_crystalsMined % 10 == 0)
            return float.PositiveInfinity;

        return Random.Range(3, (5 * (_crystalsMined % 10)));
    }
    
    private void GoRecon()
    {
        _workerMovementController.GoToDestination(GetMovingTransform().position +
                                                  new Vector3(Random.Range(0f, 30f), 0, Random.Range(0f, 30f)));
        SetCurrentState(WorkerState.Recon);
    }
    private float GetCurrentStateDuration()
    {
        return Runner.SimulationTime - currentStateStart;
    }

    public override void InvokeDeath(PlayerRef source)
    {
        base.InvokeDeath(source);
        CrystalgrabGameController.GetCrystalgrabInstance().WorkerDied(this, source);
    }

    public override void Die()
    {
        WorkersController.GetInstance().RemoveWorker(this);
        StopCurrentState();
        base.Die();
    }

    public void CrystalTargetDestroyed()
    {
        StopCurrentState();
    }

    public AiWorkerMovementController GetMovementController()
    {
        return _workerMovementController;
    }

    public void HealthChanged(HealthController healthController, float delta)
    {
        if (delta < 0)
        {
            if(Object.HasStateAuthority && CurrentState is not WorkerState.Scared or WorkerState.Fight)
                ScareFromPosition(GetMovingTransform().position + new Vector3(Random.Range(-1f, 1f),0, Random.Range(-1f, 1f)));
        }
    }
}
using System;
using Fusion;
using UnityEngine;
using UnityEngine.AI;
public class AiWorkerMovementController : CharacterMovementController, ICanDie
{
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _runSpeed;
    [SerializeField] private NavMeshAgent _navMeshAgent;
    
    private bool _disabled = false;
    private bool _running = false;

    public override void Spawned()
    {
        Initialise();
    }
    
    protected override void SetEnginePosition(Vector3 pos)
    {
        base.SetEnginePosition(pos);
        _navMeshAgent.Warp(pos);
    }
    
    private void Initialise()
    {
        _navMeshAgent.Warp(transform.position);
    }
    
    private void SetDestination(Vector3 destination)
    {
        _navMeshAgent.SetDestination(destination);
    }
    
    public void GoToDestination(Vector3 destination)
    {
        SetRunning(false);
        SetDestination(destination);
    }

    public void RunFromPosition(Vector3 position)
    {
        SetRunning(true);
        SetDestination(_navMeshAgent.transform.position +
                       ((_navMeshAgent.transform.position - position).normalized * 30));
    }

    public void SetRunning(bool on)
    {
        _running = on;
        SpeedModified();
    }
    public void FinishDestinationAndStop()
    {
        SetDestination(_navMeshAgent.destination);
    }
    
    public void Stop()
    {
        SetDestination(_navMeshAgent.transform.position);
    }

    public void RotateToTarget(Vector3 target)
    {
        transform.localEulerAngles = Vector3.RotateTowards(ReadPosition(), target, 90, 90);
    }

    public void Die()
    {
        _navMeshAgent.enabled = false;
    }
    
    public override bool IsMoving()
    {
        return _navMeshAgent.desiredVelocity.sqrMagnitude > 0.001f;
    }
    public override void SetMovementDisabled(bool disabled)
    {
        _disabled = disabled;
    }

    protected override void SpeedModified()
    {
        _navMeshAgent.speed = (_running ? _runSpeed : _walkSpeed) * GetSpeedModification();
    }

    public bool CanGoTo(Vector3 position)
    {
        bool canNavigate = _navMeshAgent.CalculatePath(position, new NavMeshPath());
        return canNavigate;
    }
}

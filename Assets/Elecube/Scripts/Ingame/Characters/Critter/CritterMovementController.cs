using Fusion;
using UnityEngine;
using UnityEngine.AI;

public class CritterMovementController : NetworkTransform, ICanDie
{
    [SerializeField] private float _followDistance;
    [SerializeField][Range(0.02f, 1f)] private float _updateFollowInterval;
    
    private NavMeshAgent _agent;
    private CritterCharacterController _critterCharacter;
    private float _lastUpdateFollowTime;
    private bool _dead;

    protected override void Awake()
    {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
        _critterCharacter = GetComponent<CritterCharacterController>();
    }

    protected override void SetEnginePosition(Vector3 pos)
    {
        base.SetEnginePosition(pos);
        _agent.Warp(pos);
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if(_dead)
            return;
        if (_lastUpdateFollowTime + _updateFollowInterval < Runner.SimulationTime)
        {
            _lastUpdateFollowTime = Runner.SimulationTime;
            if (_critterCharacter.HasTarget())
            {
                FollowTarget(_critterCharacter.GetTarget());
            }
            else
            {
                ReturnToStart();
            }   
        }
    }

    private void ReturnToStart()
    {
        _agent.SetDestination(_critterCharacter.GetStartPosition());
    }

    private void FollowTarget(IntruderCharacterController _target)
    {
        _agent.SetDestination(Vector3.MoveTowards(_target.GetNetworkTransform().ReadPosition(), 
            _critterCharacter.GetNetworkTransform().ReadPosition(), _followDistance));
    }

    private void Stop()
    {
        _agent.enabled = false;
    }

    public void Die()
    {
        _dead = true;
        Stop();
    }
}

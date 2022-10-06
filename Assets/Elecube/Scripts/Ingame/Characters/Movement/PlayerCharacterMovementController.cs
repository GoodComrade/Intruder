using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[DisallowMultipleComponent]
public class PlayerCharacterMovementController : CharacterMovementController, IPersonDisable, ICanDie
{ 
    public float acceleration = 10.0f;
    public float maxSpeed = 2.0f;
    public float rotationSpeed = 15.0f;
    [Networked] [HideInInspector] public Vector3 Velocity { get; set; }
 
    private bool _disabled;
    private bool _moving;
    private NavMeshAgent _agent;
    private float _speedModifier = 1.0f;

    protected override void Awake()
    {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
    }
    
    protected virtual void Start()
    {
        _agent.Warp(transform.position);
        _agent.acceleration = acceleration;
        _agent.speed = maxSpeed;
        _agent.angularSpeed = rotationSpeed * 180;
    }

    private void ProcessInput()
    {
        if (GetInput(out NetworkInputMaster data))
        {
            if (data.move != Vector2.zero)
            {
                Move(new Vector3(data.move.x, 0, data.move.y).normalized);
                _moving = true;
            }
            else
            {
                Stop();
            }
        }
    }
    protected virtual void Move(Vector3 direction)
    {
        var deltaTime = Runner.DeltaTime;
        var previousPos = transform.position;
        var moveVelocity = Velocity;

        direction.y = 0;
        direction = direction.normalized;

        var horizontalVel = default(Vector3);
        horizontalVel.x = moveVelocity.x;
        horizontalVel.z = moveVelocity.z;

        if (direction == default)
        {
            horizontalVel = Vector3.Lerp(horizontalVel, default, acceleration * deltaTime);
        }
        else
        {
            horizontalVel = Vector3.ClampMagnitude(horizontalVel + direction * acceleration * deltaTime, maxSpeed * _speedModifier);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction),
                rotationSpeed * Runner.DeltaTime);
        }

        moveVelocity.x = horizontalVel.x;
        moveVelocity.z = horizontalVel.z;
        moveVelocity.y = 0;

        _agent.Move(moveVelocity * deltaTime);

        Velocity = (transform.position - previousPos) * Runner.Simulation.Config.TickRate;
    }

    private void Stop()
    {
        Velocity = Vector3.zero;
        _moving = false;
    }

    public override void FixedUpdateNetwork()
    {
        base.FixedUpdateNetwork();
        if (_disabled)
            return;
        if (Object.HasStateAuthority)
        {
            ProcessInput();
        }
    }

    protected override void SetEnginePosition(Vector3 pos)
    {
        base.SetEnginePosition(pos);
        _agent.Warp(pos);
    }

    public void TeleportToClosestLand()
    {
        
    }

    public void SetPosition(Vector3 position)
    {
        _agent.Warp(position); 
    }

    public void SetAiDestination(Vector3 destination)
    {
        _agent.SetDestination(destination);
    }

    public void PersonDisable()
    {
        _disabled = true;
        _agent.enabled = false;
    }

    public void PersonEnable()
    {
        _disabled = false;
        _agent.enabled = true;
    }
    public void Die()
    {
        Stop();
        _agent.enabled = false;
        _disabled = true;
    }

    public override bool IsMoving()
    {
        return _moving && !_disabled;
    }

    public override void SetMovementDisabled(bool disabled)
    {
        if (disabled)
        {
            Stop();
        }
        _disabled = disabled;
    }

    protected override void SpeedModified()
    {
        _speedModifier = GetSpeedModification();
    }
}
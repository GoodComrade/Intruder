using System.Collections;
using System.Collections.Generic;
using Fusion;
using JetBrains.Annotations;
using UnityEngine;

/// <summary>
/// Contains the logic of scaring off workers by monster.
/// </summary>
public class MonsterScareController : NetworkBehaviour, IPersonDisable, IBushHide, ICanSetCharacter
{
    private const float SCARE_INTERVAL = 1f;

    [SerializeField] private LayerMask _detectionLayerMask;

    private bool _hiddenInBush;
    private bool _disabled;
    private TickTimer _scareTimer;

    private Coroutine _personEnableRoutine;
    private IntruderCharacterController _intruderCharacterController;

    public override void Spawned()
    {
        base.Spawned();
        _scareTimer = TickTimer.CreateFromSeconds(Runner, SCARE_INTERVAL);
    }

    public override void FixedUpdateNetwork()
    {
        if (_disabled)
            return;
        if (Object.HasStateAuthority && _scareTimer.ExpiredOrNotRunning(Runner))
        {
            _scareTimer = TickTimer.CreateFromSeconds(Runner, SCARE_INTERVAL);
            Scare();
        }
    }

    private void Scare()
    {
        WorkersController.GetInstance().GetWorkers()
            .FindAll(w =>
                Vector3.Distance(w.GetNetworkTransform().ReadPosition(), transform.position) <
                GetScareRange())
            .FindAll(CanSeeWorker).ForEach(controller =>
            {
                controller.ScareFromPosition(transform.position);
            });
    }

    private bool CanSeeWorker(AIWorkerController worker)
    {
        return IntruderHelper.CanAimAtCharacter(_intruderCharacterController, worker, _detectionLayerMask);
    }

    private float GetScareRange()
    {
        if (_hiddenInBush)
        {
           return GameplayConstants.MonsterScareRangeInBush;
        }
        return GameplayConstants.MonsterScareRange;
    }

    public bool IsScaring()
    {
        return !_disabled;
    }

    public void PersonDisable()
    {
        _disabled = true;
        if (_personEnableRoutine != null)
        {
            StopCoroutine(_personEnableRoutine);
            _personEnableRoutine = null;
        }
    }

    public void PersonEnable()
    {
        _personEnableRoutine = StartCoroutine(PersonEnableRoutine());
    }

    private IEnumerator PersonEnableRoutine()
    {
        yield return new WaitForSeconds(0.5f); 
        _disabled = false;
        _personEnableRoutine = null;
    }

    public void SetHidePercentage(float percentage)
    {
        _hiddenInBush = percentage > 0.01f;
    }

    public void SetCharacter(IntruderCharacterController intruderCharacterController)
    {
        _intruderCharacterController = intruderCharacterController;
    }
}
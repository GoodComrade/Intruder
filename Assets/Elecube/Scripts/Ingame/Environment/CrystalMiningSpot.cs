using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEditor;
using UnityEngine;

public class CrystalMiningSpot : MonoBehaviour, IIslandDegradationListener
{
    private AIWorkerController _assignedWorker;
    private bool _hasWorker;

    private CrystalController _crystal;
    
    public void SetComingWorker(AIWorkerController workerController)
    {
        _assignedWorker = workerController;
        _hasWorker = true;
    }

    public void UnsetWorker()
    {
        _hasWorker = false;
    }
    public bool HasWorker()
    {
        return _hasWorker;
    }
    
    public void MiningFinished()
    {
        UnsetWorker();
        _crystal.MiningDone();
    }

    public void SetParentCrystal(CrystalController crystal)
    {
        _crystal = crystal;
    }

    public CrystalController GetCrystal()
    {
        return _crystal;
    }
    
    public void SetIslandDegradationPercentage(float percentage)
    {
        if (percentage >= 1f && _hasWorker)
        {
            _assignedWorker.CrystalTargetDestroyed();
        }
    }

    public void SetMiningLocked(bool @on)
    {
        
    }

#if UNITY_EDITOR
    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    private static void DrawGizmoIcon(CrystalMiningSpot node, GizmoType gizmoType)
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(node.transform.position, 0.15f);
    }
#endif
}

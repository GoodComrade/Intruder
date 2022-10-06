using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using UnityEditor;
using UnityEngine;

public class IslandController : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnMinedCountChanged))] private ushort crystalsMined { get; set; }
    private HexController _hexParent;
    private List<CrystalController> _crystals;

    [SerializeField] private Vector3 _monsterSpawnPosition;

    private bool _miningLocked;
    private List<IIslandDegradationListener> _islandDegradationListeners;

    private void Awake()
    {
        _crystals = GetComponentsInChildren<CrystalController>().ToList();
        foreach (var crystal in _crystals)
        {
            crystal.SetParentIsland(this);
        }
        _islandDegradationListeners = GetComponentsInChildren<IIslandDegradationListener>(true).ToList();
    }

    public void CrystalMined()
    {
        if(crystalsMined < _hexParent.GetCrystalCount())
            crystalsMined++;
    }

    public void SetMiningLocked(bool on)
    {
        _miningLocked = on;
        foreach (var degradationListener in _islandDegradationListeners)
        {
            degradationListener.SetMiningLocked(on);
        }
    }

    public bool IsMiningLocked()
    {
        return _miningLocked;
    }

    private void DestroyIsland()
    {
        foreach (var degradationListener in _islandDegradationListeners)
        {
            degradationListener.SetIslandDegradationPercentage(1f);
        }
        Invoke(nameof(DestroyIslandFinal), GameplayConstants.IslandDestructionTime);
        WorkersController.GetInstance().ScareWorkersOutOfHex(_hexParent);
    }

    private void DestroyIslandFinal()
    {
        Runner.Despawn(Object);
        this.gameObject.SetActive(false);
        _hexParent.IslandDestroyed();
    }

    public void SetParentHex(HexController hexController)
    {
        _hexParent = hexController;
    }
    
    public static void OnMinedCountChanged(Changed<IslandController> changed)
    {
        changed.Behaviour.OnCrystalMinedCountChanged();
    }

    private void OnCrystalMinedCountChanged()
    {
        if (crystalsMined == _hexParent.GetCrystalCount())
        {
            DestroyIsland();
        }
        else
        {
            foreach (var degradationListener in _islandDegradationListeners)
            {
                degradationListener.SetIslandDegradationPercentage((float)crystalsMined / (float)_hexParent.GetCrystalCount());
            }
        }
    }

    public Vector3 GetMonsterSpawnPoint()
    {
        return transform.position + _monsterSpawnPosition;
    }

    public HexController GetHex()
    {
        return _hexParent;
    }   
    
#if UNITY_EDITOR
    [DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    private static void DrawGizmoIcon(IslandController island, GizmoType gizmoType)
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(island.transform.position + island._monsterSpawnPosition, Vector3.one);
    }
#endif
    
}

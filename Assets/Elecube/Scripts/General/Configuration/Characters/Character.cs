using System;
using Fusion;
using UnityEngine;

public abstract class Character : ScriptableObject
{
    [InspectorReadOnlyAttribute][SerializeField] private ushort _characterId;
    
    [SerializeField] private float _characterHealth = 1500;
    public float CharacterHealth => _characterHealth;
    
    [SerializeField] private short _characterDamage = 300;
    public short CharacterDamage => _characterDamage;
    [SerializeField] private NetworkPrefabRef _characterPrefab;
    public NetworkPrefabRef CharacterPrefab => _characterPrefab;
    
    [SerializeField] private GameObject _characterVisualPrefab;
    public GameObject CharacterVisualPrefab => _characterVisualPrefab;
    
    [SerializeField] private short _utilityStat;
    public short UtilityStat => _utilityStat;

    public abstract CharacterType GetCharacterType();

    public ushort GetCharacterId()
    {
        return _characterId;
    }

    #if UNITY_EDITOR
    private void OnEnable()
    {
        if (_characterId == 0)
        {
            Debug.Log("Generating character id - " + this.name);
            _characterId = Characters.GetInstance().GenerateCharacterId();
        }
    }
    #endif
}

public enum CharacterType
{
    HUNTER, MONSTER, WORKER, SPACESHIP, TURRET, CRITTER
}

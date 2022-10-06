using System.Collections.Generic;
using Fusion;
using UnityEngine;


/// <summary>
/// Wrapper controlling an permanent object spawning/destroying player character objects and holding player progress for the whole match.
/// </summary>
public abstract class PlayerWrapperController : NetworkBehaviour
{
    [Networked] protected PlayerRef ControllingPlayer { get; set; }

    [Networked(OnChanged = nameof(OnCharacterIdChange))]
    private ushort CharacterId { get; set; }

    //cached object fetched from CharacterId
    private Character _character;

    [Networked(OnChanged = nameof(OnCharacterControllerChange))]
    protected PlayerCharacterController playerCharacterController { get; set; }

    private bool _characterResolved;
    private HealthController _healthController;


    public void InitNetworkState(PlayerRef player, Character character)
    {
        ControllingPlayer = player;
        CharacterId = character.GetCharacterId();
    }

    protected virtual void Start()
    {
    }

    protected virtual void Awake()
    {
    }

    public override void Spawned()
    {
        base.Spawned();
        if (Object.HasStateAuthority)
            SpawnCharacter();
    }

    protected virtual void SpawnCharacter()
    {
        playerCharacterController = Runner.Spawn(GetCharacter().CharacterPrefab,
            GameController.GetInstance().GetPlayerSpawnPoint(GetCharacter()), Quaternion.Euler(0, 0, 0),
            ControllingPlayer, ((runner, o) => BeforeCharacterSpawn(o))).GetComponent<PlayerCharacterController>();
    }

    protected virtual void CharacterChanged()
    {
        DecideCameraFollow();
        _healthController = playerCharacterController.GetComponent<HealthController>();
    }

    protected void DecideCameraFollow()
    {
        if (playerCharacterController.GetSide() == CharacterSide.PLAYER)
        {
            SetCameraFollow();
        }
    }

    protected virtual void SetCameraFollow()
    {
        CameraController.GetInstance().SetTarget(GetCameraTarget());
    }

    protected virtual Transform GetCameraTarget()
    {
        return playerCharacterController.GetNetworkTransform().InterpolationTarget;
    }

    private void BeforeCharacterSpawn(NetworkObject charObject)
    {
        charObject.GetComponent<PlayerCharacterController>().InitNetworkState(this);
    }

    public virtual Character GetCharacter()
    {
        if (!_characterResolved)
        {
            ResolveCharacter();
        }

        return _character;
    }

    public virtual List<Vector3> GetAlertPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        if (playerCharacterController != null && !playerCharacterController.IsDead())
        {
            positions.Add(playerCharacterController.GetNetworkTransform().ReadPosition());
        }
        return positions;
    }

    private void ResolveCharacter()
    {
        _characterResolved = true;
        _character = Characters.GetInstance().GetCharacterById(CharacterId);
    }

    public static void OnCharacterIdChange(Changed<PlayerWrapperController> changed)
    {
        changed.Behaviour.ResolveCharacter();
    }

    public static void OnCharacterControllerChange(Changed<PlayerWrapperController> changed)
    {
        changed.Behaviour.CharacterChanged();
    }

    public PlayerRef GetControllingPlayer()
    {
        return ControllingPlayer;
    }

    public virtual void CharacterDied()
    {
        if (Object.HasStateAuthority)
            Invoke(nameof(SpawnCharacter), 5);
    }
    public HealthController GetHealthController()
    {
        return _healthController;
    }
}
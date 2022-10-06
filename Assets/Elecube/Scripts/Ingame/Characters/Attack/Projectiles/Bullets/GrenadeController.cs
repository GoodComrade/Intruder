
using Fusion;
using UnityEngine;

public class GrenadeController : BasicBulletController
{
    [Header("Grenade bullet:")] [SerializeField]
    private float _explosionDamageModifier;
    [SerializeField] private float _explosionRadius;
    [SerializeField] private Animator _animator;
    private static readonly int ExplodeAnim = Animator.StringToHash("explode");

    [Networked] private short explosionDamage { get; set; }
    [Networked] protected Vector2 targetPosition { get; set; }
    [Networked] protected int explosionTick { get; set; }
    
    public override void InitNetworkState(Vector3 ownerVelocity, IntruderCharacterController intruderCharacter, Vector3 aimTarget)
    {
        base.InitNetworkState(ownerVelocity, intruderCharacter, aimTarget);
        
        explosionDamage = (short) (GameplayConstants.GetStatForLevel(_explosionDamageModifier * intruderCharacter.GetCharacter().CharacterDamage, 1) *
                                   GameplayConstants.GetPowerStatMultiplier(0, intruderCharacter.GetPowerUps()));
        targetPosition = new Vector2(aimTarget.x, aimTarget.z);
        explosionTick = (Tick) (startTick + ((Vector2.Distance(startPosition, targetPosition) / Velocity.magnitude) / Runner.DeltaTime));
    }

    protected override bool CheckForImpact(Vector3 vel, Vector3 pos, float speed, out LagCompensatedHit hit)
    {
        hit = new LagCompensatedHit();
        return false;
    }

    protected override void MoveBullet()
    {
        if (!Destroyed)
        {
            float progress = GetMovementProgress();
            transform.position =
                Vector3.Lerp(new Vector3(startPosition.x, transform.position.y, startPosition.y), new Vector3(targetPosition.x, transform.position.y, targetPosition.y), progress);
            if (progress >= 0.99f)
            {
                Detonate(transform.position);
            }
        }
    }

    private float GetMovementProgress()
    {
        return ((float) (Runner.Tick - startTick)) / (explosionTick - startTick);
    }

    protected override void Detonate(Vector3 hitPoint)
    {
        base.Detonate(hitPoint);
        if (explosionDamage > 0)
        {
            ApplyAreaDamage(hitPoint);
        }
    }

    protected override void DestroyEffect()
    {
        _animator.SetBool(ExplodeAnim, true);
    }

    private void ApplyAreaDamage(Vector3 hitPoint)
    {
        int cnt = Runner.LagCompensation.OverlapSphere(hitPoint, _explosionRadius, Object.InputAuthority, _areaHits, _collisionConfiguration.DamageLayer,
            HitOptions.None);
        if (cnt > 0)
        {
            for (int i = 0; i < cnt; i++)
            {
                IntruderHitboxRoot other = _areaHits[i].Hitbox.Root as IntruderHitboxRoot;
                if (other)
                {
                    ApplyDamage(other, explosionDamage);
                }
            }
        }
    }
}

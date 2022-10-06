using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Intruder/Collision Configuration")]
public class CollisionConfiguration : ScriptableObject
{
    [SerializeField] private LayerMask _collideLayer;
    [SerializeField] private LayerMask _damageLayer;

    public LayerMask CollideLayer => _collideLayer;
    public LayerMask DamageLayer => _damageLayer;
}

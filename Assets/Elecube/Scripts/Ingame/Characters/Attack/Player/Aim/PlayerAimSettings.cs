using System;
using UnityEngine;

[Serializable]
public class PlayerAimSettings
{
    public PlayerAimType type;
    public float length;
    public float width;
    public CollisionConfiguration collision;
}

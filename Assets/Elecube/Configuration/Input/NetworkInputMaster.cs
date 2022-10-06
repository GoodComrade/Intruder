
using Fusion;
using UnityEngine;

public struct NetworkInputMaster : INetworkInput
{
    public Vector2 move;
    public Vector2 aim;
    public NetworkBool shoot;
    public NetworkBool ability1;
    public NetworkBool ability2;
    public NetworkBool ability3;
    public NetworkBool evolve;
}

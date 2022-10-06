using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WeaponChargeSettings
{
    [SerializeField][Range(1, 10)] private int _maxCharges;
    [SerializeField][Range(1f, 20f)] private float _chargeTime;
    [SerializeField][Range(0.05f, 2f)] private float _cooldown;
    [SerializeField][Range(1, 12)] private byte _shotsPerCharge;
    [SerializeField][Range(0.0f, 0.5f)] private float _shotsInterval;

    public int MaxCharges => _maxCharges;
    public float ChargeTime => _chargeTime;
    public float Cooldown => _cooldown;
    public byte ShotsPerCharge => _shotsPerCharge;
    public float ShotsInterval => _shotsInterval;
}
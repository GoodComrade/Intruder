using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IIslandDegradationListener
{
    void SetIslandDegradationPercentage(float percentage);

    void SetMiningLocked(bool on);
}

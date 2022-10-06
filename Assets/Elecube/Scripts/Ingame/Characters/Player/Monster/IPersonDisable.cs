using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Used to disable components on worker/monster person objects when playing as a monster and morphing.
/// </summary>
public interface IPersonDisable
{
    void PersonDisable();
    void PersonEnable();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    public virtual void Enter() { }

    public abstract State RunCurrentStateLogic();

    public virtual void RunCurrentStatePhysics() { }

    public virtual void Exit() { }
}

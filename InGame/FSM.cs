using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM
{
    private object target;

    public FSM(object target)
    {
        this.target = target;
    }

    public virtual void Enter()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void Exit()
    {
    }
}

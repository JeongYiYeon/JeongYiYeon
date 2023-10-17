using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSMManager : MonoBehaviour 
{
    private Dictionary<string, FSM> fsmDic = new Dictionary<string, FSM>();

    private FSM currentState = null;
    private string currentStateKey = "";

    private void FixedUpdate()
    {
        if(currentState != null)
        {
            currentState.Update();
        }
    }

    public void SetFSM(string key, FSM value)
    {
        if (fsmDic.ContainsKey(key) == false)
        {
            fsmDic.Add(key, value);
        }
    }

    public void SetState(string key)
    {
        if(fsmDic.ContainsKey(key))
        {
            ActiveState(key);
        }
        else
        {
            Debug.LogError($"{key}등록 되지 않음");
        }
    }

    public string CurrentStateName()
    {
        return currentStateKey;
    }

    private void ActiveState(string key)
    {
        if(currentState == fsmDic[key])
        {
            return;
        }

        if (fsmDic.ContainsKey(key))
        {
            if (currentState != null)
            {
                currentState.Exit();
            }

            currentStateKey = key;
            currentState = fsmDic[key];
            currentState.Enter();
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UtilState : MonoBehaviour
{
    [SerializeField]
    private string enableObjectName = string.Empty;

    private Transform[] transforms = null;
    private string currentEnableObjectName = string.Empty;

    private void OnEnable()
    {   
        Array.Resize(ref transforms, this.transform.childCount);

        for(int i = 0; i < transforms.Length; i++) 
        {
            transforms[i] = this.transform.GetChild(i);
        }

        if(string.IsNullOrEmpty(enableObjectName) == false)
        {
            ActiveState(enableObjectName);
        }
    }

    public string ActiveStateName()
    {
        return currentEnableObjectName;
    }

    public void ActiveState(string name)
    {
        if(name == BasePopup.EButtonType.None.ToString())
        {
            ResetState();
            return;
        }

        if (currentEnableObjectName != name)
        {
            Transform tf = transform.Find(name);

            if (tf != null)
            {
                ResetState();

                tf.gameObject.SetActive(true);

                currentEnableObjectName = name;
            }
            else
            {
                Debug.LogError("이름 잘못 넣음");
            }
        }
    }

    public void ResetState()
    {
        currentEnableObjectName = "";

        if (transforms != null && transforms.Length > 0)
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    public void DeActiveState(string name)
    {
        if(string.IsNullOrEmpty(name))
        {
            return;
        }
        else
        {
            Transform tf = transform.Find(name);

            if (tf != null)
            {
                tf.gameObject.SetActive(false);
                currentEnableObjectName = "";
            }
            else
            {
                Debug.LogError("이름 잘못 넣음");
            }
        }
    }
}

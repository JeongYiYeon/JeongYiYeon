using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentFilterItem : MonoBehaviour
{
    private Action filterCb = null;

    public void SetFilterCb(Action filterCb)
    {
        if (filterCb != null)
        {
            this.filterCb = filterCb;
        }
    }

    public void OnClickFilter()
    {
        if (filterCb != null)
        {
            filterCb.Invoke();
        }
    }
}

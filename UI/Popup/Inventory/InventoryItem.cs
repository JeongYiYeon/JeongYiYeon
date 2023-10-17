using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryItem : EnhancedScrollerCellView
{
    [SerializeField]
    private BaseItem item = null;

    private BaseItemData data = null;

    public void SetData(BaseItemData data)
    {
        if(data == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        this.data = data;

        item.InitItem(data);

        this.gameObject.SetActive(true);
    }
}

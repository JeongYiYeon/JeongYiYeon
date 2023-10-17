using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePassItemData
{
    private DataGamePass dataGamePass;
    private BaseItemData itemData;
    private bool isRecive;
    private bool isPurchase;

    public DataGamePass DataGamePass => dataGamePass;

    public BaseItemData ItemData => itemData;
    public bool IsRecive => isRecive;
    public bool IsPurchase => isPurchase;

    public void SetDataGamePass(DataGamePass dataGamePass)
    {
        this.dataGamePass = dataGamePass;
    }

    public void SetItemData(BaseItemData itemData)
    {
        this.itemData = itemData;
    }

    public void SetRecive(bool isRecive) 
    {
        this.isRecive = isRecive;
    }
    public void SetPurchase(bool isPurchase)
    {
        this.isPurchase = isPurchase;
    }
}

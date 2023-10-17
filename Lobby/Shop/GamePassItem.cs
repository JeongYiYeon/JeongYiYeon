using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GamePassItem : EnhancedScrollerCellView
{
    [SerializeField]
    private TMP_Text labelLevel = null;
    [SerializeField]
    private BaseItem item = null;
    [SerializeField]
    private Image imgRecive = null;

    private GamePassItemData data = null;

    public void SetData(GamePassItemData data, int dataIndex)
    {
        bool isRecive = false;

        this.data = data;        
        this.dataIndex = dataIndex;

        labelLevel.text = $"{data.DataGamePass.GAMEPASS_LEVEL}";

        item.gameObject.SetActive(data.ItemData != null);

        if (data.ItemData != null)
        {
            item.InitItem(data.ItemData);

            isRecive = data.IsRecive == true && data.IsPurchase == true;
        }

        imgRecive.gameObject.SetActive(isRecive);
    }
}

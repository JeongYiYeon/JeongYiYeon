using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestRewardItem : MonoBehaviour
{
    [SerializeField]
    private BaseItem item = null;
    [SerializeField]
    private Image imgRecive = null;

    private BaseItemData itemData = null;
    private bool isRecive = false;

    public BaseItemData ItemData => itemData;

    public bool IsRecive => isRecive;

    public void Reset()
    {
        isRecive = false;
        imgRecive.gameObject.SetActive(false);
    }

    public void RefreshQuestRewardItem()
    {
        item.InitItem(itemData);
        imgRecive.gameObject.SetActive(isRecive);
    }

    public void SetRewardItem(BaseItemData itemData)
    {
        this.itemData = itemData;
    }

    public void SetRecive(bool isRecive)
    {
        this.isRecive = isRecive;
    }
}

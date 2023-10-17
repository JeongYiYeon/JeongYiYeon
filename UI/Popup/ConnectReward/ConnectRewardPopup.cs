using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectRewardPopup : BasePopup
{
    [SerializeField]
    private Transform itemListRoot = null;
    [SerializeField]
    private BaseItem[] rewardItems = null;

    private List<BaseItemData> rewardItemDataList = new List<BaseItemData>();


    public void SetRewardItemDataList(List<BaseItemData> rewardItemDataList)
    {
        this.rewardItemDataList = rewardItemDataList;
    }
    private void ResetRewardItemList()
    {
        for (int i = 0; i < rewardItems.Length; i++)
        {
            rewardItems[i].gameObject.SetActive(false);
        }
    }

    private void SetRewardItemList()
    {
        for (int i = 0; i < rewardItemDataList.Count; i++)
        {
            rewardItems[i].InitItem(rewardItemDataList[i]);

            rewardItems[i].gameObject.SetActive(true);
        }
    }

    private void OnEnable()
    {
        SetType(EPopupType.ConnectReward);
    }

    public override void Init()
    {
        base.Init();
    }

    public override void Active()
    {
        base.Active();

        ResetRewardItemList();
        SetRewardItemList();
    }
}

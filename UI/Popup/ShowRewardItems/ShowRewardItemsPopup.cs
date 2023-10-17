using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShowRewardItemsPopup : BasePopup
{
    [SerializeField]
    private Transform itemListRoot = null;
    [SerializeField]
    private BaseItem[] rewardItems = null;

    [SerializeField]
    private TMP_Text labelDesc = null;

    private List<BaseItemData> rewardItemDataList = new List<BaseItemData>();

    private void OnEnable()
    {
        SetType(EPopupType.ShowRewardItems);
    }

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

    public override void Init()
    {
        base.Init();
    }

    public override void Active()
    {
        base.Active();

        ResetRewardItemList();
        SetRewardItemList();
        labelDesc.text = desc;        
    }
    public override void OnClickConfirm()
    {
        base.OnClickConfirm();
    }

    public override void OnClickCancel()
    {
        base.OnClickCancel();
    }
}

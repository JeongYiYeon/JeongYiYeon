using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using EnhancedUI.EnhancedScroller;

public class QuestItem : EnhancedScrollerCellView
{
    [SerializeField]
    private TMP_Text labelTitle = null;
    [SerializeField]
    private TMP_Text labelDesc = null;
    [SerializeField]
    private TMP_Text labelClear = null;
    [SerializeField]
    private QuestRewardItem rewardItem = null;

    [SerializeField]
    private Button btClear = null;

    private QuestData data = null;

    public void SetData(QuestData data, int dataIndex)
    {
        this.data = data;
        this.dataIndex = dataIndex;

        rewardItem.gameObject.SetActive(data.RewardItemData != null);

        if (data.RewardItemData != null && rewardItem != null)
        {
            rewardItem.SetRewardItem(data.RewardItemData);
            rewardItem.SetRecive(data.ClearType == QuestData.EQuestClearType.Clear);

            rewardItem.RefreshQuestRewardItem();
        }            

        RefreshTitleLabel();

        labelDesc.text = $"{data.Desc}";

        RefreshClearLabel();
        RefreshClearButton();
    }

    private void RefreshTitleLabel()
    {
        labelTitle.text = $"{data.Title}";
    }

    private void RefreshClearLabel()
    {
        string labelString = "";

        if(data.ClearType == QuestData.EQuestClearType.Clear)
        {
            labelString = "완료";
        }
        else
        {
            if(data.CurrentCount < data.ClearCount)
            {
                labelString = "이동";
            }
            else
            {
                labelString = "완료하기";
            }
        }

        labelClear.text = labelString;
    }

    private void RefreshClearButton()
    {
        bool isClear = data.ClearType == QuestData.EQuestClearType.Clear;

        btClear.interactable = !isClear;
    }

    private void MoveQuestCategoryType()
    {
        switch(data.QuestCategoryType)
        {
            case QuestData.EQuestCategoryType.NONE:
                Debug.LogError("이동 안함");
                break;
            case QuestData.EQuestCategoryType.ENCHANT_EQUIP:
                LobbyManager.Instance.SetMenu(BaseEnum.EMenuCategory.Hero);                
                break;
            case QuestData.EQuestCategoryType.ENCHANT_HERO:
                LobbyManager.Instance.SetMenu(BaseEnum.EMenuCategory.Hero);                
                break;
            case QuestData.EQuestCategoryType.WATCH_AD:                
                break;
            case QuestData.EQuestCategoryType.ATTENDANCE:                
                break;
            case QuestData.EQuestCategoryType.CONNECT_REWARD:                
                break;
            case QuestData.EQuestCategoryType.CHALLENGE_STAGE_CLEAR:
                break;
        }

        if (data.QuestCategoryType != QuestData.EQuestCategoryType.NONE)
        {
            LobbyManager.Instance.SetMenu(BaseEnum.EMenuCategory.Quest);
        }
    }

    #region OnClick
    public async void OnClickClearQuest()
    {
        if (data.CurrentCount < data.ClearCount)
        {
            MoveQuestCategoryType();            
        }
        else
        {
            await NetworkPacketQuest.Instance.TaskQuestClear(data.QuestUID, (questList) =>
            {
                RefreshTitleLabel();
                RefreshClearButton();
                RefreshClearLabel();

                Quest.Instance.RefreshQuestData(questList);

                //questPopup.ClearRewardItem(data.QuestType, data.QuestUID);
            });
        }
    }
    #endregion
}

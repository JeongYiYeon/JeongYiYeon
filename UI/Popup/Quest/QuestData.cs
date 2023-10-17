using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData
{
    public enum EQuestType
    {
        Day = 1,          // ����
        Guide =2          // ���̵� ����Ʈ
    }

    public enum EQuestClearType
    { 
        NotClear,
        Clear,
    }

    public enum EQuestCategoryType
    {
        NONE = 0,

        COUNT_REWARD = 1,               //����Ʈ �Ϸ� ī��Ʈ ����
        MAIN_STAGE_CLEAR = 2,           //�������� Ŭ���� (QUEST_VALUE�� �Էµ� �������� Ŭ���� ����)
        CHALLENGE_STAGE_CLEAR = 3,      //���� �������� Ŭ����(QUEST_VALUE�� �Էµ� �������� Ŭ���� ����)
        ENCHANT_EQUIP = 4,              //��� ��ȭ
        ENCHANT_HERO = 5,               //���� ��ȭ
        WATCH_AD = 6,                   //���� ��û
        ATTENDANCE = 7,                 //�⼮ �Ϸ�
        CONNECT_REWARD = 8,             //���� ���� ȹ��
        ENEMY_KILL = 9,                 //�� óġ
    }

    private EQuestType questType;
    private EQuestCategoryType questCategoryType;

    [JsonProperty("UID")]
    private int questUID;
    private string title;
    private string desc;

    [JsonProperty("COUNT")]
    private int currentCount;
    private int clearCount;

    private int clearStageUID;

    [JsonProperty("IS_CLEAR")]
    private bool clearType;
    private BaseItemData rewardItemData;

    public int QuestUID => questUID;
    public EQuestType QuestType => questType;
    public EQuestCategoryType QuestCategoryType => questCategoryType;
    public string Title => title;
    public string Desc => desc;
    public int CurrentCount => currentCount;
    public int ClearCount => clearCount;
    public int ClearStageUID => clearStageUID;
    public EQuestClearType ClearType 
    {
        get
        {
            if(clearType == true)
            {
                return EQuestClearType.Clear;
            }
            else
            {
                return EQuestClearType.NotClear;
            }
        }
    }

    public BaseItemData RewardItemData => rewardItemData;

    public void SetQuestType(EQuestType questType)
    {
        this.questType = questType;
    }
    public void SetQuestCategoryType(EQuestCategoryType questCategoryType)
    {
        this.questCategoryType = questCategoryType;
    }
    public void SetTitle(string title)
    {
        this.title = title;
    }

    public void SetDesc(string desc)
    {
        this.desc = desc;
    }

    public void SetClearCount(int clearCount)
    {
        this.clearCount = clearCount;
    }

    public void SetClearStageUID(int clearStageUID)
    {
        this.clearStageUID = clearStageUID;
    }

    public void SetRewardItemData(BaseItemData rewardItemData)
    {
        this.rewardItemData = rewardItemData;
    }
}

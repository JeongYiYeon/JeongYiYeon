using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestData
{
    public enum EQuestType
    {
        Day = 1,          // 일일
        Guide =2          // 가이드 퀘스트
    }

    public enum EQuestClearType
    { 
        NotClear,
        Clear,
    }

    public enum EQuestCategoryType
    {
        NONE = 0,

        COUNT_REWARD = 1,               //퀘스트 완료 카운트 보상
        MAIN_STAGE_CLEAR = 2,           //스테이지 클리어 (QUEST_VALUE에 입력된 스테이지 클리어 조건)
        CHALLENGE_STAGE_CLEAR = 3,      //도전 스테이지 클리어(QUEST_VALUE에 입력된 스테이지 클리어 조건)
        ENCHANT_EQUIP = 4,              //장비 강화
        ENCHANT_HERO = 5,               //영웅 강화
        WATCH_AD = 6,                   //광고 시청
        ATTENDANCE = 7,                 //출석 완료
        CONNECT_REWARD = 8,             //접속 보상 획득
        ENEMY_KILL = 9,                 //적 처치
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

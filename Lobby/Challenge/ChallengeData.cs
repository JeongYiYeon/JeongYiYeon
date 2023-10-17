using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeData
{
    public enum EChallengeType
    {
        NONE = -1,
        GOLD,               // 골드
        GROWUP,             // 성장 재료
        ENCHANT,            // 강화 재료
    }

    //챌린지 UID
    private int uid;
    //챌린지 타이틀
    private string challengeTitle;
    //챌린지 설명
    private string challengeDesc;
    //챌린지 배경 이미지 경로
    private string challengePrefabPath;

    private EChallengeType challengeType = EChallengeType.NONE;

    private int beforechallengeUID = 0;
    private int afterchallengeUID = 0;

    private int enemyRegenGroupUID = 0;

    //초단위
    private int timeLimit = 0;

    private int dayEnterCnt = 0;

    private BaseItemData enterItemData = null;

    private List<BaseItemData> rewardItemDataList = new List<BaseItemData>();

    public int UID => uid;
    public string ChallengeTitle => challengeTitle;
    public string ChallengeDesc => challengeDesc;
    public string ChallengePrefabPath => challengePrefabPath;

    public EChallengeType ChallengeType => challengeType;

    public int BeforeChallengeUID => beforechallengeUID;
    public int AfterChallengeUID => afterchallengeUID;
    public int EnemyRegenGroupUID => enemyRegenGroupUID;
    public int TimeLimit => timeLimit;
    public int DayEnterCnt => dayEnterCnt;
    public BaseItemData EnterItemData => enterItemData;
    public List<BaseItemData> RewardItemDataList => rewardItemDataList;


    public void SetUID(int uid)
    {
        this.uid = uid;
    }

    public void SetChallengeTitle(string challengeTitle)
    {
        this.challengeTitle = challengeTitle;
    }
    public void SetChallengeDesc(string challengeDesc)
    {
        this.challengeDesc = challengeDesc;
    }

    public void SetChallengePrefabPath(string challengePrefabPath)
    {
        this.challengePrefabPath = challengePrefabPath;
    }

    public void SetChallengeType(string challengeType)
    {
        if (Enum.TryParse<EChallengeType>(challengeType, out this.challengeType) == false)
        {
            Debug.LogError("스테이지 타입 에러");
        }
    }
    public void SetBeforeChallengeUID(int beforechallengeUID)
    {
        this.beforechallengeUID = beforechallengeUID;
    }
    public void SetAfterChallengeUID(int afterchallengeUID)
    {
        this.afterchallengeUID = afterchallengeUID;
    }
    public void SetEnemyRegenGroupUID(int enemyRegenGroupUID)
    {
        this.enemyRegenGroupUID = enemyRegenGroupUID;
    }
    public void SetTimeLimit(int timeLimit)
    {
        this.timeLimit = timeLimit;
    }
    public void SetDayEnterCnt(int dayEnterCnt)
    {
        this.dayEnterCnt = dayEnterCnt;
    }
    public void SetEnterItem(int itemUID, int itemCnt)
    {
        BaseItemData item = new BaseItemData();

        DataItem itemData = DataManager.Instance.DataHelper.Item.Find(x => x.UID == itemUID);

        if (itemData == null)
        {
            return;
        }

        item.SetDataItem(itemData);
        item.SetItemUID(itemData.UID);
        item.SetTitle(DataManager.Instance.GetLocalization(itemData.ITEM_NAME));
        item.SetDesc(DataManager.Instance.GetLocalization(itemData.ITEM_DESC));
        item.SetItemCategory(Enum.Parse<BaseItem.EItemCategory>(itemData.ITEM_CATEGORY));
        item.SetItemGrade(itemData.ITEM_GRADE);
        item.SetAtlasPath(itemData.ITEM_ICON_ATLAS);
        item.SetImgItemPath(itemData.ITEM_ICON);

        item.SetViewItemCount(itemCnt);

        enterItemData = item;
    }
    public void SetRewardItemDataList(List<BaseItemData> rewardItemDataList)
    {
        this.rewardItemDataList = rewardItemDataList;
    }
}

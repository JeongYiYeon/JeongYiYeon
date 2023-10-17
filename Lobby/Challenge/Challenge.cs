using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenge : MonoSingletonInScene<Challenge>
{
    [SerializeField]
    private ChallengeItem[] challengeItems = null;

    private Dictionary<ChallengeData.EChallengeType, List<ChallengeData>> challengeDataDic = new Dictionary<ChallengeData.EChallengeType, List<ChallengeData>>();
    private ChallengeData currentChallengeData = null;
    public ChallengeData CurrentChallengeData => currentChallengeData;

    private void OnEnable()
    {
        InitChallenge();
    }

    private void InitChallenge()
    {
        for(int i = 0; i < Enum.GetValues(typeof(ChallengeData.EChallengeType)).Length; i++)
        {
            List<DataChallenge> challengeList = DataManager.Instance.DataHelper.Challenge.FindAll(x => x.STAGE_TYPE == ((ChallengeData.EChallengeType)i).ToString());

            if (challengeList != null && challengeList.Count > 0)
            {
                List<ChallengeData> tmpChallengeData = new List<ChallengeData>();

                for (int j = 0; j < challengeList.Count; j++)
                {
                    ChallengeData challengeData = new ChallengeData();
                    challengeData.SetUID(challengeList[j].UID);
                    challengeData.SetChallengeTitle(DataManager.Instance.GetLocalization(challengeList[j].CHALLENGE_NAME));
                    challengeData.SetChallengeDesc(DataManager.Instance.GetLocalization(challengeList[j].CHALLENGE_DESC));
                    challengeData.SetChallengePrefabPath(challengeList[j].CHALLENGE_BACK);
                    challengeData.SetChallengeType(challengeList[j].STAGE_TYPE);
                    challengeData.SetBeforeChallengeUID(challengeList[j].BEFORE_STAGE);
                    challengeData.SetAfterChallengeUID(challengeList[j].NEXT_STAGE);
                    challengeData.SetEnemyRegenGroupUID(challengeList[j].REGEN_GROUP);
                    challengeData.SetTimeLimit(challengeList[j].TIME_LIMIT);
                    challengeData.SetDayEnterCnt(challengeList[j].DAY_ENTER_COUNT);
                    challengeData.SetEnterItem(challengeList[j].ENTER_ITEM, challengeList[j].ENTER_ITEM_COUNT);

                    List<DataRewardLink> rewardGroupList =
                                DataManager.Instance.DataHelper.RewardLink.FindAll(x => x.REWARD_GROUP == challengeList[j].CLEAR_REWARD_GROUP);

                    if (rewardGroupList != null && rewardGroupList.Count > 0)
                    {
                        List<BaseItemData> rewardItemDataList = new List<BaseItemData>();

                        for (int k = 0; k < rewardGroupList.Count; k++)
                        {
                            BaseItemData item = new BaseItemData();

                            DataItem itemData = DataManager.Instance.DataHelper.Item.Find(x => x.UID == rewardGroupList[k].REWARD_ITEM);

                            if (itemData == null)
                            {
                                continue;
                            }
                            item.SetDataItem(itemData);
                            item.SetItemUID(itemData.UID);
                            item.SetTitle(DataManager.Instance.GetLocalization(itemData.ITEM_NAME));
                            item.SetDesc(DataManager.Instance.GetLocalization(itemData.ITEM_DESC));
                            item.SetItemCategory(Enum.Parse<BaseItem.EItemCategory>(itemData.ITEM_CATEGORY));
                            item.SetItemGrade(itemData.ITEM_GRADE);
                            item.SetRewardItemRate((float)rewardGroupList[k].REWARD_ITEM_RATE);
                            item.SetAtlasPath(itemData.ITEM_ICON_ATLAS);
                            item.SetImgItemPath(itemData.ITEM_ICON);

                            item.SetItemCount(rewardGroupList[k].REWARD_ITEM_COUNT);
                        }

                        challengeData.SetRewardItemDataList(rewardItemDataList);
                    }

                    tmpChallengeData.Add(challengeData);
                }

                if (challengeDataDic.ContainsKey((ChallengeData.EChallengeType)i) == false)
                {
                    challengeDataDic.Add((ChallengeData.EChallengeType)i, tmpChallengeData);
                }
                else
                {
                    challengeDataDic[(ChallengeData.EChallengeType)i] = tmpChallengeData;
                }
            }
        }

        for (int i = 0; i < challengeItems.Length; i++) 
        {
            challengeItems[i].SetDataList(challengeDataDic[(ChallengeData.EChallengeType)i]);

            //서버에서 받아서 처리해야됨
            challengeItems[i].SetLastClearChallengeUID(challengeDataDic[(ChallengeData.EChallengeType)i][0].UID);
            challengeItems[i].SetCurrentChallengeData(challengeDataDic[(ChallengeData.EChallengeType)i][0].UID);
            challengeItems[i].SetChallengeItem();
        }
    }

    public void SetChallengeData(ChallengeData data)
    {
        currentChallengeData = data;
    }    
}

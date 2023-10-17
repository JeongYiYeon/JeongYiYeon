using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Coffee.UIExtensions;
using UnityEngine.U2D;

public class ChallengeItem : MonoBehaviour
{
    [SerializeField]
    private UtilEnumSelect challengeType = null;

    [SerializeField]
    private TMP_Text labelTitle = null;
    [SerializeField]
    private TMP_Text labelDesc = null;

    [SerializeField]
    private GameObject preChallengeBt = null;
    [SerializeField]
    private GameObject nextChallengeBt = null;

    [SerializeField]
    private TMP_Text enterChallengeCost = null;
    [SerializeField]
    private AtlasImage ImgEnterChallenge = null;

    private List<ChallengeData> dataList = new List<ChallengeData>();
    private ChallengeData data = null;

    private int lastClearChallengeUID = 0;
    private int idx = 0;

    public UtilEnumSelect ChallengeType => challengeType;

    public void SetDataList(List<ChallengeData> dataList)
    {
        this.dataList = dataList;
    }

    public void SetLastClearChallengeUID(int lastClearChallengeUID)
    {
        this.lastClearChallengeUID = lastClearChallengeUID;
    }

    public void SetCurrentChallengeData(int currentChallengeUID)
    {
        idx = dataList.FindIndex(x => x.UID == currentChallengeUID);

        if (idx != -1)
        {
            data = dataList[idx];
        }
    }

    public void SetChallengeItem()
    {
        if (data == null)
        {
            return;
        }

        labelTitle.text = data.ChallengeTitle;
        labelDesc.text = data.ChallengeDesc;
        RefreshChallengeBt(data);
        RefreshEnterChallengeCost(data);
    }

    public void RefreshChallengeBt(ChallengeData data)
    {
        preChallengeBt.SetActive(data.BeforeChallengeUID != 0);
        nextChallengeBt.SetActive(data.AfterChallengeUID != 0 && lastClearChallengeUID + 1 >= data.AfterChallengeUID);
    }

    public void RefreshEnterChallengeCost(ChallengeData data)
    {
        //나중에 서버에서 받아서 앞에 처리
        enterChallengeCost.text = $"{data.DayEnterCnt} / {data.DayEnterCnt}";

        SpriteAtlas atlas = AtlasManager.Instance.Atlas[data.EnterItemData.AtlasPath];

        AtlasManager.Instance.SetSprite(ImgEnterChallenge, atlas, data.EnterItemData.ImgItemPath);
    }

    public void OnClickPre()
    {
        if(data.BeforeChallengeUID > 0)
        {
            SetCurrentChallengeData(data.UID - 1);
            SetChallengeItem();
            Challenge.Instance.SetChallengeData(data);
        }
        else
        {
            Debug.LogError("이전 스테이지 없음");
            preChallengeBt.SetActive(false);
        }
    }

    public void OnClickNext()
    {
        if (data.AfterChallengeUID > 0)
        {
            SetCurrentChallengeData(data.UID + 1);
            SetChallengeItem();
            Challenge.Instance.SetChallengeData(data);
        }
        else
        {
            Debug.LogError("다음 스테이지 없음");
            nextChallengeBt.SetActive(false);
        }
    }

    public void OnClickEnter()
    {
        if (GameManager.Instance.IsChallenge == false)
        {
            LoadingManager.Instance.ActiveLoading();
            Challenge.Instance.SetChallengeData(data);
            GameManager.Instance.SetChallenge();
            LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
        }
    }
}

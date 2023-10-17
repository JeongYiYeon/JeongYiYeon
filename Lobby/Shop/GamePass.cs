using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class GamePass : MonoBehaviour
{
    [SerializeField]
    private TMP_Text labelSeason = null;
    [SerializeField]
    private TMP_Text labelRemainTime = null;
    [SerializeField]
    private TMP_Text labelGPLevel = null;
    [SerializeField]
    private TMP_Text labelGPExp = null;
    [SerializeField]
    private TMP_Text labelPurchase = null;

    [SerializeField]
    private Slider gpFreeSlider = null;
    [SerializeField]
    private Slider gpPurchaseSlider = null;

    [SerializeField]
    private GamePassScrollContorller gpFreeScrollController = null;
    [SerializeField]
    private GamePassScrollContorller gpPurchaseScrollController = null;

    [SerializeField]
    private GameObject noPurchaseGo = null;

    private int season = 0;
    private int currentExp = 0;
    private int nextExp = 0;
    private int gpLevel = 0;

    private bool isPurchase = false;

    private DateTime currentTime = DateTime.MinValue;
    private DateTime endTime = DateTime.MinValue;

    private TimeSpan time = TimeSpan.Zero;

    private Dictionary<EnumShop.EGamePass, List<GamePassItemData>> dicGamePassInfo = new Dictionary<EnumShop.EGamePass, List<GamePassItemData>>();

    public void InitGamePassData()
    {
        InitGamePassInfo();
        InitGamePassFreeData();
        InitGamePassPurchaseData();

        gpFreeScrollController.SetGPItem(dicGamePassInfo[EnumShop.EGamePass.Free], RefreshGPSlider, isFree : true);
        gpPurchaseScrollController.SetGPItem(dicGamePassInfo[EnumShop.EGamePass.Purchase], RefreshGPSlider, isFree: false);
    }

    public void SetSeason(int season)
    {
        this.season = season;
    }

    public void SetGPLevel(int gpLevel)
    {
        this.gpLevel = gpLevel;
    }
    public void SetCurrentExp(int currentExp)
    {
        this.currentExp = currentExp;
    }
    public void SetNextExp(int nextExp)
    {
        this.nextExp = nextExp;
    }

    private void InitGamePassInfo()
    {
        currentTime = DateTime.Now;
        endTime = currentTime.AddDays(6);

        time = endTime- currentTime;

        labelSeason.text = $"시즌 {season}";
        labelGPLevel.text = $"레벨 : {gpLevel}";
        labelGPExp.text = $"{currentExp} / {nextExp}";

        StartCoroutine(IERemainTime());
    }    

    private void RefreshLabelRemainTime()
    {
        labelRemainTime.text = $"남은 시간 : {time.Days} 일 {time.Hours} 시간 {time.Minutes} 분 남음";
    }

    private IEnumerator IERemainTime()
    {
        while(true)
        {
            currentTime = DateTime.Now;

            time = endTime - currentTime;

            RefreshLabelRemainTime();

            yield return new WaitForSeconds(1f);
        }
    }

    private void InitGamePassFreeData()
    {
        if (dicGamePassInfo.ContainsKey(EnumShop.EGamePass.Free) == false)
        {
            List<DataGamePass> dataGamePassList =
                DataManager.Instance.DataHelper.GamePass.FindAll(x => x.EGamePass == EnumShop.EGamePass.Free);

            List<GamePassItemData> list = new List<GamePassItemData>();

            for (int i = 0; i < dataGamePassList.Count; i++)
            {
                GamePassItemData itemData = new GamePassItemData();
                itemData.SetDataGamePass(dataGamePassList[i]);

                DataItem rewardItemData = DataManager.Instance.DataHelper.Item.Find(x => x.UID == dataGamePassList[i].REWARD_ITEM);
                BaseItemData rewardItem = new BaseItemData();

                rewardItem.SetDataItem(rewardItemData);
                rewardItem.SetDesc(DataManager.Instance.GetLocalization(rewardItemData.ITEM_DESC));
                rewardItem.SetItemCategory(Enum.Parse<BaseItem.EItemCategory>(rewardItemData.ITEM_CATEGORY));

                rewardItem.SetItemGrade(rewardItemData.ITEM_GRADE);
                rewardItem.SetAtlasPath(rewardItemData.ITEM_ICON_ATLAS);
                rewardItem.SetImgItemPath(rewardItemData.ITEM_ICON);

                itemData.SetItemData(rewardItem);

                itemData.SetRecive(gpLevel >= i + 1);
                itemData.SetPurchase(true);
                list.Add(itemData);
            }

            dicGamePassInfo.Add(EnumShop.EGamePass.Free, list);
        }
    }

    private void InitGamePassPurchaseData()
    {
        if (dicGamePassInfo.ContainsKey(EnumShop.EGamePass.Purchase) == false)
        {
            List<DataGamePass> dataGamePassList =
                  DataManager.Instance.DataHelper.GamePass.FindAll(x => x.EGamePass == EnumShop.EGamePass.Purchase);

            List<GamePassItemData> list = new List<GamePassItemData>();

            for (int i = 0; i < dataGamePassList.Count; i++)
            {
                GamePassItemData itemData = new GamePassItemData();
                itemData.SetDataGamePass(dataGamePassList[i]);

                DataItem rewardItemData = DataManager.Instance.DataHelper.Item.Find(x => x.UID == dataGamePassList[i].REWARD_ITEM);
                BaseItemData rewardItem = new BaseItemData();

                rewardItem.SetDataItem(rewardItemData);
                rewardItem.SetDesc(DataManager.Instance.GetLocalization(rewardItemData.ITEM_DESC));
                rewardItem.SetItemCategory(Enum.Parse<BaseItem.EItemCategory>(rewardItemData.ITEM_CATEGORY));

                rewardItem.SetItemGrade(rewardItemData.ITEM_GRADE);
                rewardItem.SetAtlasPath(rewardItemData.ITEM_ICON_ATLAS);
                rewardItem.SetImgItemPath(rewardItemData.ITEM_ICON);

                itemData.SetItemData(rewardItem);

                itemData.SetRecive(gpLevel >= i + 1);
                itemData.SetPurchase(true);
                list.Add(itemData);
            }

            dicGamePassInfo.Add(EnumShop.EGamePass.Purchase, list);
        }
    }

    private void RefreshPurchaseItem()
    {
        foreach(GamePassItemData itemData in dicGamePassInfo[EnumShop.EGamePass.Purchase])
        {
            itemData.SetPurchase(isPurchase);
        }

        gpPurchaseScrollController.SetGPItem(dicGamePassInfo[EnumShop.EGamePass.Purchase], RefreshGPSlider, isFree: false);

    }

    public void RefreshGPSlider(bool isFree)
    {
        if (isFree == true)
        {
            gpFreeSlider.minValue = gpFreeScrollController.GetStartCellViewIdx;
            gpFreeSlider.maxValue = gpFreeScrollController.GetEndCellViewIdx;

            gpFreeSlider.value = gpLevel > gpFreeSlider.minValue ? gpLevel : gpFreeSlider.minValue;
        }
        else
        {
            gpPurchaseSlider.minValue = gpPurchaseScrollController.GetStartCellViewIdx;
            gpPurchaseSlider.maxValue = gpPurchaseScrollController.GetEndCellViewIdx;

            gpPurchaseSlider.value = gpLevel > gpPurchaseSlider.minValue ? gpLevel : gpPurchaseSlider.minValue;
        }
    }

    #region OnClick
    public void OnClickPurchaseGamePass()
    {
        if(isPurchase == true)
        {
            return;
        }

        isPurchase = true;
        labelPurchase.text = "구매 완료";
        noPurchaseGo.SetActive(!isPurchase);

        RefreshPurchaseItem();
    }
    #endregion
}

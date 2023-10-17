using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItemData
{
    [JsonProperty("UID")]
    private string uid;
    [JsonProperty("IID")]
    private int itemUID;
    [JsonProperty("COUNT")]
    private double itemCount;
    [JsonProperty("ITEM_LEVEL")]
    private protected int upgradeCnt = 0;
    [JsonProperty("EQUIP_CID")]
    private protected int equipCharacterUID = 0;

    private string title;
    private string desc;
    private BaseItem.EItemCategory itemCategory;
    private string atlasPath;
    private string imgItemPath;

    private int itemGrade = 0;
    private double maxCount = 0;                                                    // 아이템 최대 보유 가능수
    private double stackCount = 0;                                                  // 아이템 겹칠 수 있는 수
    private bool isSellPossible = false;                                         // 판매 가능 여부

    private UserData.EGoodsType sellGoodsType = UserData.EGoodsType.NONE;        // 판매 했을때 받을 재화 타입
    private int sellItemCount = 0;                                               // 판매 했을때 받을 재화 갯수

    private bool isToolTip = true;                     //툴팁창은 디폴트로 둠
    
    private float rewardItemRate = 0f;                 //확률 가중치 1000f = 100%

    private DataItem dataItem = null;

    public string GetUID => uid;
    public int ItemUID => itemUID;
    public string Title => title;
    public string Desc => desc;
    public BaseItem.EItemCategory ItemCategory => itemCategory;
    public string AtlasPath => atlasPath;
    public string ImgItemPath => imgItemPath;
    public double ItemCount => itemCount;

    public int ItemGrade => itemGrade;
    public double MaxCount => maxCount;
    public double StackCount => stackCount;

    public bool IsSellPossible => isSellPossible;
    public UserData.EGoodsType SellGoodsType => sellGoodsType;
    public int SellItemCount => sellItemCount;

    public bool IsToolTip => isToolTip;

    public float RewardItemRate => rewardItemRate;

    public DataItem DataItem => dataItem;

    public void SetDataItem(DataItem dataItem)
    {
        this.dataItem = dataItem;
    }

    public void SetItemUID(int itemUID)
    {
        this.itemUID = itemUID;
    }

    public void SetTitle(string title)
    {
        this.title = title;
    }

    public void SetDesc(string desc)
    {
        this.desc = desc;
    }

    public void SetItemCategory(BaseItem.EItemCategory itemCategory)
    {
        this.itemCategory = itemCategory;
    }
    public void SetAtlasPath(string atlasPath)
    {
        this.atlasPath = atlasPath;
    }

    public void SetImgItemPath(string imgItemPath)
    {
        this.imgItemPath = imgItemPath;
    }

    public void SetItemGrade(int itemGrade)
    {
        this.itemGrade = itemGrade;
    }

    public void SetItemMaxCount(double maxCount)
    {
        this.maxCount = maxCount;
    }

    public void SetItemStackCount(double stackCount)
    {
        this.stackCount = stackCount;
    }

    /// <summary>
    /// 실제로 아이템 갯수 처리할때
    /// </summary>
    /// <param name="itemCount"></param>
    public void SetItemCount(double itemCount)
    {
        if(stackCount == 0 || maxCount == 0)
        {
            return;
        }

        if(itemCount <= maxCount && itemCount <= stackCount)
        {
            this.itemCount = itemCount;
        }
        else
        {
            if(itemCount > maxCount)
            {
                this.itemCount = maxCount;
            }
            else if(itemCount > stackCount)
            {
                this.itemCount = stackCount;
            }
        }
    }

    /// <summary>
    /// 보여주는 용도로 아이템 갯수 셋팅할때
    /// </summary>
    /// <param name="itemCount"></param>
    public void SetViewItemCount(int itemCount)
    {
        this.itemCount = itemCount;
    }


    public void SetSellPossible(bool isSellPossible)
    {
        this.isSellPossible = isSellPossible;
    }

    public void SetSellGoodsType(int itemUID)
    {
        DataItem item = DataManager.Instance.DataHelper.Item.Find(x => x.UID == itemUID);

        if (item == null)
        {
            sellGoodsType = UserData.EGoodsType.NONE;
        }
        else
        {
            sellGoodsType = Enum.Parse<UserData.EGoodsType>(item.ITEM_TYPE);
        }
    }

    public void SetSellItemCount(int sellItemCount)
    {
        this.sellItemCount = sellItemCount;
    }

    public void SetToolTip(bool isToolTip)
    {
        this.isToolTip = isToolTip;
    }

    public void SetRewardItemRate(float rewardItemRate)
    {
        this.rewardItemRate = rewardItemRate;
    }


}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroListItemData
{
    private int uid = 0;
    private CharacterData characterData = null;
    private UserData.EGoodsType purchaseType = UserData.EGoodsType.NONE;
    private double purchasePrice = 0;
    private int stageUID = 0;
    private bool isHave = false;
    private bool isPackage = false;

    public int UID => uid;
    public CharacterData CharacterData => characterData;
    public UserData.EGoodsType PurchaseType => purchaseType;
    public double PurchasePrice => purchasePrice;

    public int StageUID => stageUID;
    public bool IsHave => isHave;
    public bool IsPackage => isPackage;

    public void SetUID(int uid)
    {
        this.uid = uid;
    }

    public void SetCharacterData(CharacterData characterData)
    {
        this.characterData = characterData;
    }

    public void SetPurchaseType(UserData.EGoodsType purchaseType)
    {
        this.purchaseType = purchaseType;
    }

    //스테이지 획득 캐릭터 일때 사용
    public void SetStageUID(int stageUID)
    {
        this.stageUID = stageUID;
    }

    public void SetPurchasePrice(double purchasePrice) 
    {
        this.purchasePrice = purchasePrice;
    }

    public void SetHave(bool isHave) 
    {
        this.isHave = isHave;
    }
    public void SetPackage(bool isPackage)
    {
        this.isPackage = isPackage;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumShop : BaseEnum
{
    public enum ESelectShopMenu
    {
        None,

        ShopSubCategory,
        EquipmentGachaCount
    }

    //상점 세부 카테고리
    public enum EShopSubCategory
    {
        None,

        Cash,
        Gacha,
        Package,
    }

    //가챠 카운트
    public enum EEquipmentGachaCount
    {
        None,

        One,
        Ten,
        AdTen
    }

    //게임 패스 구매여부
    public enum EGamePass
    {
        None,
        
        Free,
        Purchase,
    }
    

    private ESelectShopMenu selectShopMenu;
    private EShopSubCategory shopSubCategory;
    private EEquipmentGachaCount equipmentGachaCount;
    private EGamePass gamePass;

    public ESelectShopMenu SelectShopMenu => selectShopMenu;
    public EShopSubCategory ShopSubCategory => shopSubCategory;
    public EEquipmentGachaCount EquipmentGachaCount => equipmentGachaCount;
    public EGamePass GamePass => gamePass;

    public void SetSelectShopMenu(ESelectShopMenu type)
    {
        selectShopMenu = type;
    }

    public void SetShopSubCategory(EShopSubCategory type)
    {
        shopSubCategory = type;
    }
    public void SetEquipmentGachaCount(EEquipmentGachaCount type)
    {
        equipmentGachaCount = type;
    }
    public void SetGamePass(EGamePass type)
    {
        gamePass = type;
    }
}

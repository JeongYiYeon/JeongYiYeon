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

    //���� ���� ī�װ�
    public enum EShopSubCategory
    {
        None,

        Cash,
        Gacha,
        Package,
    }

    //��í ī��Ʈ
    public enum EEquipmentGachaCount
    {
        None,

        One,
        Ten,
        AdTen
    }

    //���� �н� ���ſ���
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

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnum
{
    public enum ESelectCategory
    {
        None,

        Menu,
        Hero,
        Stat,
        Challenge,
        HeroMenu,
        Item,
    }

    //로비 메뉴 카테고리
    public enum EMenuCategory
    {
        None,

        //TOP
        Notice = 10,
        Mail = 11,
        Setting = 12,

        //LEFT
        AdBuff = 20,
        ConnectReward = 21,

        //RIGHT
        GamePass = 30,
        Attendance = 31, 
        Inventory = 32,

        //BOTTOM
        Hero = 40,
        HeroPos = 41,
        Challenge = 42,
        Shop = 43,
        Quest = 34,
    }

    private ESelectCategory selectCategory;
    private EMenuCategory menuCategory;

    public ESelectCategory SelectCategory => selectCategory;
    public EMenuCategory MenuCategory => menuCategory;

    public void SetSelectCategory(ESelectCategory type)
    {
        selectCategory = type;
    }
    public void SetMenuCategory(EMenuCategory type)
    {
        menuCategory = type;
    }
}


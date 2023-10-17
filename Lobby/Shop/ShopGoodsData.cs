using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopGoodsData : ShopData
{
    private string title;

    private string atlasPath;
    private string itemImgPath;
    private int itemCount;

    private string cashAtlasPath;
    private string cashImgPath;
    private int cashCount;

    private Action callback;

    public string Title => title;

    public string AtlasPath => atlasPath;
    public string ItemImgPath => itemImgPath;
    public int ItemCount => itemCount;

    public string CashAtlasPath => cashAtlasPath;
    public string CashImgPath => cashImgPath;
    public int CashCount => cashCount;

    public Action Callback => callback;

    public void SetTitle(string title)
    {
        this.title = title;
    }

    public void SetAtlasPath(string atlasPath) 
    {
        this.atlasPath = atlasPath;
    }

    public void SetItemImgPath(string itemImgPath)
    {
        this.itemImgPath = itemImgPath;
    }

    public void SetItemCount(int itemCount)
    {
        this.itemCount= itemCount;
    }

    public void SetCashAtlasPath(string cashAtlasPath)
    {
        this.cashAtlasPath = cashAtlasPath;
    }

    public void SetCashImgPath(string cashImgPath)
    {
        this.cashImgPath = cashImgPath;
    }

    public void SetCashCount(int cashCount)
    {
        this.cashCount = cashCount;
    }

    public void SetCallback(Action callback)
    {
        this.callback = callback;
    }
}

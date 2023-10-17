using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PackageData : ShopData
{
    private int uid;
    private string title;
    private string desc;
    private List<BaseItemData> itemList;
    private int cashCount;

    public int UID => uid;
    public string Title => title;
    public string Desc => desc;
    public List<BaseItemData> ItemList => itemList;

    public int CashCount => cashCount;

    //юс╫ц
    public void SetUID(int uid)
    {
        this.uid = uid;
    }

    public void SetTitle(string title)
    {
        this.title = title;
    }

    public void SetDesc(string desc)
    {
        this.desc = desc;
    }

    public void SetItemList(List<BaseItemData> itemList)
    {
        this.itemList = itemList;
    }

    public void SetCashCount(int cashCount)
    {
        this.cashCount = cashCount;
    }

}

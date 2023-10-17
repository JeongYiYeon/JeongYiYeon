using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnhancedUI.EnhancedScroller;

public class MailData
{
    public string senderName { get; private set; }
    public string title { get; private set; }
    public string desc { get; private set; }
    public List<BaseItemData> itemList { get; private set; }

    public bool isExpanded;

    public float collapsedSize;

    public float expandedSize;

    public Tween.TweenType tweenType;

    public float tweenTimeExpand;

    public float tweenTimeCollapse;

    public float Size
    {
        get
        {
            return isExpanded ? expandedSize : collapsedSize;
        }
    }

    public float SizeDifference
    {
        get
        {
            return expandedSize - collapsedSize;
        }
    }

    public void SetSenderName(string senderName)
    {
        this.senderName = senderName;
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
}

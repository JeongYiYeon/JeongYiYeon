using EnhancedUI.EnhancedScroller;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MailItem : EnhancedScrollerCellView
{
    [SerializeField]
    private TMP_Text labelSender = null;
    [SerializeField]
    private TMP_Text labelTitle = null;

    [SerializeField]
    private BaseItem[] mailRewardItems = null;

    [SerializeField]
    private GameObject descGo = null;
    [SerializeField]
    private TMP_Text labelDesc = null;

    [SerializeField]
    private Tween tween;

    private LayoutElement layoutElement;

    private MailData data;

    private Action<int, int> initializeTween;
    private Action<int, int, float, float> updateTween;
    private Action<int, int> endTween;

    private void Start()
    {
        layoutElement = this.GetComponent<LayoutElement>();
    }

    public void SetData(MailData data, int dataIndex)
    {
        this.data = data;
        this.dataIndex = dataIndex;

        labelSender.text = $"발신자 : {data.senderName}";
        labelTitle.text = $"제목 : {data.title}";

        for(int i = 0; i < mailRewardItems.Length; i++) 
        {
            mailRewardItems[i].gameObject.SetActive(false);
        }        

        for(int i = 0; i < data.itemList.Count ; i++) 
        {        
            mailRewardItems[i].InitItem(data.itemList[i]);
            mailRewardItems[i].gameObject.SetActive(true);
        }

        labelDesc.text = data.desc;

        descGo.SetActive(data.isExpanded);
    }

    public void SetInitTween(Action<int, int> initializeTween)
    {
        this.initializeTween = initializeTween;
    }

    public void SetUpdateTween(Action<int, int, float, float> updateTween)
    {
        this.updateTween = updateTween;
    }

    public void SetEndTween(Action<int, int> endTween)
    {
        this.endTween = endTween;
    }

    public void BeginTween()
    {
        descGo.SetActive(false);

        if (!data.isExpanded)
        {
            layoutElement.minHeight = data.expandedSize;

            if (data.tweenType == Tween.TweenType.immediate)
            {
                TweenCompleted();
                return;
            }

            StartCoroutine(tween.TweenPosition(data.tweenType, data.tweenTimeCollapse, data.expandedSize, data.collapsedSize, TweenUpdated, TweenCompleted));
        }
        else
        {
            layoutElement.minHeight = data.collapsedSize;

            if (data.tweenType == Tween.TweenType.immediate)
            {
                TweenCompleted();
                return;
            }

            StartCoroutine(tween.TweenPosition(data.tweenType, data.tweenTimeExpand, data.collapsedSize, data.expandedSize, TweenUpdated, TweenCompleted));
        }
    }

    private void TweenUpdated(float newValue, float delta)
    {
       layoutElement.minHeight += delta;

        if (updateTween != null)
        {
            updateTween(dataIndex, cellIndex, newValue, delta);
        }
    }

    private void TweenCompleted()
    {
        if (data.isExpanded)
        {
            descGo.SetActive(true);
        }

        if (endTween != null)
        {
            endTween(dataIndex, cellIndex);
        }
    }

    #region OnClick

    public void OnClickMailDesc()
    {
        if (initializeTween != null)
        {
            initializeTween(dataIndex, cellIndex);
        }
    }


    public void OnClickMailRecive()
    {
        MailPopup mail = UIManager.Instance.GetPopup(BasePopup.EPopupType.Mail) as MailPopup;
        mail.ReciveMail(data);
    }
    #endregion
}

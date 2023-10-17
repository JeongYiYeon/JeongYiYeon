using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemToolTip : MonoSingleton<ItemToolTip>
{
    [SerializeField]
    private RectTransform bgRectTf = null;
    [SerializeField]
    private RectTransform descRectTf = null;

    [SerializeField]
    private TMP_Text labelTitle = null;
    [SerializeField]
    private TMP_Text labelDesc = null;

    public void InitItemToolTip(BaseItem.EItemCategory itemCategory, string title, string desc)
    {
        SetParent();

        switch (itemCategory)
        {
            case BaseItem.EItemCategory.EQUIP:
                break;
            case BaseItem.EItemCategory.ETC:
                break;
        }

        labelTitle.text = title;
        labelDesc.text = desc;
    }

    private void SetParent()
    {
        this.transform.SetParent(UIManager.Instance.CanvasTf);
        this.transform.localPosition = Vector3.zero;
        this.transform.localScale = Vector3.one;

        this.transform.SetAsLastSibling();
    }

    public void SetPosition(Vector3 pos)
    {
        this.transform.position = pos;
    }
    
    public void SetAttachToolTipSize()
    {
        Canvas.ForceUpdateCanvases();
        bgRectTf.sizeDelta = new Vector2(descRectTf.rect.width - descRectTf.anchoredPosition.x, descRectTf.rect.height - descRectTf.anchoredPosition.y);
    }

    //중간 기준으로 왼쪽에 있는지 확인
    public void SetRotation(bool isLeft)
    {
        if(isLeft == true)
        {
            this.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            labelTitle.transform.localRotation = Quaternion.Euler(0, 180f, 0);
            labelDesc.transform.localRotation = Quaternion.Euler(0, 180f, 0);
        }
        else
        {
            this.transform.localRotation = Quaternion.identity;
            labelTitle.transform.localRotation = Quaternion.identity;
            labelDesc.transform.localRotation = Quaternion.identity;
        }
    }

    public void Active()
    {
        this.gameObject.SetActive(true);
    }
    public void DeActive()
    {
        this.gameObject.SetActive(false);
    }
}

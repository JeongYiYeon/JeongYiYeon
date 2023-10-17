using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EnhancedUI.EnhancedScroller;
using TMPro;

public class MailPopup : BasePopup
{
    [SerializeField]
    private TMP_Text labelMailCount = null;
    [SerializeField]
    private MailScrollController scrollViewController = null;

    private List<MailData> mailDataList = new List<MailData>();

    public void InitMailData()
    {
        for (int i = 0; i < 10; i++)
        {
            MailData mailData = new MailData();

            mailData.SetSenderName($"운영자{i + 1}");
            mailData.SetTitle($"메일 테스트{i + 1}");
            mailData.SetDesc($"메일 테스트{i + 1}에 대한 상세 설명을 적는 공간입니다.");

            List<BaseItemData> itemList = new List<BaseItemData>();

            for (int j = 0; j < Random.Range(1, 4); j++)
            {
                BaseItemData item = new BaseItemData();
                item.SetTitle($"아이템 {j + 1}번");

                if (j % 2 == 1)
                {
                    item.SetDesc($"아이템 {j + 1}번에 대한 설명을 이것 저것 적어봐요.\n 이것은 장비 아이템 입니다.");
                    item.SetItemCategory(BaseItem.EItemCategory.EQUIP);
                    item.SetAtlasPath("Item");
                    item.SetImgItemPath("Item_Icon_001");
                }
                else
                {
                    item.SetDesc($"아이템 {j + 1}번에 대한 설명을 이것 저것 적어봐요.\n 이것은 업글용 아이템 입니다.");
                    item.SetItemCategory(BaseItem.EItemCategory.ETC);
                    item.SetAtlasPath("Item");
                    item.SetImgItemPath("Item_Icon_001");
                }

                item.SetItemCount(Random.Range(1, 999));

                itemList.Add(item);
            }       

            mailData.SetItemList(itemList);

            mailData.isExpanded = false;
            mailData.expandedSize = 590f;
            mailData.collapsedSize = 240f;
            mailData.tweenType = Tween.TweenType.easeInOutSine;
            mailData.tweenTimeExpand = 0.5f;
            mailData.tweenTimeCollapse = 0.5f;

            mailDataList.Add(mailData);
        }

        scrollViewController.SetMailInfo(mailDataList);

        RefreshLabelMailListCount();
    }    

    private void RefreshLabelMailListCount()
    {
        labelMailCount.text = "받은 우편 : " + mailDataList.Count.ToString("#,##0");
    }

    public void ReciveMail(MailData data)
    {
        mailDataList.Remove(data);

        scrollViewController.ScrollViewRefresh();

        RefreshLabelMailListCount();
    }

    #region OnClick
    public void OnClickMailReciveAll()
    {
        for(int i = mailDataList.Count - 1; i >= 0; i--) 
        {
            ReciveMail(mailDataList[i]);
        }
    }
    #endregion
}

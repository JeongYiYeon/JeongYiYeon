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

            mailData.SetSenderName($"���{i + 1}");
            mailData.SetTitle($"���� �׽�Ʈ{i + 1}");
            mailData.SetDesc($"���� �׽�Ʈ{i + 1}�� ���� �� ������ ���� �����Դϴ�.");

            List<BaseItemData> itemList = new List<BaseItemData>();

            for (int j = 0; j < Random.Range(1, 4); j++)
            {
                BaseItemData item = new BaseItemData();
                item.SetTitle($"������ {j + 1}��");

                if (j % 2 == 1)
                {
                    item.SetDesc($"������ {j + 1}���� ���� ������ �̰� ���� �������.\n �̰��� ��� ������ �Դϴ�.");
                    item.SetItemCategory(BaseItem.EItemCategory.EQUIP);
                    item.SetAtlasPath("Item");
                    item.SetImgItemPath("Item_Icon_001");
                }
                else
                {
                    item.SetDesc($"������ {j + 1}���� ���� ������ �̰� ���� �������.\n �̰��� ���ۿ� ������ �Դϴ�.");
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
        labelMailCount.text = "���� ���� : " + mailDataList.Count.ToString("#,##0");
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

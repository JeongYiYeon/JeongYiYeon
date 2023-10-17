using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;
using Coffee.UIExtensions;

public class EquipmentItem : BaseItem
{
    [SerializeField]
    private BaseItem item = null;
    [SerializeField]
    private AtlasImage imgEquipAlram = null;
    [SerializeField]
    private TMP_Text labelUpgradeCnt = null;

    private EquipItemData equipItemData = null;

    public virtual void SetData(EquipItemData equipItemData)
    {
        if (equipItemData == null)
        {
            this.gameObject.SetActive(false);
            return;
        }
        this.equipItemData = equipItemData;

        item.InitItem(equipItemData);

        labelUpgradeCnt.gameObject.SetActive(equipItemData.UpgradeCnt > 0);
        labelUpgradeCnt.text = $"+{equipItemData.UpgradeCnt}";

        if (imgEquipAlram != null)
        {
            imgEquipAlram.gameObject.SetActive(equipItemData.EquipCharacterUID > 0);
        }

        this.gameObject.SetActive(true);
    }

    public void OnClickEquipmentInfoPopup()
    {
        if (equipItemData == null)
        {
            return;
        }

        ItemInfoPopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.ItemInfo) as ItemInfoPopup;

        popup.SetItemData(equipItemData);
        popup.Active();
        

        Debug.LogError("ÀåºñÆË¾÷Ã¢ ¿ÀÇÂ");
    }
}

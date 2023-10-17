using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class ItemUpgrade : BasePopup
{
    [SerializeField]
    private TMP_Text labelDesc = null;

    [SerializeField]
    private BaseItem item = null;
    [SerializeField]
    private TMP_Text labelItemUpgradeCnt = null;
    [SerializeField]
    private TMP_Text labelItemName = null;
    [SerializeField]
    private TMP_Text labelItemType = null;
    [SerializeField]
    private TMP_Text labelItemDesc = null;
    [SerializeField]
    private TMP_Text labelItemOption = null;

    [SerializeField]
    private GameObject upgradeGoodsInfoGo = null;
    [SerializeField]
    private BaseItem[] upgradeItems = null;
    [SerializeField]
    private TMP_Text[] labelsUpgradeItemCnt = null;

    private EquipItemData equipItemData = null;
    private DataItemEnchant enchantData = null;

    private bool isAvailableUpgrade = false;

    private void OnEnable()
    {
        SetType(EPopupType.ItemUpgrade);
    }

    public override void Active()
    {
        base.Active();
    }

    public void SetUpgradeItemInfo(EquipItemData data)
    {
        isAvailableUpgrade = false;

        equipItemData = data;

        enchantData = DataManager.Instance.DataHelper.ItemEnchant.Find(x =>
        x.ITEM_UID == data.ItemUID && x.BEFORE_ITEM_LEVEL == data.UpgradeCnt);

        if (enchantData == null)
        {
            AlramPopup alramPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
            alramPopup.SetButtonType(BasePopup.EButtonType.One);
            alramPopup.SetTitle("����");
            alramPopup.SetDesc("��ȭ �ȵ�");
            alramPopup.SetConfirmBtLabel("Ȯ��");
            alramPopup.SetConfirmCallBack(() => { alramPopup.DeActive(); });
            alramPopup.Active();
            return;
        }

        EquipmentItem equipmentItem = (EquipmentItem)item;
        equipmentItem.SetData(data);

        labelItemName.text = $"������ �̸� : {data.Title}";
        labelItemType.text = $"������ Ÿ�� : {GetItemType(data)}";
        labelItemDesc.text = $"���� : {data.Desc}";

        labelDesc.text = "���� Ȯ�� 100%, ���� �� ��� �Ҹ�";

        StringBuilder options = new StringBuilder();

        options = GetItemAddStat(data, options);

        if (data.Options != null && data.Options.Count > 0)
        {
            for (int i = 0; i < data.Options.Count; i++)
            {
                options.Append(GetItemOption(data.Options[i]));
                if (i != data.Options.Count - 1)
                {
                    options.Append("\n");
                }
            }

            labelItemOption.text = $"���� ȿ�� :\n {options}";
        }

        labelItemUpgradeCnt.gameObject.SetActive(data.UpgradeCnt > 0);

        if (data.UpgradeCnt > 0)
        {
            labelItemUpgradeCnt.text = $"+{data.UpgradeCnt}";
        }

        if (enchantData == null)
        {
            isAvailableUpgrade = false;
            upgradeGoodsInfoGo.SetActive(false);
            labelDesc.text = "";
        }
        else
        {
            upgradeGoodsInfoGo.SetActive(true);

            labelDesc.text = "���� Ȯ�� 100%, ���� �� ��� �Ҹ�";


            //labelAtk.text = $"�� ���ݷ� : {data.CharacterData.Damage.ToString("#,##0")} <color=red>(+{addAtk})</color>";
            //labelDef.text = $"�� ���� : {data.CharacterData.Defense.ToString("#,##0")} <color=red>(+{addDef})</color>";
            //labelHp.text = $"�� ü�� : {data.CharacterData.HP.ToString("#,##0")} <color=red>(+{addHp})</color>";
            //labelCri.text = $"ũ��Ƽ�� Ȯ�� : {data.CharacterData.Critical.ToString("F2")}%<color=red>(+{addCri})</color>";

            bool isEnoughFirstMaterial = SetUpgradeItemData(0, enchantData.ENCHANT_ITEM_1, enchantData.ENCHANT_ITEM_COUNT_1);
            bool isEnoughSecondMaterial = SetUpgradeItemData(1, enchantData.ENCHANT_ITEM_2, enchantData.ENCHANT_ITEM_COUNT_2);
            bool isEnoughGoods = SetUpgradeItemData(2, enchantData.ENCHANT_ITEM_3, enchantData.ENCHANT_ITEM_COUNT_3);

            isAvailableUpgrade = isEnoughFirstMaterial && isEnoughSecondMaterial && isEnoughGoods;
        }
    }

    private string GetItemType(BaseItemData itemData)
    {
        string itemTypeName = "";

        if (itemData.ItemCategory == BaseItem.EItemCategory.EQUIP)
        {
            EquipItemData equipItemData = (EquipItemData)itemData;

            switch (equipItemData.EquipItemType)
            {
                case EquipItemData.EEquipItemType.WEAPON:
                    itemTypeName = "����";
                    break;
                case EquipItemData.EEquipItemType.HELM:
                    itemTypeName = "����";
                    break;
                case EquipItemData.EEquipItemType.GLOVE:
                    itemTypeName = "�尩";
                    break;
                case EquipItemData.EEquipItemType.ARMOR:
                    itemTypeName = "����";
                    break;
                case EquipItemData.EEquipItemType.ACCESSORY:
                    itemTypeName = "��ű�";
                    break;
                case EquipItemData.EEquipItemType.BOOTS:
                    itemTypeName = "�Ź�";
                    break;
            }
        }
        else
        {
            switch (itemData.ItemCategory)
            {
                case BaseItem.EItemCategory.MONEY:
                    itemTypeName = "��ȭ";
                    break;
                case BaseItem.EItemCategory.ETC:
                    itemTypeName = "��Ÿ";
                    break;
            }
        }

        return itemTypeName;
    }

    private StringBuilder GetItemAddStat(EquipItemData itemData, StringBuilder stringBuilder)
    {
        float addAtk = 0;
        float addHp = 0;

        if (enchantData != null)
        {
            addAtk = (float)enchantData.ATTACK_DAM_UP;
            addHp = (float)enchantData.HP_UP;

            if (itemData.Dam > 0)
            {
                if (addAtk > 0)
                {
                    stringBuilder.Append($"\t <color=red>���ݷ� {itemData.Dam + itemData.GetUpgradeStat().atk} +{addAtk} ����</color>\n");
                }
                else
                {
                    stringBuilder.Append($"\t <color=red>���ݷ� {itemData.Dam + itemData.GetUpgradeStat().atk} ����</color>\n");
                }
            }
            else
            {
                if (addAtk > 0)
                {
                    stringBuilder.Append($"\t <color=red>���ݷ� {addAtk} ����</color>\n");
                }
            }            

            if (itemData.HP > 0)
            {
                if (addHp > 0)
                {
                    stringBuilder.Append($"\t <color=yellow>ü�� {itemData.HP + itemData.GetUpgradeStat().hp} +{addHp} ����</color>\n");
                }
                else
                {
                    stringBuilder.Append($"\t <color=yellow>ü�� {itemData.HP + itemData.GetUpgradeStat().hp} ����</color>\n");
                }
            }
            else
            {
                if (addHp > 0)
                {
                    stringBuilder.Append($"\t <color=yellow>ü�� {addHp} ����</color>\n");
                }
            }
        }

        else
        {
            if (itemData.Dam > 0)
            {
                stringBuilder.Append($"\t <color=red>���ݷ� {itemData.Dam + itemData.GetUpgradeStat().atk} ����</color>\n");
            }            
            if (itemData.HP > 0)
            {
                stringBuilder.Append($"\t <color=yellow>ü�� {itemData.HP + itemData.GetUpgradeStat().hp} ����</color>\n");
            }
        }

        return stringBuilder;
    }

    private string GetItemOption(BaseOptionData option)
    {
        string itemOptionName = "";

        if (enchantData != null)
        {
            BaseOptionData.EOptionValueType optionType = BaseOptionData.EOptionValueType.NONE;

            if (Enum.TryParse < BaseOptionData.EOptionValueType>(enchantData.ENCHANT_OPTION_1, out optionType))
            {
                if (optionType != BaseOptionData.EOptionValueType.NONE)
                {
                    //���� �ɼ��� ������ �ڿ� �߰�
                    if (option.ValueType == optionType)
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} " +
                            $"+{GetItemOptionValue(optionType, enchantData.OPTION_VALUE_1)} {GetItemOptionValueType(optionType)} ����</color>";
                    }
                    else
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(optionType, enchantData.OPTION_VALUE_1)} {GetItemOptionValueType(optionType)} ����</color>";
                    }
                }
                else
                {
                    itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} ����</color>";
                }
            }
            if (Enum.TryParse<BaseOptionData.EOptionValueType>(enchantData.ENCHANT_OPTION_2, out optionType))
            {
                if (optionType != BaseOptionData.EOptionValueType.NONE)
                {
                    if (option.ValueType == optionType)
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} " +
                            $"+{GetItemOptionValue(optionType, enchantData.OPTION_VALUE_2)} {GetItemOptionValueType(optionType)} ����</color>";
                    }
                    else
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(optionType, enchantData.OPTION_VALUE_2)} {GetItemOptionValueType(optionType)} ����</color>";
                    }
                }
                else
                {
                    itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} ����</color>";
                }
            }
            if (Enum.TryParse<BaseOptionData.EOptionValueType>(enchantData.ENCHANT_OPTION_3, out optionType))
            {
                if (optionType != BaseOptionData.EOptionValueType.NONE)
                {
                    if (option.ValueType == optionType)
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} " +
                            $"+{GetItemOptionValue(optionType, enchantData.OPTION_VALUE_3)} {GetItemOptionValueType(optionType)} ����</color>";
                    }
                    else
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(optionType, enchantData.OPTION_VALUE_3)} {GetItemOptionValueType(optionType)} ����</color>";
                    }
                }
                else
                {
                    itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} ����</color>";
                }
            }
            if (Enum.TryParse<BaseOptionData.EOptionValueType>(enchantData.ENCHANT_OPTION_4, out optionType))
            {
                if (optionType != BaseOptionData.EOptionValueType.NONE)
                {
                    if (option.ValueType == optionType)
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} " +
                            $"+{GetItemOptionValue(optionType, enchantData.OPTION_VALUE_4)} {GetItemOptionValueType(optionType)} ����</color>";
                    }
                    else
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(optionType, enchantData.OPTION_VALUE_4)} {GetItemOptionValueType(optionType)} ����</color>";
                    }
                }
                else
                {
                    itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} ����</color>";
                }
            }
            if (Enum.TryParse<BaseOptionData.EOptionValueType>(enchantData.ENCHANT_OPTION_5, out optionType))
            {
                if (optionType != BaseOptionData.EOptionValueType.NONE)
                {
                    if (option.ValueType == optionType)
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} " +
                            $"+{GetItemOptionValue(optionType, enchantData.OPTION_VALUE_5)} {GetItemOptionValueType(optionType)} ����</color>";
                    }
                    else
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(optionType, enchantData.OPTION_VALUE_5)} {GetItemOptionValueType(optionType)} ����</color>";
                    }
                }
                else
                {
                    itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} ����</color>";
                }
            }
        }
        else
        {
            itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} ����</color>";
        }
        return itemOptionName;
    }

    private string GetOptionName(BaseOptionData option)
    {
        string optionName = "";

        switch (option.Type)
        {
            case BaseOptionData.EOptionType.ATTACK_DAM_UP:
                optionName = "\t <color=red>���ݷ�";
                break;
            case BaseOptionData.EOptionType.HP_UP:
                optionName = "\t <color=green>ü��";
                break;
            case BaseOptionData.EOptionType.CRIRATE_UP:
                optionName = "\t <color=red>ũ��Ƽ�� Ȯ��";
                break;
            case BaseOptionData.EOptionType.ATTACK_SPEED_UP:
                optionName = "\t <#F100FF>���ݼӵ�";
                break;
        }

        return optionName;
    }

    private float GetItemOptionValue(BaseOptionData option)
    {
        switch (option.ValueType)
        {
            case BaseOptionData.EOptionValueType.PER:
                return option.Value - 100f;
            case BaseOptionData.EOptionValueType.PLUS:
                return option.Value;
            default:
                return option.Value;
        }
    }
    private double GetItemOptionValue(BaseOptionData.EOptionValueType type, double value)
    {
        switch (type)
        {
            case BaseOptionData.EOptionValueType.PER:
                return value - 100f;
            case BaseOptionData.EOptionValueType.PLUS:
                return value;
            default:
                return value;
        }
    }

    private string GetItemOptionValueType(BaseOptionData option)
    {
        string itemOptionValueTypeName = "";

        switch (option.ValueType)
        {
            case BaseOptionData.EOptionValueType.PER:
                itemOptionValueTypeName = "%";
                break;
            case BaseOptionData.EOptionValueType.PLUS:
                itemOptionValueTypeName = "��ġ ��ŭ";
                break;
        }
        return itemOptionValueTypeName;
    }

    private string GetItemOptionValueType(BaseOptionData.EOptionValueType type)
    {
        string itemOptionValueTypeName = "";

        switch (type)
        {
            case BaseOptionData.EOptionValueType.PER:
                itemOptionValueTypeName = "%";
                break;
            case BaseOptionData.EOptionValueType.PLUS:
                itemOptionValueTypeName = "��ġ ��ŭ";
                break;
        }
        return itemOptionValueTypeName;
    }

    private BaseItemData GetUpgradeItemData(int itemUID)
    {
        DataItem itemData = DataManager.Instance.DataHelper.Item.Find(x => x.UID == itemUID);

        BaseItemData upgradeItemData = new BaseItemData();

        upgradeItemData.SetDataItem(itemData);
        upgradeItemData.SetItemUID(itemData.UID);
        upgradeItemData.SetTitle(DataManager.Instance.GetLocalization(itemData.ITEM_NAME));
        upgradeItemData.SetDesc(DataManager.Instance.GetLocalization(itemData.ITEM_DESC));
        upgradeItemData.SetItemCategory(Enum.Parse<BaseItem.EItemCategory>(itemData.ITEM_CATEGORY));
        upgradeItemData.SetItemGrade(itemData.ITEM_GRADE);
        upgradeItemData.SetAtlasPath(itemData.ITEM_ICON_ATLAS);
        upgradeItemData.SetImgItemPath(itemData.ITEM_ICON);

        return upgradeItemData;
    }

    private bool SetUpgradeItemData(int idx, int itemUID, int itemCnt)
    {
        upgradeItems[idx].InitItem(GetUpgradeItemData(itemUID));

        string goodsString = "";

        bool isEnoughGoods = false;

        switch (UserData.Instance.user.Goods.GetGoodsType(itemUID))
        {
            //���̾�
            case UserData.EGoodsType.PURCHASECASH:
            case UserData.EGoodsType.CASH:
                goodsString = UserData.Instance.user.Goods.GetGoodsString(UserData.EGoodsType.CASH);

                if (UserData.Instance.user.Goods.TotalCash >= itemCnt)
                {
                    isEnoughGoods = true;
                }

                break;

            //���
            case UserData.EGoodsType.GOLD:
                goodsString = UserData.Instance.user.Goods.GetGoodsString(UserData.EGoodsType.GOLD);

                if (UserData.Instance.user.Goods.Gold >= itemCnt)
                {
                    isEnoughGoods = true;
                }

                break;

            default:
                goodsString = UserData.Instance.user.Item.GetItemCount(itemUID).ToString("#,##0");

                if (UserData.Instance.user.Item.GetItemCount(itemUID) >= itemCnt)
                {
                    isEnoughGoods = true;
                }

                break;
        }

        labelsUpgradeItemCnt[idx].text = $"{itemCnt:#,##0} / {goodsString}";

        return isEnoughGoods;
    }

    public void OnClickUpgrade()
    {
        if (equipItemData == null)
        {
            return;
        }

        if (isAvailableUpgrade == false)
        {
            AlramPopup alramPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
            alramPopup.SetButtonType(BasePopup.EButtonType.One);
            alramPopup.SetTitle("����");
            alramPopup.SetDesc("��� ����");
            alramPopup.SetConfirmBtLabel("Ȯ��");
            alramPopup.SetConfirmCallBack(() => { alramPopup.DeActive(); });
            alramPopup.Active();

            return;
        }

        AlramPopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
        popup.SetButtonType(BasePopup.EButtonType.Two);
        popup.SetTitle("��ȭ");
        popup.SetDesc("��ȭ �Ҳ���?");
        popup.SetConfirmBtLabel("��ȭ");
        popup.SetCancelBtLabel("����");
        popup.SetConfirmCallBack(async () =>
        {
            await NetworkPacketItem.Instance.TaskUpgradeItem(equipItemData.GetUID, () =>
            {
                BaseItemData itemData = UserData.Instance.user.Item.InventoryDic[BaseItem.EItemCategory.EQUIP].Find(x =>
                x.GetUID == equipItemData.GetUID);

                if (itemData != null)
                {
                    SetUpgradeItemInfo((EquipItemData)itemData);
                }

                ItemInfoPopup itemInfoPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.ItemInfo) as ItemInfoPopup;
                itemInfoPopup.SetItemData(itemData);
                itemInfoPopup.SetItemInfo(itemData);

                if (LobbyManager.Instance != null)
                {
                    LobbyManager.Instance.RefreshGoodsLabel();
                    LobbyManager.Instance.RefreshHeroMenu();
                }

                popup.DeActive();
            });
        });
        popup.Active();
    }
}

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
            alramPopup.SetTitle("오류");
            alramPopup.SetDesc("강화 안됨");
            alramPopup.SetConfirmBtLabel("확인");
            alramPopup.SetConfirmCallBack(() => { alramPopup.DeActive(); });
            alramPopup.Active();
            return;
        }

        EquipmentItem equipmentItem = (EquipmentItem)item;
        equipmentItem.SetData(data);

        labelItemName.text = $"아이템 이름 : {data.Title}";
        labelItemType.text = $"아이템 타입 : {GetItemType(data)}";
        labelItemDesc.text = $"설명 : {data.Desc}";

        labelDesc.text = "성장 확률 100%, 성장 시 재료 소모";

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

            labelItemOption.text = $"장착 효과 :\n {options}";
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

            labelDesc.text = "성장 확률 100%, 성장 시 재료 소모";


            //labelAtk.text = $"총 공격력 : {data.CharacterData.Damage.ToString("#,##0")} <color=red>(+{addAtk})</color>";
            //labelDef.text = $"총 방어력 : {data.CharacterData.Defense.ToString("#,##0")} <color=red>(+{addDef})</color>";
            //labelHp.text = $"총 체력 : {data.CharacterData.HP.ToString("#,##0")} <color=red>(+{addHp})</color>";
            //labelCri.text = $"크리티컬 확률 : {data.CharacterData.Critical.ToString("F2")}%<color=red>(+{addCri})</color>";

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
                    itemTypeName = "무기";
                    break;
                case EquipItemData.EEquipItemType.HELM:
                    itemTypeName = "모자";
                    break;
                case EquipItemData.EEquipItemType.GLOVE:
                    itemTypeName = "장갑";
                    break;
                case EquipItemData.EEquipItemType.ARMOR:
                    itemTypeName = "갑옷";
                    break;
                case EquipItemData.EEquipItemType.ACCESSORY:
                    itemTypeName = "장신구";
                    break;
                case EquipItemData.EEquipItemType.BOOTS:
                    itemTypeName = "신발";
                    break;
            }
        }
        else
        {
            switch (itemData.ItemCategory)
            {
                case BaseItem.EItemCategory.MONEY:
                    itemTypeName = "재화";
                    break;
                case BaseItem.EItemCategory.ETC:
                    itemTypeName = "기타";
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
                    stringBuilder.Append($"\t <color=red>공격력 {itemData.Dam + itemData.GetUpgradeStat().atk} +{addAtk} 증가</color>\n");
                }
                else
                {
                    stringBuilder.Append($"\t <color=red>공격력 {itemData.Dam + itemData.GetUpgradeStat().atk} 증가</color>\n");
                }
            }
            else
            {
                if (addAtk > 0)
                {
                    stringBuilder.Append($"\t <color=red>공격력 {addAtk} 증가</color>\n");
                }
            }            

            if (itemData.HP > 0)
            {
                if (addHp > 0)
                {
                    stringBuilder.Append($"\t <color=yellow>체력 {itemData.HP + itemData.GetUpgradeStat().hp} +{addHp} 증가</color>\n");
                }
                else
                {
                    stringBuilder.Append($"\t <color=yellow>체력 {itemData.HP + itemData.GetUpgradeStat().hp} 증가</color>\n");
                }
            }
            else
            {
                if (addHp > 0)
                {
                    stringBuilder.Append($"\t <color=yellow>체력 {addHp} 증가</color>\n");
                }
            }
        }

        else
        {
            if (itemData.Dam > 0)
            {
                stringBuilder.Append($"\t <color=red>공격력 {itemData.Dam + itemData.GetUpgradeStat().atk} 증가</color>\n");
            }            
            if (itemData.HP > 0)
            {
                stringBuilder.Append($"\t <color=yellow>체력 {itemData.HP + itemData.GetUpgradeStat().hp} 증가</color>\n");
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
                    //동일 옵션이 있으면 뒤에 추가
                    if (option.ValueType == optionType)
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} " +
                            $"+{GetItemOptionValue(optionType, enchantData.OPTION_VALUE_1)} {GetItemOptionValueType(optionType)} 증가</color>";
                    }
                    else
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(optionType, enchantData.OPTION_VALUE_1)} {GetItemOptionValueType(optionType)} 증가</color>";
                    }
                }
                else
                {
                    itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} 증가</color>";
                }
            }
            if (Enum.TryParse<BaseOptionData.EOptionValueType>(enchantData.ENCHANT_OPTION_2, out optionType))
            {
                if (optionType != BaseOptionData.EOptionValueType.NONE)
                {
                    if (option.ValueType == optionType)
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} " +
                            $"+{GetItemOptionValue(optionType, enchantData.OPTION_VALUE_2)} {GetItemOptionValueType(optionType)} 증가</color>";
                    }
                    else
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(optionType, enchantData.OPTION_VALUE_2)} {GetItemOptionValueType(optionType)} 증가</color>";
                    }
                }
                else
                {
                    itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} 증가</color>";
                }
            }
            if (Enum.TryParse<BaseOptionData.EOptionValueType>(enchantData.ENCHANT_OPTION_3, out optionType))
            {
                if (optionType != BaseOptionData.EOptionValueType.NONE)
                {
                    if (option.ValueType == optionType)
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} " +
                            $"+{GetItemOptionValue(optionType, enchantData.OPTION_VALUE_3)} {GetItemOptionValueType(optionType)} 증가</color>";
                    }
                    else
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(optionType, enchantData.OPTION_VALUE_3)} {GetItemOptionValueType(optionType)} 증가</color>";
                    }
                }
                else
                {
                    itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} 증가</color>";
                }
            }
            if (Enum.TryParse<BaseOptionData.EOptionValueType>(enchantData.ENCHANT_OPTION_4, out optionType))
            {
                if (optionType != BaseOptionData.EOptionValueType.NONE)
                {
                    if (option.ValueType == optionType)
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} " +
                            $"+{GetItemOptionValue(optionType, enchantData.OPTION_VALUE_4)} {GetItemOptionValueType(optionType)} 증가</color>";
                    }
                    else
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(optionType, enchantData.OPTION_VALUE_4)} {GetItemOptionValueType(optionType)} 증가</color>";
                    }
                }
                else
                {
                    itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} 증가</color>";
                }
            }
            if (Enum.TryParse<BaseOptionData.EOptionValueType>(enchantData.ENCHANT_OPTION_5, out optionType))
            {
                if (optionType != BaseOptionData.EOptionValueType.NONE)
                {
                    if (option.ValueType == optionType)
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} " +
                            $"+{GetItemOptionValue(optionType, enchantData.OPTION_VALUE_5)} {GetItemOptionValueType(optionType)} 증가</color>";
                    }
                    else
                    {
                        itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(optionType, enchantData.OPTION_VALUE_5)} {GetItemOptionValueType(optionType)} 증가</color>";
                    }
                }
                else
                {
                    itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} 증가</color>";
                }
            }
        }
        else
        {
            itemOptionName = $"{GetOptionName(option)} {GetItemOptionValue(option)} {GetItemOptionValueType(option)} 증가</color>";
        }
        return itemOptionName;
    }

    private string GetOptionName(BaseOptionData option)
    {
        string optionName = "";

        switch (option.Type)
        {
            case BaseOptionData.EOptionType.ATTACK_DAM_UP:
                optionName = "\t <color=red>공격력";
                break;
            case BaseOptionData.EOptionType.HP_UP:
                optionName = "\t <color=green>체력";
                break;
            case BaseOptionData.EOptionType.CRIRATE_UP:
                optionName = "\t <color=red>크리티컬 확률";
                break;
            case BaseOptionData.EOptionType.ATTACK_SPEED_UP:
                optionName = "\t <#F100FF>공격속도";
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
                itemOptionValueTypeName = "수치 만큼";
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
                itemOptionValueTypeName = "수치 만큼";
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
            //다이아
            case UserData.EGoodsType.PURCHASECASH:
            case UserData.EGoodsType.CASH:
                goodsString = UserData.Instance.user.Goods.GetGoodsString(UserData.EGoodsType.CASH);

                if (UserData.Instance.user.Goods.TotalCash >= itemCnt)
                {
                    isEnoughGoods = true;
                }

                break;

            //골드
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
            alramPopup.SetTitle("오류");
            alramPopup.SetDesc("재료 없음");
            alramPopup.SetConfirmBtLabel("확인");
            alramPopup.SetConfirmCallBack(() => { alramPopup.DeActive(); });
            alramPopup.Active();

            return;
        }

        AlramPopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
        popup.SetButtonType(BasePopup.EButtonType.Two);
        popup.SetTitle("강화");
        popup.SetDesc("강화 할꺼임?");
        popup.SetConfirmBtLabel("강화");
        popup.SetCancelBtLabel("안함");
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

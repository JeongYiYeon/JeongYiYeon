using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text;
using System;
using Coffee.UIExtensions;
using UnityEngine.UI;

public class ItemInfoPopup : BasePopup
{
    private enum EState
    {
        Equipment,
        NoEquipment
    }

    [SerializeField]
    private UtilState state = null;
    [SerializeField]
    private UtilState btState = null;

    [SerializeField]
    private BaseItem[] items = null;

    [SerializeField]
    private TMP_Text labelItemName = null;
    [SerializeField]
    private TMP_Text labelItemType = null;
    [SerializeField]
    private TMP_Text labelItemDesc = null;
    [SerializeField]
    private TMP_Text labelItemOption = null;

    [SerializeField]
    private GameObject itemStackGo = null;
    [SerializeField]
    private Slider itemStackSlider = null;
    [SerializeField]
    private TMP_InputField itemStackInputField = null;

    [SerializeField]
    private TMP_Text labelUpgradeBt = null;
    [SerializeField]
    private TMP_Text labelEquipStateBt = null;

    [SerializeField]
    private AtlasImage[] imgSellIcon = null;
    [SerializeField]
    private TMP_Text[] labelSellBt = null;

    private BaseItemData itemData = null;

    private bool isHeroEquipment = false;

    private CharacterData otherEquipmentCharacter = null;       // 아이템을 다른 캐릭이 장착하고 있을때 체크용

    private void OnEnable()
    {
        SetType(EPopupType.ItemInfo);
    }

    public override void Init()
    {
        base.Init();
    }

    public override void ResetLabel()
    {
        base.ResetLabel();

        if (labelItemName != null)
        {
            labelItemName.text = "";
        }

        if (labelItemType != null)
        {
            labelItemType.text = "타입 : ";
        }

        if(labelItemDesc != null)
        {
            labelItemDesc.text = "";
        }

        if(labelItemOption != null)
        {
            labelItemOption.text = "";
        }

        if (labelUpgradeBt != null)
        {
            labelUpgradeBt.text = "강화";
        }

        if (labelEquipStateBt != null)
        {
            labelEquipStateBt.text = "장착 / 해제";
        }

        if (imgSellIcon != null && imgSellIcon.Length > 0)
        {
            for (int i = 0; i < imgSellIcon.Length; i++)
            {
                imgSellIcon[i] = UserData.Instance.user.Goods.GetGoodsImage(itemData.ItemUID, imgSellIcon[i]);
            }
        }

        if (labelSellBt != null && labelSellBt.Length > 0)
        {
            for (int i = 0; i < labelSellBt.Length; i++)
            {
                labelSellBt[i].text = $"{itemData.SellItemCount} 판매";
            }
        }
    }

    public override void Active()
    {
        base.Active();

        if(itemData != null)
        {
            SetItemInfo(itemData);           
        }
    }

    public override void Reset()
    {
        base.Reset();

        isHeroEquipment = false;
        itemData = null;
        otherEquipmentCharacter = null;
    }

    public void SetItemInfo(BaseItemData itemData)
    {
        labelItemName.text = $"{itemData.Title}";
        labelItemType.text = $"타입 : {GetItemType(itemData)}";
        labelItemDesc.text = $"{itemData.Desc}";

        if (itemData.ItemCategory == BaseItem.EItemCategory.EQUIP)
        {
            state.ActiveState(EState.Equipment.ToString());
            btState.ActiveState(EState.Equipment.ToString());

            EquipmentItem equipmentItem = (EquipmentItem)items[(int)EState.Equipment];
            EquipItemData equipmentItemData = (EquipItemData)itemData;
            equipmentItem.SetData(equipmentItemData);

            StringBuilder options = new StringBuilder();

            options = GetItemAddStat(equipmentItemData, options);

            if (equipmentItemData.Options != null && equipmentItemData.Options.Count > 0)
            {
                for (int i = 0; i < equipmentItemData.Options.Count; i++)
                {
                    options.Append(GetItemOption(equipmentItemData.Options[i]));
                    if (i != equipmentItemData.Options.Count - 1)
                    {
                        options.Append("\n");
                    }
                }

                labelItemOption.text = $"{options}";
            }

            //isHeroEquipment = UserData.Instance.IsHeroEquipment(UserData.Instance.SelectCharacter, equipmentItemData.EquipItemType);

            isHeroEquipment = equipmentItemData.EquipCharacterUID > 0;

            if (Hero.Instance != null && Hero.Instance.gameObject.activeSelf)
            {
                otherEquipmentCharacter = UserData.Instance.GetOtherHeroEquipment(equipmentItemData).otherEquipmentHero;

                if (isHeroEquipment == true)
                {
                    if (otherEquipmentCharacter != null)
                    {
                        labelEquipStateBt.text = "교체";
                    }
                    else
                    {
                        labelEquipStateBt.text = "해제";
                    }
                }
                else
                {
                    labelEquipStateBt.text = "장착";
                }
            }

            if (equipmentItemData.IsSellPossible == true)
            {
                labelSellBt[(int)EState.Equipment].text = $"{equipmentItemData.SellItemCount.ToString("#,##0")} 판매";
            }
            else
            {
                labelSellBt[(int)EState.Equipment].text = "판매 불가";
            }
        }
        else
        {
            state.ActiveState(EState.NoEquipment.ToString());
            btState.ActiveState(EState.NoEquipment.ToString());

            items[(int)EState.NoEquipment].InitItem(itemData);

            if (itemData.IsSellPossible == true)
            {
                labelSellBt[(int)EState.NoEquipment].text = $"{itemData.SellItemCount.ToString("#,##0")} 판매";
            }
            else
            {
                labelSellBt[(int)EState.NoEquipment].text = "판매 불가";
            }

            labelItemOption.gameObject.SetActive(false);
        }

        if (itemData.ItemCount > 1)
        {
            itemStackGo.SetActive(true);
            itemStackSlider.minValue = 1;

            float tmpMaxCnt = 0;

            if(itemData.ItemCount > float.MaxValue)
            {
                tmpMaxCnt = float.MaxValue;
            }
            else
            {
                tmpMaxCnt = (float)itemData.ItemCount;
            }

            itemStackSlider.maxValue = tmpMaxCnt;
            itemStackInputField.text = ((int)itemStackSlider.value).ToString();
        }
        else
        {
            itemStackGo.SetActive(false);
        }
    }

    private string GetItemType(BaseItemData itemData)
    {
        string itemTypeName = "";

        if(itemData.ItemCategory == BaseItem.EItemCategory.EQUIP)
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
            switch(itemData.ItemCategory)
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
        if (itemData.Dam > 0)
        {
            stringBuilder.Append($"<#C34242>공격력 {itemData.Dam + itemData.GetUpgradeStat().atk} 증가</color>\n");
        }
        if (itemData.HP > 0)
        {
            stringBuilder.Append($"<#4389AE>체력 {itemData.HP + itemData.GetUpgradeStat().hp} 증가</color>\n");
        }

        return stringBuilder;
    }

    private string GetItemOption(BaseOptionData option)
    {
        string itemOptionName = "";

        switch (option.Type)
        {
            case BaseOptionData.EOptionType.ATTACK_DAM_UP:
                itemOptionName = $"<#C34242>공격력 {GetItemOptionValue(option)} {GetItemOptionValueType(option)} 증가</color>";
                break;
            case BaseOptionData.EOptionType.HP_UP:
                itemOptionName = $"<#4389AE>체력 {GetItemOptionValue(option)} {GetItemOptionValueType(option)} 증가</color>";
                break;
            case BaseOptionData.EOptionType.CRIRATE_UP:
                itemOptionName = $"<#C34281>크리티컬 확률 {GetItemOptionValue(option)} {GetItemOptionValueType(option)} 증가</color>";
                break;
            case BaseOptionData.EOptionType.ATTACK_SPEED_UP:
                itemOptionName = $"<#56A439>공격속도 {GetItemOptionValue(option)} {GetItemOptionValueType(option)} 증가</color>";
                break;
        }

        return itemOptionName;
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

    public void SetItemData(BaseItemData itemData)
    {
        this.itemData = itemData;
    }

    public void OnClickUpgrade()
    {
        if(itemData == null)
        {
            return;
        }

        EquipItemData tmpData = (EquipItemData)itemData;

        if(tmpData == null || tmpData.UpgradeCnt == UserData.Instance.user.Config.ItemLevelMax)
        {
            Debug.LogError("풀강");
            return;
        }

        ItemUpgrade popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.ItemUpgrade) as ItemUpgrade;
        popup.SetUpgradeItemInfo(tmpData);
        popup.Active();
       
    }

    public async void OnClickEquipmentState()
    {
        EquipItemData equipmentItemData = (EquipItemData)itemData;

        if (equipmentItemData != null)
        {
            if (isHeroEquipment == true && otherEquipmentCharacter == null)
            {
                await NetworkPacketItem.Instance.TaskEquipItem(
                       0,
                       equipmentItemData.GetUID,
                       () =>
                       {
                           UserData.SetHeroEquipment(equipmentItemData.EquipCharacterUID, equipmentItemData.EquipItemType, null);

                           LobbyManager.Instance.RefreshHeroMenu(UserData.Instance.user.Character.SelectCharacter.Type);
                           DeActive();

                           LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
                       });
            }
            else
            {
                await NetworkPacketItem.Instance.TaskEquipItem(
                        UserData.Instance.user.Character.SelectCharacter.DataCharacter.UID,
                        equipmentItemData.GetUID,
                        () =>
                        {
                            LobbyManager.Instance.RefreshHeroMenu(UserData.Instance.user.Character.SelectCharacter.Type);
                            DeActive();
                            LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
                        });
            }
        }
    }

    public void OnClickSell()
    {
        DeActive();
    }

    public void OnClickItemStackSliderMinus()
    {
        if(itemStackSlider.value > 2)
        {
            itemStackSlider.value -= 1;
            itemStackInputField.text = ((int)itemStackSlider.value).ToString();
        }
    }
    public void OnClickItemStackSliderPlus()
    {
        if (itemStackSlider.value < itemStackSlider.maxValue)
        {
            itemStackSlider.value += 1;
            itemStackInputField.text = ((int)itemStackSlider.value).ToString();
        }
    }

    public void OnItemStackInputField(TMP_InputField input)
    {
        int value = 0;

        if(int.TryParse(input.text, out value) == true)
        {
            if (value > 0)
            {
                if(itemStackSlider.maxValue > value)
                {
                    value = (int)itemStackSlider.maxValue;
                }

                itemStackSlider.value = value;
            }
        }            
    }
}

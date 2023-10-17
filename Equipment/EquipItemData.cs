using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItemData : BaseItemData
{
    public enum EEquipItemType
    {
        NONE,

        WEAPON = 1,          //¹«±â
        HELM = 2,            //¸ðÀÚ
        GLOVE = 3,           //Àå°©
        ARMOR = 4,           //°©¿Ê
        ACCESSORY = 5,       //Àå½Å±¸
        BOOTS = 6,           //½Å¹ß
    }


    private EEquipItemType equipItemType = EEquipItemType.NONE;

    public EEquipItemType EquipItemType => equipItemType;

    private double dam = 0;
    private double hp = 0;

    private List<BaseOptionData> options = null;
    public double Dam => dam;
    public double HP => hp;
    public int UpgradeCnt => upgradeCnt;

    public List<BaseOptionData> Options => options;

    public int EquipCharacterUID => equipCharacterUID;
    public void SetEquipItemType(EEquipItemType equipItemType)
    {
        this.equipItemType = equipItemType;
    }

    public void SetDam(double dam)
    {
        this.dam = dam;
    }
    public void SetHp(double hp)
    {
        this.hp = hp;
    }

    public void SetOption(BaseOptionData option)
    {
        if(options == null)
        {
            options = new List<BaseOptionData>();
        }

        if (options.Contains(option) == false)
        {
            options.Add(option);
        }
    }

    public void SetOption(int optionUID)
    {
        DataOptionSet option = DataManager.Instance.DataHelper.OptionSet.Find(x => x.UID == optionUID);

        BaseOptionData optionData = new BaseOptionData();

        optionData.SetOptionCategory(Enum.Parse<BaseOptionData.EOptionCategory>(option.OPTION_CATEGORY));
        optionData.SetOptionType(Enum.Parse<BaseOptionData.EOptionType>(option.OPTION_TYPE));
        optionData.SetValueType(Enum.Parse<BaseOptionData.EOptionValueType>(option.VALUE_TYPE));
        if ((BaseOptionData.EOptionActiveType)option.OPTION_VALUE_1 != BaseOptionData.EOptionActiveType.NONE)
        {
            optionData.SetActiveType((BaseOptionData.EOptionActiveType)option.OPTION_VALUE_1);
        }
        optionData.SetValue((float)option.OPTION_VALUE_2);

        if (option != null)
        {
            SetOption(optionData);
        }
    }

    public (float atk, float hp, float cri, float atkSpd) GetEquipmentItemStat(CharacterData character = null)
    {
        float _atk = 0;
        float _hp = 0;
        float cri = 0;
        float atkSpd = 0;

        float addAtk = 0;
        float addHp = 0;
        float addCri = 0f;
        float addAtkSpd = 0f;

        List<DataItemEnchant> tmpEnchantDataList = DataManager.Instance.DataHelper.ItemEnchant.FindAll(x => x.ITEM_UID == ItemUID);

        if (tmpEnchantDataList != null && tmpEnchantDataList.Count > 0)
        {
            for (int i = 0; i < tmpEnchantDataList.Count; i++)
            {
                if (tmpEnchantDataList[i].LATE_ITEM_LEVEL <= upgradeCnt)
                {
                    addAtk += (float)tmpEnchantDataList[i].ATTACK_DAM_UP;
                    addHp += (float)tmpEnchantDataList[i].HP_UP;
                }
            }
        }

        if (Options != null && Options.Count > 0)
        {
            for (int i = 0; i < Options.Count; i++)
            {
                if (Options[i].Type == BaseOptionData.EOptionType.ATTACK_DAM_UP)
                {
                    addAtk += (float)Options[i].CalculateAddValue(DataItem.MAIN_ATTACK_DAM);
                }
                else if (Options[i].Type == BaseOptionData.EOptionType.HP_UP)
                {
                    addHp += (float)Options[i].CalculateAddValue(DataItem.MAIN_HP);
                }

                if (character != null)
                {
                    if (Options[i].Type == BaseOptionData.EOptionType.CRIRATE_UP)
                    {
                        addCri += (float)Options[i].CalculateAddValue(character.Critical);
                    }
                    else if (Options[i].Type == BaseOptionData.EOptionType.ATTACK_SPEED_UP)
                    {
                        addAtkSpd -= (float)Options[i].CalculateAddValue(character.AttackSpd);
                    }
                }
            }
        }

        _atk = (float)DataItem.MAIN_ATTACK_DAM + addAtk;
        _hp = (float)DataItem.MAIN_HP + addHp;

        if (character != null)
        {
            cri = Mathf.Clamp((float)character.Critical + addCri, (float)character.Critical + addCri, 100f);
            atkSpd = Mathf.Clamp((float)character.AttackSpd - addAtkSpd, 0.5f, (float)character.AttackSpd - addAtkSpd);
        }

        return (_atk,  _hp, cri, atkSpd);
    }

    public (float atk, float hp) GetUpgradeStat()
    {
        if (upgradeCnt == 0)
        {
            return (0, 0);
        }

        float _atk = 0;
        float _hp = 0;

        List<DataItemEnchant> tmpEnchantDataList = DataManager.Instance.DataHelper.ItemEnchant.FindAll(x => x.ITEM_UID == ItemUID);

        if (tmpEnchantDataList != null && tmpEnchantDataList.Count > 0)
        {
            for (int i = 0; i < tmpEnchantDataList.Count; i++)
            {
                if (tmpEnchantDataList[i].LATE_ITEM_LEVEL <= upgradeCnt)
                {
                    _atk += (float)tmpEnchantDataList[i].ATTACK_DAM_UP;
                    _hp += (float)tmpEnchantDataList[i].HP_UP;
                }
            }
        }

        return (_atk, _hp);
    }

}

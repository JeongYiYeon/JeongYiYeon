using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumSetting : BaseEnum
{
    public enum ESelectSettingMenu
    {
        None,
        SettingCategory,
        PolicyType,
    }

    public enum ESettingCategory
    {
        None,

        GameSetting,
        Account,
        CustomerSupport,
    }

    public enum EPolicyType
    {
        None,

        TermOfUse,
        PrivacyPolicy,
        CancellationPolicy
    }

    private ESelectSettingMenu selectSettingMenu;
    private ESettingCategory settingCategory;
    private EPolicyType policyType;

    public ESelectSettingMenu SelectSettingMenu => selectSettingMenu;
    public ESettingCategory SettingCategory => settingCategory;
    public EPolicyType PolicyType => policyType;

    public void SetSelectSettingMenu(ESelectSettingMenu type)
    {
        selectSettingMenu = type;
    }

    public void SetSettingCategory(ESettingCategory type)
    {
        settingCategory = type;
    }
    public void SetPolicyType(EPolicyType type)
    {
        policyType = type;
    }
}

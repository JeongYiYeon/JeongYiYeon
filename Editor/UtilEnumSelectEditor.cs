using Codice.Client.Common.GameUI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UtilEnumSelect), true)]
public class UtilEnumSelectEditor : Editor
{
    private UtilEnumSelect enumSelect;

    private void OnEnable()
    {
        enumSelect = target as UtilEnumSelect;
    }

    public override void OnInspectorGUI()
    {
        enumSelect.SelectCategory = (BaseEnum.ESelectCategory)EditorGUILayout.EnumPopup(new GUIContent("카테고리"), enumSelect.SelectCategory);

        if (enumSelect.SelectCategory == BaseEnum.ESelectCategory.Menu)
        {
            enumSelect.MenuCategory = (BaseEnum.EMenuCategory)EditorGUILayout.EnumPopup(new GUIContent("\t메뉴 카테고리"), enumSelect.MenuCategory);

            if (enumSelect.MenuCategory == BaseEnum.EMenuCategory.Shop)
            {
                ResetEnum();

                enumSelect.SelectShopMenu = (EnumShop.ESelectShopMenu)EditorGUILayout.EnumPopup(new GUIContent("\t샵 내부 메뉴"), enumSelect.SelectShopMenu);

                if (enumSelect.SelectShopMenu == EnumShop.ESelectShopMenu.ShopSubCategory)
                {
                    enumSelect.ShopSubCategory = (EnumShop.EShopSubCategory)EditorGUILayout.EnumPopup(new GUIContent("\t샵 내부 카테고리"), enumSelect.ShopSubCategory);
                }
                else if (enumSelect.SelectShopMenu == EnumShop.ESelectShopMenu.EquipmentGachaCount)
                {
                    enumSelect.EquipmentGachaCount = (EnumShop.EEquipmentGachaCount)EditorGUILayout.EnumPopup(new GUIContent("\t장비 가챠 카운트"), enumSelect.EquipmentGachaCount);
                }
            }
            else if (enumSelect.MenuCategory == BaseEnum.EMenuCategory.Setting)
            {
                ResetEnum();
                enumSelect.SelectSettingMenu = (EnumSetting.ESelectSettingMenu)EditorGUILayout.EnumPopup(new GUIContent("\t셋팅 카테고리"), enumSelect.SelectSettingMenu);

                if (enumSelect.SelectSettingMenu == EnumSetting.ESelectSettingMenu.SettingCategory)
                {
                    enumSelect.SettingCategory = (EnumSetting.ESettingCategory)EditorGUILayout.EnumPopup(new GUIContent("\t셋팅 카테고리 타입"), enumSelect.SettingCategory);
                }
                else if (enumSelect.SelectSettingMenu == EnumSetting.ESelectSettingMenu.PolicyType)
                {
                    enumSelect.PolicyType = (EnumSetting.EPolicyType)EditorGUILayout.EnumPopup(new GUIContent("\t약관 타입"), enumSelect.PolicyType);
                }
            }
            else if (enumSelect.MenuCategory == BaseEnum.EMenuCategory.Attendance)
            {
                ResetEnum();
                enumSelect.AttendanceType = (EnumAttendance.EAttendance)EditorGUILayout.EnumPopup(new GUIContent("\t출첵 종류"), enumSelect.AttendanceType);
            }
            else if (enumSelect.MenuCategory == BaseEnum.EMenuCategory.Quest)
            {
                ResetEnum();
                enumSelect.QuestType = (EnumQuest.EQuest)EditorGUILayout.EnumPopup(new GUIContent("\t퀘스트 종류"), enumSelect.QuestType);
            }
        }
        else if (enumSelect.SelectCategory == BaseEnum.ESelectCategory.Hero)
        {
            ResetEnum();
            enumSelect.CharacterType = (BaseCharacter.CHARACTER_TYPE)EditorGUILayout.EnumPopup(new GUIContent("\t캐릭터 타입"), enumSelect.CharacterType);
        }
        else if (enumSelect.SelectCategory == BaseEnum.ESelectCategory.Stat)
        {
            ResetEnum();
            enumSelect.StatType = (BaseCharacter.EStat)EditorGUILayout.EnumPopup(new GUIContent("\t스텟 타입"), enumSelect.StatType);
        }
        else if (enumSelect.SelectCategory == BaseEnum.ESelectCategory.Challenge)
        {
            ResetEnum();
            enumSelect.ChallengeType = (ChallengeData.EChallengeType)EditorGUILayout.EnumPopup(new GUIContent("\t챌린지 타입"), enumSelect.ChallengeType);
        }
        else if (enumSelect.SelectCategory == BaseEnum.ESelectCategory.HeroMenu)
        {
            ResetEnum();
            enumSelect.HeroMenuCategory = (Hero.ECategory)EditorGUILayout.EnumPopup(new GUIContent("\t메뉴 타입"), enumSelect.HeroMenuCategory);
        }
        else if (enumSelect.SelectCategory == BaseEnum.ESelectCategory.Item)
        {
            ResetEnum();
            enumSelect.ItemCategory = (BaseItem.EItemCategory)EditorGUILayout.EnumPopup(new GUIContent("\t아이템 타입"), enumSelect.ItemCategory);
        }
        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }

    private void ResetEnum()
    {
        ResetEnumShop();
        ResetEnumSetting();
        ResetEnumAttendance();
        ResetEnumQuest();
        ResetEnumCharacterType();
        ResetEnumStatType();
        ResetEnumChallengeType();
        ResetEnumHeroMenuCategory();
        ResetEnumItemCategory();
    }

    private void ResetEnumShop()
    {
        if(enumSelect.MenuCategory == BaseEnum.EMenuCategory.Shop)
        {
            return;
        }

        enumSelect.SelectShopMenu = EnumShop.ESelectShopMenu.None;
        enumSelect.ShopSubCategory = EnumShop.EShopSubCategory.None;
        enumSelect.EquipmentGachaCount = EnumShop.EEquipmentGachaCount.None;
    }

    private void ResetEnumSetting()
    {
        if (enumSelect.MenuCategory == BaseEnum.EMenuCategory.Setting)
        {
            return;
        }

        enumSelect.SelectSettingMenu = EnumSetting.ESelectSettingMenu.None;
        enumSelect.SettingCategory = EnumSetting.ESettingCategory.None;
        enumSelect.PolicyType = EnumSetting.EPolicyType.None;
    }
    
    private void ResetEnumAttendance()
    {
        if (enumSelect.MenuCategory == BaseEnum.EMenuCategory.Attendance)
        {
            return;
        }

        enumSelect.AttendanceType = EnumAttendance.EAttendance.None;
    }

    private void ResetEnumQuest()
    {
        if (enumSelect.MenuCategory == BaseEnum.EMenuCategory.Quest)
        {
            return;
        }
        enumSelect.QuestType = EnumQuest.EQuest.None;
    }

    private void ResetEnumCharacterType()
    {
        if (enumSelect.SelectCategory == BaseEnum.ESelectCategory.Hero)
        {
            return;
        }
        enumSelect.CharacterType = BaseCharacter.CHARACTER_TYPE.NONE;
    }

    private void ResetEnumStatType()
    {
        if (enumSelect.SelectCategory == BaseEnum.ESelectCategory.Stat)
        {
            return;
        }
        enumSelect.StatType = BaseCharacter.EStat.ATK;
    }
    private void ResetEnumChallengeType()
    {
        if (enumSelect.SelectCategory == BaseEnum.ESelectCategory.Challenge)
        {
            return;
        }
        enumSelect.ChallengeType = ChallengeData.EChallengeType.NONE;
    }

    private void ResetEnumHeroMenuCategory()
    {
        if (enumSelect.SelectCategory == BaseEnum.ESelectCategory.HeroMenu)
        {
            return;
        }
        enumSelect.HeroMenuCategory = Hero.ECategory.Info;
    }

    private void ResetEnumItemCategory()
    {
        if (enumSelect.SelectCategory == BaseEnum.ESelectCategory.Item)
        {
            return;
        }
        enumSelect.ItemCategory = BaseItem.EItemCategory.None;
    }
}


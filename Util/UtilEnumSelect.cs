using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UtilEnumSelect : MonoBehaviour
{
    public BaseEnum.ESelectCategory SelectCategory = BaseEnum.ESelectCategory.None;
    public BaseEnum.EMenuCategory MenuCategory = BaseEnum.EMenuCategory.None;

    public EnumShop.ESelectShopMenu SelectShopMenu = EnumShop.ESelectShopMenu.None;
    public EnumShop.EShopSubCategory ShopSubCategory = EnumShop.EShopSubCategory.None;
    public EnumShop.EEquipmentGachaCount EquipmentGachaCount = EnumShop.EEquipmentGachaCount.None;

    public EnumSetting.ESelectSettingMenu SelectSettingMenu = EnumSetting.ESelectSettingMenu.None;
    public EnumSetting.ESettingCategory SettingCategory = EnumSetting.ESettingCategory.None;
    public EnumSetting.EPolicyType PolicyType = EnumSetting.EPolicyType.None;

    public EnumAttendance.EAttendance AttendanceType = EnumAttendance.EAttendance.None;

    public EnumQuest.EQuest QuestType = EnumQuest.EQuest.None;

    public BaseCharacter.CHARACTER_TYPE CharacterType = BaseCharacter.CHARACTER_TYPE.NONE;
    public BaseCharacter.EStat StatType = BaseCharacter.EStat.ATK;

    public ChallengeData.EChallengeType ChallengeType = ChallengeData.EChallengeType.NONE;

    public Hero.ECategory HeroMenuCategory = Hero.ECategory.Info;

    public BaseItem.EItemCategory ItemCategory = BaseItem.EItemCategory.None;
}

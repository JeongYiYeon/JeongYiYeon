using Coffee.UIExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class Hero : MonoSingletonInScene<Hero>
{
    public enum ECategory
    {
        Info,
        Equipment,
        HeroList,
        Growth            
    }

    private enum EEquipState
    {
        Empty,
        Equip
    }

    #region 장비
    [SerializeField]
    private UtilState[] equipItemSlots = null;
    [SerializeField]
    private EquipmentItem[] equipItems = null;
    [SerializeField]
    private EquipmentItemScrollController inventoryScrollViewController = null;
    #endregion

    #region 영웅
    [SerializeField]
    private TMP_Text labelHeroName = null;
    [SerializeField]
    private BaseCharacter heroCharacter = null;
    #endregion

    #region 영웅 스텟
    [SerializeField]
    private TMP_Text labelAtk = null;
    [SerializeField]
    private TMP_Text labelHp = null;
    [SerializeField]
    private TMP_Text labelCri = null;
    [SerializeField]
    private TMP_Text labelCriDam = null;
    #endregion

    #region 스킬 정보
    [SerializeField]
    private SkillThumbnail[] skillThumbnails = null;
    #endregion

    #region 영웅목록
    [SerializeField]
    private HeroThumbnailScrollController heroListScrollViewController = null;
    #endregion

    [SerializeField]
    private EquipmentFilter equipmentFilter = null;

    [SerializeField]
    private UtilState state = null;

    [SerializeField]
    private ToggleGroup toggleGroup = null;

    private ECategory curCategory = ECategory.Info;

    private CharacterData originHeroCharacter = null;
    private BaseCharacter.CHARACTER_TYPE curCharacterType = BaseCharacter.CHARACTER_TYPE.NONE;

    private List<HeroListItemData> heroList = new List<HeroListItemData>();

    public List<HeroListItemData> HeroList => heroList;


    private void OnEnable()
    {
        originHeroCharacter = UserData.Instance.user.Character.SelectCharacter;
        curCategory = ECategory.Info;
        InitHeroList();
    }

    private void OnDisable()
    {
        UserData.Instance.SetSelectCharacter(originHeroCharacter);
    }

    private void Start()
    {
        RefreshToggle();

        state.ActiveState("Info");
    }

    public void RefreshToggle()
    {
        StartCoroutine(ToggleOn());
    }
    private IEnumerator ToggleOn()
    {
        yield return new WaitForEndOfFrame();

        Transform tf = toggleGroup.transform.Find(curCategory.ToString());

        if (tf != null)
        {
            if (tf.GetComponent<Toggle>() != null)
            {
                tf.GetComponent<Toggle>().SetIsOnWithoutNotify(true);
            }
        }
    }

    public void SetCharacterType(BaseCharacter.CHARACTER_TYPE category)
    {
        if (category == BaseCharacter.CHARACTER_TYPE.NONE)
        {
            if (curCharacterType != BaseCharacter.CHARACTER_TYPE.NONE)
            {
                category = curCharacterType;
            }
            else
            {
                category = BaseCharacter.CHARACTER_TYPE.HERO;
            }
        }

        curCharacterType = category;

        UserData.Instance.SetSelectCharacter(UserData.Instance.user.Character.GetHeroCharacter());

        labelHeroName.text = UserData.Instance.user.Character.GetHeroCharacter().Name;

        heroCharacter.CharacterAnimation.SetAtlas(UserData.Instance.user.Character.SelectCharacter.DataCharacter.CHA_FILE);
        heroCharacter.PlayAnimation(BaseCharacter.AnimationType.Idle);

        SetEquip(category);

        SetStatInfo(UserData.Instance.user.Character.SelectCharacter);

        SetSkill(UserData.Instance.user.Character.SelectCharacter.SkillGroupList);
    }

    private void SetEquipInventory()
    {
        if (UserData.Instance.user.Item.InventoryDic.ContainsKey(BaseItem.EItemCategory.EQUIP) == true)
        {
            inventoryScrollViewController.SetEquipmentItems(equipmentFilter.GetFilterEquipmentList(EquipmentFilter.EEquipmnetFilter.ALL));
        }

        if (equipmentFilter != null)
        {
            for (int i = 0; i < equipmentFilter.FilterItems.Length; i++)
            {
                int idx = i;

                equipmentFilter.FilterItems[idx].SetFilterCb(
                    () =>
                    {
                        inventoryScrollViewController.SetEquipmentItems(
                            equipmentFilter.GetFilterEquipmentList((EquipmentFilter.EEquipmnetFilter)idx));
                    });
            }
        }
    }

    public void SetHeroList()
    {
        heroListScrollViewController.SetThumbnailData(heroList);
    }

    private void SetEquip(BaseCharacter.CHARACTER_TYPE category)
    {
        foreach (KeyValuePair<EquipItemData.EEquipItemType, EquipItemData> equipment in UserData.Instance.user.Character.GetHeroCharacter().EquipmentDic)
        {
            if(equipment.Value == null)
            {
                equipItemSlots[((int)equipment.Key) - 1].ActiveState(EEquipState.Empty.ToString());
            }
            else
            {
                equipItemSlots[((int)equipment.Key) - 1].ActiveState(EEquipState.Equip.ToString());
                equipItems[((int)equipment.Key) - 1].SetData(equipment.Value);
            }
        }
    }

    public void SetEquip(EquipItemData.EEquipItemType type, EquipItemData itemData)
    {
        equipItemSlots[((int)type) - 1].ActiveState(EEquipState.Equip.ToString());
        equipItems[((int)type) - 1].SetData(itemData);
    }

    public void SetUnEquip(EquipItemData.EEquipItemType type)
    {
        equipItemSlots[((int)type) - 1].ActiveState(EEquipState.Empty.ToString());
    }

    private void SetStatInfo(CharacterData character)
    {
        double atk = 0;
        double hp = 0;
        float cri = 0f;
        double criDam = 0f;

        double addAtk = 0;
        double addHp = 0;
        float addCri = 0f;
        double addCriDam = 0f;

        atk = GameManager.Instance.HeroCharacter.Damage;
        hp = GameManager.Instance.HeroCharacter.MaxHP;
        cri = (float)character.Critical;
        criDam = 100f;

        foreach (EquipItemData equipmentItemData in character.EquipmentDic.Values.ToList())
        {
            if(equipmentItemData != null)
            {

                addAtk += equipmentItemData.GetEquipmentItemStat().atk;
                addHp += equipmentItemData.GetEquipmentItemStat().hp;
                addCri = equipmentItemData.GetEquipmentItemStat().cri;                
            }
        }

        atk += addAtk;
        hp += addHp;
        cri += addCri;
        criDam += addCriDam;

        labelAtk.text = $"총 공격력 : {UserData.Instance.user.Goods.GetPriceUnit(atk)} <color=red>(+{UserData.Instance.user.Goods.GetPriceUnit(addAtk)})</color>";
        labelHp.text = $"총 체력 : {UserData.Instance.user.Goods.GetPriceUnit(hp)} <color=red>(+{UserData.Instance.user.Goods.GetPriceUnit(addHp)})</color>";
        labelCri.text = $"크리티컬 확률 : {cri.ToString("F2")}% <color=red>(+{addCri})</color>";
        labelCriDam.text = $"크리티컬 데미지 : {criDam.ToString("F2")}% <color=red>(+{addCriDam})</color>";
    }

    private void SetSkill(List<int> skillGroupList)
    {
        if(skillGroupList == null)
        {
            return;
        }
        
        if(skillGroupList.Count == 0)
        {
            return;
        }

        for(int i = 0; i < skillGroupList.Count; i++) 
        {
            //스킬렙업했을때 스킬 값으로 가져와야됨
            skillThumbnails[i].SetSkillData(SkillManager.Instance.SkillDic[skillGroupList[i]][0]);
            skillThumbnails[i].InitSkillThumbnail();
        }
    }

    public void InitHeroList()
    {
        heroList.Clear();

        if(DataManager.Instance.DataHelper.HeroList != null && DataManager.Instance.DataHelper.HeroList.Count > 0)
        {
            for(int i = 0; i < DataManager.Instance.DataHelper.HeroList.Count; i++)
            {
                DataCharacter dataCharacter = DataManager.Instance.DataHelper.Character.Find(x => x.UID == DataManager.Instance.DataHelper.HeroList[i].CHARACTER_UID);

                if (dataCharacter == null)
                {
                    continue;
                }

                BaseCharacter.CHARACTER_TYPE characterType = BaseCharacter.CHARACTER_TYPE.NONE;

                CharacterData heroData = null;

                if (Enum.TryParse<BaseCharacter.CHARACTER_TYPE>(dataCharacter.CHA_TYPE, out characterType))
                {
                    if(characterType == BaseCharacter.CHARACTER_TYPE.HERO)
                    {
                        heroData = UserData.Instance.user.Character.CharacterDatas.Find(x => x.DataCharacter.UID == dataCharacter.UID 
                        && x.Type == BaseCharacter.CHARACTER_TYPE.HERO);
                    }
                }
                
                if(heroData == null)
                {
                    heroData = new CharacterData();
                    heroData.SetDataCharacter(dataCharacter);
                }

                HeroListItemData heroListItemData = new HeroListItemData();

                UserData.EGoodsType type = UserData.EGoodsType.NONE;

                heroListItemData.SetUID(DataManager.Instance.DataHelper.HeroList[i].UID);
                heroListItemData.SetCharacterData(heroData);
                if (Enum.TryParse<UserData.EGoodsType>(DataManager.Instance.DataHelper.HeroList[i].GET_TYPE, out type))
                {
                    heroListItemData.SetPurchaseType(type);
                    heroListItemData.SetPurchasePrice(DataManager.Instance.DataHelper.HeroList[i].BUY_ITEM_COUNT);
                }
                else
                {
                    //스테이지로 획득
                    if(DataManager.Instance.DataHelper.HeroList[i].GET_TYPE == "STAGE")
                    {
                        heroListItemData.SetStageUID(DataManager.Instance.DataHelper.HeroList[i].TYPE_VALUE);
                    }
                    else if (DataManager.Instance.DataHelper.HeroList[i].GET_TYPE == "PACKAGE")
                    {
                        heroListItemData.SetPackage(true);
                    }
                }


                if (UserData.Instance.user.Character.CharacterDatas.Find(x => 
                x.DataCharacter.UID == dataCharacter.UID && x.Type == BaseCharacter.CHARACTER_TYPE.HERO) != null)
                {
                    heroListItemData.SetHave(true);
                }

                heroList.Add(heroListItemData);
            }
        }
    }

    public void OnClickMenuCategory(UtilEnumSelect select)
    {
        curCategory = select.HeroMenuCategory;

        if (curCategory != ECategory.Growth)
        {
            state.ActiveState(select.HeroMenuCategory.ToString());

            switch (select.HeroMenuCategory)
            {
                case ECategory.Info:
                    break;
                case ECategory.Equipment:
                    SetEquipInventory();
                    break;
                case ECategory.HeroList:
                    SetHeroList();
                    break;
            }
        }
        else
        {
            HeroListItemData heroData = null;

            if (curCharacterType == BaseCharacter.CHARACTER_TYPE.HERO)
            {
                heroData = heroList.Find(x =>
                x.CharacterData.DataCharacter.UID == UserData.Instance.user.Character.GetHeroCharacter().DataCharacter.UID);
            }

            if (heroData == null)
            {
                return;
            }

            HeroUpgradePopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.HeroUpgrade) as HeroUpgradePopup;
            popup.SetUpgradeHeroInfo(heroData.CharacterData);
            popup.Active();
        }
    }
}

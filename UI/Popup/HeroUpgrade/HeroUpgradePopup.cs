using Coffee.UIExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class HeroUpgradePopup : BasePopup
{
    [SerializeField]
    private TMP_Text labelDesc = null;

    [SerializeField]
    private TMP_Text labelSelectHeroName = null;
    [SerializeField]
    private AtlasImage imgSelectHero = null;
    [SerializeField]
    private TMP_Text labelAtk = null;
    [SerializeField]
    private TMP_Text labelHp = null;
    [SerializeField]
    private TMP_Text labelCri = null;

    [SerializeField]
    private SkillThumbnail[] skillThumbnails = null;

    [SerializeField]
    private GameObject upgradeGoodsInfoGo = null;
    [SerializeField]
    private BaseItem[] upgradeItems = null;
    [SerializeField]
    private TMP_Text[] labelsUpgradeItemCnt = null;

    private CharacterData heroData = null;

    //강화일때 데이터
    private DataCharacterEnchant enchantData = null;

    private bool isAvailableUpgrade = false;

    private void OnEnable()
    {
        SetType(EPopupType.HeroUpgrade);
    }

    public override void Active()
    {
        base.Active();    
    }

    public void SetUpgradeHeroInfo(CharacterData data)
    {
        isAvailableUpgrade = false;

        heroData = data;

        enchantData = DataManager.Instance.DataHelper.CharacterEnchant.Find(x =>
        x.CHARACTER_UID == data.DataCharacter.UID && x.BEFORE_CHARACTER_LEVEL == data.UpgradeCnt);

        if (data.UpgradeCnt > 0)
        {
            labelSelectHeroName.text = $"{data.Name} +{data.UpgradeCnt}";
        }
        else
        {
            labelSelectHeroName.text = $"{data.Name}";
        }

        SetSkill(data.SkillGroupList);

        AtlasManager.Instance.SetSprite(imgSelectHero, AtlasManager.Instance.Atlas[data.CharacterAtlasName], "Idle_00");

        if (enchantData == null)
        {
            isAvailableUpgrade = false;
            upgradeGoodsInfoGo.SetActive(false);
            labelDesc.text = "";

            labelAtk.text = $"총 공격력 : {data.Damage.ToString("#,##0")}";
            labelHp.text = $"총 체력 : {data.HP.ToString("#,##0")}";
            labelCri.text = $"크리티컬 확률 : {data.Critical.ToString("F2")}%";
        }
        else
        {
            upgradeGoodsInfoGo.SetActive(true);

            labelDesc.text = "성장 확률 100%, 성장 시 재료 소모";

            double addAtk = enchantData.ATTACK_DAM_UP;
            double addHp = enchantData.HP_UP;
            float addCri = (float)enchantData.CRIRATE_UP;

            labelAtk.text = $"총 공격력 : {data.Damage.ToString("#,##0")} <color=red>(+{addAtk})</color>";
            labelHp.text = $"총 체력 : {data.HP.ToString("#,##0")} <color=red>(+{addHp})</color>";
            labelCri.text = $"크리티컬 확률 : {data.Critical.ToString("F2")}%<color=red>(+{addCri})</color>";

            bool isEnoughFirstMaterial = SetUpgradeItemData(0, enchantData.ENCHANT_ITEM_1, enchantData.ENCHANT_ITEM_COUNT_1);
            bool isEnoughSecondMaterial = SetUpgradeItemData(1, enchantData.ENCHANT_ITEM_2, enchantData.ENCHANT_ITEM_COUNT_2);
            bool isEnoughGoods = SetUpgradeItemData(2, enchantData.ENCHANT_ITEM_3, enchantData.ENCHANT_ITEM_COUNT_3);

            isAvailableUpgrade = isEnoughFirstMaterial && isEnoughSecondMaterial && isEnoughGoods;
        }
    }
    private void SetSkill(List<int> skillGroupList)
    {
        if (skillGroupList == null)
        {
            return;
        }

        if (skillGroupList.Count == 0)
        {
            return;
        }

        for (int i = 0; i < skillGroupList.Count; i++)
        {
            //스킬렙업했을때 스킬 값으로 가져와야됨
            skillThumbnails[i].SetSkillData(SkillManager.Instance.SkillDic[skillGroupList[i]][0]);
            skillThumbnails[i].InitSkillThumbnail();
        }
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

        switch(itemUID)
        {
            //다이아
            case 2:
            case 3:
                goodsString = UserData.Instance.user.Goods.GetGoodsString(UserData.EGoodsType.CASH);

                if (UserData.Instance.user.Goods.TotalCash >= itemCnt)
                {
                    isEnoughGoods = true;
                }

                break;

            //골드
            case 4:
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
        if(heroData == null)
        {
            return;
        }

        if(isAvailableUpgrade == false)
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
        popup.SetConfirmCallBack(() =>
        {
            NetworkManager.Instance.SendProtocol(NetworkManager.ESOCKET_PROTOCOL.HERO_ENCHANT, heroData.DataCharacter.UID);
            popup.DeActive();
        });
        popup.Active();
    }
}

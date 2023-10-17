using Coffee.UIEffects;
using Coffee.UIExtensions;
using EnhancedUI.EnhancedScroller;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Cysharp.Threading.Tasks;

public class HeroThumbnail : EnhancedScrollerCellView
{
    [SerializeField]
    private UtilState state = null;

    [SerializeField]
    private UIEffect uiEffect = null;

    [SerializeField]
    private TMP_Text labelLevel = null;

    [SerializeField]
    private TMP_Text labelName = null;

    [SerializeField]
    private AtlasImage imgCharacter = null;

    [SerializeField]
    private HeroPositionDataItem dataItem = null;

    [SerializeField]
    private SkillThumbnail[] skillThumbnails = null;

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

    private HeroListItemData data = null;

    public HeroPositionDataItem DataItem => dataItem;

    public void SetData(HeroListItemData data, int dataIndex)
    {
        this.data = data;
        this.dataIndex = dataIndex;


        labelLevel.gameObject.SetActive(data.CharacterData.UpgradeCnt > 0);
        if (data.CharacterData.UpgradeCnt > 0)
        {
            labelLevel.text = $"+{data.CharacterData.UpgradeCnt}";
        }

        labelName.text = data.CharacterData.Name;
        AtlasManager.Instance.SetSprite(imgCharacter,
            AtlasManager.Instance.Atlas[data.CharacterData.CharacterAtlasName], "Idle_00");

        if (dataItem != null)
        {
            dataItem.SetCharacterData(data.CharacterData);

            if (data.CharacterData.DataCharacter.UID == UserData.Instance.user.Character.GetHeroCharacter().DataCharacter.UID)
            {
                dataItem.SetCharacterType(BaseCharacter.CHARACTER_TYPE.HERO);
            }
            else
            {
                dataItem.SetCharacterType(BaseCharacter.CHARACTER_TYPE.NPC);
            }

            dataItem.SetHave(data.IsHave);
        }

        SetStatInfo(data.CharacterData);

        SetSkill(data.CharacterData.SkillGroupList);

        state.ActiveState(data.IsHave == true ? "Have" : "NotHave");

        RefreshEffect();
    }

    private void RefreshEffect()
    {
        uiEffect.effectFactor = data.IsHave == true ? 0f : 1f;
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

        atk = character.Damage;
        hp = character.MaxHP;
        cri = (float)character.Critical;
        criDam = 100f;

        foreach (EquipItemData equipmentItemData in character.EquipmentDic.Values.ToList())
        {
            if (equipmentItemData != null)
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
            skillThumbnails[i].SetToolTip(true);
            skillThumbnails[i].InitSkillThumbnail();
        }
    }

    public void OnClickHeroChange()
    {
        if (data == null)
        {
            return;
        }

        if (data.CharacterData.DataCharacter.UID == UserData.Instance.user.Character.SelectCharacter.DataCharacter.UID)
        {
            AlramPopup alramPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
            alramPopup.SetButtonType(BasePopup.EButtonType.One);
            alramPopup.SetTitle("오류");
            alramPopup.SetDesc("같은 캐릭은 안됨");
            alramPopup.SetConfirmBtLabel("확인");
            alramPopup.SetConfirmCallBack(() => { alramPopup.DeActive(); });
            alramPopup.Active();
            return;
        }

        NetworkManager.Instance.SendProtocol(NetworkManager.ESOCKET_PROTOCOL.HERO_CHANGE, data.CharacterData.DataCharacter.UID);
    }


    public void OnClickHeroUpgrade()
    {
        HeroListItemData heroData = null;

        if (data == null)
        {
            return;
        }

        heroData = Hero.Instance.HeroList.Find(x =>
        x.CharacterData.DataCharacter.UID == data.CharacterData.DataCharacter.UID);

        HeroUpgradePopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.HeroUpgrade) as HeroUpgradePopup;
        popup.SetUpgradeHeroInfo(heroData.CharacterData);
        popup.Active();
    }

    public void OnClickPurchaseHero()
    {
        if (data == null)
        {
            return;
        }

        if (data.StageUID > 0)
        {
            if(UserData.Instance.user.Stage.StageUID >= data.StageUID)
            {
                //스테이지 캐릭터 해금
            }
            else
            {
                DataStage stageData = DataManager.Instance.DataHelper.Stage.Find(x => x.UID == data.StageUID);

                LoadingManager.Instance.ActiveOneLineAlram($"{DataManager.Instance.GetLocalization(stageData.STAGE_NAME)} 클리어 시 해금");
            }
        }
        else
        {
            if (data.PurchaseType != UserData.EGoodsType.NONE)
            {
                if (UserData.Instance.IsEnoughGoods(data.PurchaseType, data.PurchasePrice) == true)
                {
                    AlramPopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
                    popup.SetButtonType(BasePopup.EButtonType.Two);
                    popup.SetTitle("영웅구매");
                    popup.SetDesc($"{data.PurchaseType} : {data.PurchasePrice}로 살꺼에요??");
                    popup.SetConfirmBtLabel("구매");
                    popup.SetCancelBtLabel("안삼");
                    popup.SetConfirmCallBack(() =>
                    {
                        NetworkManager.Instance.SendProtocol(NetworkManager.ESOCKET_PROTOCOL.SHOP_HERO, data.UID);
                        popup.DeActive();

                        // await NetworkPacketShop.Instance.TaskBuyHero(data.UID,
                        //successCb: () =>
                        //{
                        //    LobbyManager.Instance.RefreshGoodsLabel();

                        //    Hero.Instance.InitHeroList();
                        //    Hero.Instance.SetHeroList();
                        //    popup.DeActive();
                        //});
                    });
                    popup.Active();
                }
                else
                {
                    if (data.PurchaseType == UserData.EGoodsType.CASH)
                    {
                        UIManager.Instance.GetChargePopup(data.PurchaseType);
                    }
                    else if (data.PurchaseType == UserData.EGoodsType.GOLD)
                    {
                        string tmpGold = UserData.Instance.user.Goods.GetPriceUnit(data.PurchasePrice - UserData.Instance.user.Goods.Gold);
                        LoadingManager.Instance.ActiveOneLineAlram($"{tmpGold} 골드가 부족 합니다.");
                    }
                }
            }
            else
            {
                if (data.IsPackage == true)
                {
                    LoadingManager.Instance.ActiveOneLineAlram("패키지로만 구매 가능");
                }
            }    
        }
    }

}

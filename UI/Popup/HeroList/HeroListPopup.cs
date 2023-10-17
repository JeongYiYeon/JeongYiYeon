using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Coffee.UIExtensions;
using UnityEngine.U2D;
using System;
using Cysharp.Threading.Tasks;

public class HeroListPopup : BasePopup
{
    private enum EHaveType
    {
        Have,
        NotHave,
    }

    [SerializeField]
    private HeroListScrollController heroListScrollController = null;

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
    private UtilState heroHaveState = null;

    [SerializeField]
    private AtlasImage imgPurchaseGoods = null;
    [SerializeField]
    private TMP_Text labelPurchasePrice = null;


    private List<HeroListItemData> heroListItemDatas = null;

    //선택창에 누른 영웅 캐릭터
    private HeroListItemData selectHeroData = null;

    public HeroListItemData SelectHeroData => selectHeroData;

    private void OnEnable()
    {
        SetType(EPopupType.HeroList);
    }

    public void SetHeroList(List<HeroListItemData> heroListItemDatas)
    {
        if(heroListItemDatas == null || heroListItemDatas.Count == 0)
        {
            Debug.LogError("오류");
            return;
        }

        this.heroListItemDatas = heroListItemDatas;

        selectHeroData = heroListItemDatas.Find(x =>
        x.CharacterData.DataCharacter.UID == UserData.Instance.user.Character.SelectCharacter.DataCharacter.UID);

        heroListScrollController.SetHeroListItems(heroListItemDatas);
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

    public override void Reset()
    {
        base.Reset();

        if (heroListItemDatas != null && heroListItemDatas.Count > 0)
        {
            selectHeroData = heroListItemDatas.Find(x =>
            x.CharacterData.DataCharacter.UID == UserData.Instance.user.Character.SelectCharacter.DataCharacter.UID);

            SetHeroInfo(selectHeroData);
        }
    }

    public override void Active()
    {
        base.Active();
        Reset();
    }

    public void SetHeroInfo(HeroListItemData data)
    {
        selectHeroData = data;

        labelSelectHeroName.text = data.CharacterData.Name;

        AtlasManager.Instance.SetSprite(imgSelectHero,
            AtlasManager.Instance.Atlas[data.CharacterData.CharacterAtlasName],
            "Idle_00");

        labelAtk.text = $"총 공격력 : {data.CharacterData.Damage.ToString("#,##0")}";
        labelHp.text = $"총 체력 : {data.CharacterData.HP.ToString("#,##0")}";
        labelCri.text = $"크리티컬 확률 : {data.CharacterData.Critical.ToString("F2")}%";

        SetSkill(data.CharacterData.SkillGroupList);

        if (data.IsHave == true)
        {
            heroHaveState.ActiveState(EHaveType.Have.ToString());
        }
        else
        {
            heroHaveState.ActiveState(EHaveType.NotHave.ToString());
            imgPurchaseGoods.gameObject.SetActive(data.StageUID == 0);

            if (data.StageUID > 0)
            {
                labelPurchasePrice.text = $"{data.StageUID} 스테이지 클리어시 해방";
            }

            else
            {
                if (data.PurchaseType == UserData.EGoodsType.CASH)
                {
                    AtlasManager.Instance.SetSprite(imgPurchaseGoods, imgPurchaseGoods.spriteAtlas, "Common_Icon_Gem");

                }
                else if (data.PurchaseType == UserData.EGoodsType.GOLD)
                {
                    AtlasManager.Instance.SetSprite(imgPurchaseGoods, imgPurchaseGoods.spriteAtlas, "Common_Icon_Gold");
                }
                else
                {
                    Debug.LogError("가격 타입 오류");
                }

                labelPurchasePrice.text = data.PurchasePrice.ToString();
            }
        }
    }

    public void OnClickHeroChange()
    {
        if (selectHeroData == null)
        {
            return;
        }

        if(selectHeroData.CharacterData.DataCharacter.UID == UserData.Instance.user.Character.SelectCharacter.DataCharacter.UID)
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

        NetworkPacketCharacter.Instance.TaskChangeHero(selectHeroData.CharacterData.DataCharacter.UID,
                   () =>
                   {
                       Hero.Instance.InitHeroList();
                       LobbyManager.Instance.RefreshHeroMenu(BaseCharacter.CHARACTER_TYPE.HERO);
                       heroListScrollController.SetHeroListItems(Hero.Instance.HeroList);
                       LoadingManager.Instance.Reset(LoadingManager.EState.Loading);
                   }).Forget();
    }

    public void OnClickHeroUpgrade()
    {
        if (selectHeroData == null)
        {
            return;
        }

        HeroUpgradePopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.HeroUpgrade) as HeroUpgradePopup;
        popup.SetUpgradeHeroInfo(selectHeroData.CharacterData);
        popup.Active();
    }

    public void OnClickPurchaseHero()
    {
        if(selectHeroData == null)
        {
            return;
        }

        if (selectHeroData.PurchaseType != UserData.EGoodsType.NONE)
        {
            if (UserData.Instance.IsEnoughGoods(selectHeroData.PurchaseType, selectHeroData.PurchasePrice) == true)
            {
                AlramPopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
                popup.SetButtonType(BasePopup.EButtonType.Two);
                popup.SetTitle("영웅구매");
                popup.SetDesc($"{selectHeroData.PurchaseType} : {selectHeroData.PurchasePrice}로 살꺼에요??");
                popup.SetConfirmBtLabel("구매");
                popup.SetCancelBtLabel("안삼");
                popup.SetConfirmCallBack(() =>
                {  
                   // await NetworkPacketShop.Instance.TaskBuyHero(selectHeroData.UID,
                   //successCb: () =>
                   //{
                   //    LobbyManager.Instance.RefreshGoodsLabel();

                   //    Hero.Instance.InitHeroList();

                   //    heroListScrollController.SetHeroListItems(Hero.Instance.HeroList);
                       popup.DeActive();
                   //});                   
                });
                popup.Active();
            }
            else
            {
                UIManager.Instance.GetChargePopup(selectHeroData.PurchaseType);
            }
        }
    }
}

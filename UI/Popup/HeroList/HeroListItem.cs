using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Coffee.UIExtensions;
using UnityEngine.U2D;

public class HeroListItem : MonoBehaviour
{
    [SerializeField]
    private TMP_Text labelHeroName = null;

    [SerializeField]
    private AtlasImage imgHeroCharacter = null;

    [SerializeField]
    private GameObject imgNotHaveGo = null;

    [SerializeField]
    private GameObject imgSelectGo = null;

    private HeroListItemData heroListItemData = null;

    private HeroListPopup heroListPopup = null;

    private bool isSelect = false;

    private void OnEnable()
    {
        if (heroListPopup == null)
        {
            heroListPopup = UIManager.Instance.GetPopup(BasePopup.EPopupType.HeroList) as HeroListPopup;
        }

        Messenger<bool>.AddListener(ConstHelper.MessengerString.MSG_HEROLISTITEM_RESET, ActiveSelectGo);
    }

    private void OnDisable()
    {
        Messenger<bool>.RemoveListener(ConstHelper.MessengerString.MSG_HEROLISTITEM_RESET, ActiveSelectGo);
    }

    public virtual void SetData(HeroListItemData heroListItemData)
    {
        if (heroListItemData == null)
        {
            this.gameObject.SetActive(false);
            return;
        }

        this.heroListItemData = heroListItemData;

        if (heroListPopup != null && heroListPopup.SelectHeroData != null)
        {
            isSelect = (heroListPopup.SelectHeroData.CharacterData.DataCharacter.UID == heroListItemData.CharacterData.DataCharacter.UID);
        }

        labelHeroName.text = heroListItemData.CharacterData.Name;

        AtlasManager.Instance.SetSprite(imgHeroCharacter,
            AtlasManager.Instance.Atlas[heroListItemData.CharacterData.CharacterAtlasName],
            "Idle_00");

        imgNotHaveGo.SetActive(!heroListItemData.IsHave);
        ActiveSelectGo(isSelect);

        this.gameObject.SetActive(true);
    }    

    private void ActiveSelectGo(bool isSelect)
    {
        this.isSelect = isSelect;
        imgSelectGo.SetActive(isSelect);
    }

    public void OnClickHero()
    {
        Messenger<bool>.Broadcast(ConstHelper.MessengerString.MSG_HEROLISTITEM_RESET, false);

        ActiveSelectGo(true);

        heroListPopup.SetHeroInfo(heroListItemData);
    }
}

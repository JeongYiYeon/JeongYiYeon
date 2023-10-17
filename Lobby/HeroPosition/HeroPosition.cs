using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using UnityEngine.UI;

public class HeroPosition : MonoSingletonInScene<HeroPosition>
{
    private enum ECompleteType
    {
        Complete,
        NotComplete,
    }

    [SerializeField]
    private HeroThumbnailScrollController scrollViewController = null;

    [SerializeField]
    private GridLayoutGroup gridLayoutGroup = null;

    [SerializeField]
    private HeroPositionGridItem[] posGridHeroItems = null;

    [SerializeField]
    private AtlasImage heroPostionMoveImg = null;

    public CharacterData CurClickCharacterData = null;

    private List<HeroListItemData> heroList = new List<HeroListItemData>();

    private Dictionary<int, CharacterData> heroPosInfoDic = new Dictionary<int, CharacterData>() 
    {
        {1, null}, {2, null}, {3, null},
        {4, null}, {5, null}, {6, null},
        {7, null}, {8, null}, {9, null},
    };


    private void OnEnable()
    {
        CurClickCharacterData = null;

        Reset();        
    }

    public void Reset()
    {
        if (heroPosInfoDic != null && heroPosInfoDic.Count > 0)
        {
            heroPosInfoDic.Keys.ToList().ForEach
                (x =>
                    {
                        if (heroPosInfoDic[x] != null && x != UserData.Instance.user.Config.HeroPos)
                        {
                            heroPosInfoDic[x].SetPositionIdx(0);
                            heroPosInfoDic[x] = null;
                        }
                    });
        }

        for (int i = 0; i < posGridHeroItems.Length; i++)
        {
            posGridHeroItems[i].SetCharacterData(null);
            posGridHeroItems[i].ResetCharacterType();
            posGridHeroItems[i].posHeroImgGo.SetActive(false);            
        }
    }

    public void InitHeroPosition()
    {
        SetHeroThumbnail();

        if (UserData.Instance.user.Character.CharacterDatas != null && UserData.Instance.user.Character.CharacterDatas.Count > 0)
        {
            //클라에서 그리드 0부터라서 하나씩 낮음
            SetPositionHero(UserData.Instance.user.Character.GetHeroCharacter(), UserData.Instance.user.Config.HeroPos);

            if (UserData.Instance.user.Character.GetFollowCharacters() != null &&
                UserData.Instance.user.Character.GetFollowCharacters().Count > 0)
            {
                foreach (CharacterData item in UserData.Instance.user.Character.GetFollowCharacters())
                {
                    SetPositionHero(item, item.PositionIdx);
                }
            }
        }
    }

    private void InitHeroList()
    {
        heroList.Clear();

        if (DataManager.Instance.DataHelper.HeroList != null && DataManager.Instance.DataHelper.HeroList.Count > 0)
        {
            for (int i = 0; i < DataManager.Instance.DataHelper.HeroList.Count; i++)
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
                    if (characterType == BaseCharacter.CHARACTER_TYPE.HERO)
                    {
                        heroData = UserData.Instance.user.Character.CharacterDatas.Find(x => x.DataCharacter.UID == dataCharacter.UID
                        && x.Type == BaseCharacter.CHARACTER_TYPE.HERO);
                    }
                }

                if (heroData == null)
                {
                    heroData = new CharacterData();
                    heroData.SetDataCharacter(dataCharacter);
                }

                HeroListItemData heroListItemData = new HeroListItemData();

                heroListItemData.SetUID(DataManager.Instance.DataHelper.HeroList[i].UID);
                heroListItemData.SetCharacterData(heroData);

                if (UserData.Instance.user.Character.CharacterDatas.Find(x =>
                x.DataCharacter.UID == dataCharacter.UID && x.Type == BaseCharacter.CHARACTER_TYPE.HERO) != null)
                {
                    heroListItemData.SetHave(true);
                }

                heroList.Add(heroListItemData);
            }
        }
    }

    private void SetHeroThumbnail()
    {
        InitHeroList();

        scrollViewController.SetThumbnailData(heroList);
    }

    public void RefreshHeroThumbnail()
    {
        scrollViewController.ScrollViewRefresh();
    }


    public void OnClickConfirm()
    {
        NetworkManager.Instance.SendProtocol(NetworkManager.ESOCKET_PROTOCOL.HERO_POSITION, heroPosInfoDic.Values.ToList());

        LobbyManager.Instance.SetMenu(BaseEnum.EMenuCategory.HeroPos);
    }

    public void OnClickReset()
    {
        Reset();
        InitHeroPosition();
        LoadingManager.Instance.ActiveOneLineAlram("각 영웅들 배치가 초기화됨");
    }

    private void SetPositionHero(CharacterData characterData, int idx)
    {
        characterData.SetTempPositionIdx(idx);
        heroPosInfoDic[idx] = characterData;

        AtlasManager.Instance.SetSprite(heroPostionMoveImg, 
            AtlasManager.Instance.Atlas[characterData.DataCharacter.CHA_FILE],
            "Idle_00");

        //클라 그리드 0부터라 하나 뺴줘야됨
        posGridHeroItems[idx - 1].SetCharacterData(characterData);
        posGridHeroItems[idx - 1].SetHeroImg(heroPostionMoveImg.spriteAtlas, heroPostionMoveImg.sprite);
        posGridHeroItems[idx - 1].SetCharacterType(idx - 1);        
        posGridHeroItems[idx - 1].posHeroImgGo.SetActive(true);

        Debug.LogError(">>>>>>>>>>>>" + idx);
    }

    private bool IsSetPositionHero(CharacterData data, out int posIdx)
    {
        for (int i = 0; i < posGridHeroItems.Length; i++)
        {
            if(posGridHeroItems[i].DataItem.Data == null || data == null)
            {
                continue;
            }

            if(posGridHeroItems[i].DataItem.Data.DataCharacter.UID == data.DataCharacter.UID)
            {
                posIdx = i + 1;
                return true;
            }
        }

        posIdx = -1;
        return false;
    }

    public Vector3 GetOffset()
    {
        Vector3 offset = transform.TransformPoint(new Vector3(0, (gridLayoutGroup.cellSize.y / 2f) + gridLayoutGroup.spacing.y, 0));
        
        return offset;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (CurClickCharacterData == null)
        {
            return;
        }

        AtlasManager.Instance.SetSprite(heroPostionMoveImg,
            AtlasManager.Instance.Atlas[CurClickCharacterData.CharacterAtlasName], "Idle_00");

        heroPostionMoveImg.transform.position = eventData.pointerCurrentRaycast.worldPosition;
        heroPostionMoveImg.gameObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        heroPostionMoveImg.transform.GetComponent<RectTransform>().anchoredPosition = transform.InverseTransformPoint(eventData.pointerCurrentRaycast.worldPosition);
    }

    public void OnEndDrag(HeroPositionDataItem item, PointerEventData eventData)
    {
        if (heroPostionMoveImg.gameObject.activeSelf == true)
        {
            int idx = -1;

            //이미 딴곳에 있을때
            if (IsSetPositionHero(CurClickCharacterData, out idx) == true)
            {
                heroPosInfoDic[idx] = null;
                posGridHeroItems[idx - 1].SetCharacterData(null);
                posGridHeroItems[idx - 1].ResetCharacterType();
                posGridHeroItems[idx - 1].posHeroImgGo.SetActive(false);
            }

            idx = int.Parse(item.name);

            SetPositionHero(CurClickCharacterData, idx);
        }

        heroPostionMoveImg.gameObject.SetActive(false);

        CurClickCharacterData = null;        
    }

    public void ResetClick()
    {
        heroPostionMoveImg.gameObject.SetActive(false);

        CurClickCharacterData = null;
    }
}

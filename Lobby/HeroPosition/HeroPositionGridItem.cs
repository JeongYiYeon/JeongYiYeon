using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class HeroPositionGridItem : MonoBehaviour
{
    [SerializeField]
    private AtlasImage posHeroImg = null;

    [SerializeField]
    private HeroPositionDataItem dataItem = null;

    public AtlasImage PosHeroImg => posHeroImg;
    public GameObject posHeroImgGo => posHeroImg.gameObject;

    public HeroPositionDataItem DataItem => dataItem;

    public void SetCharacterData(CharacterData characterData)
    {
        dataItem.SetCharacterData(characterData);
    }

    public void SetCharacterType(int posIdx)
    {
        dataItem.SetCharacterType(posIdx == 5 ?
            BaseCharacter.CHARACTER_TYPE.HERO : BaseCharacter.CHARACTER_TYPE.NPC);
    }

    public void ResetCharacterType()
    {
        dataItem.SetCharacterType(BaseCharacter.CHARACTER_TYPE.NONE);
    }

    public void SetHeroImg(SpriteAtlas atlas, Sprite sprite)
    {
        posHeroImg.spriteAtlas = atlas;
        posHeroImg.sprite = sprite;
    }

    public void SetHeroImg(SpriteAtlas atlas, string spriteName)
    {
        AtlasManager.Instance.SetSprite(posHeroImg, atlas, spriteName);
    }
}

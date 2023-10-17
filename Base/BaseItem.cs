using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.U2D;
using Coffee.UIExtensions;

public class BaseItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private enum EState
    {
        Count,              //템 갯수가 1개 이상일때 표시
        NoCount,
    }

    public enum EItemCategory
    {
        None,
        EQUIP = 10,         //장비
        MONEY = 20,         //재화
        ETC = 30,           //기타
        SPEND = 40,         //소모성
    }

    [SerializeField]
    private AtlasImage bgGrade = null;
    [SerializeField]
    private AtlasImage[] imgItem = null;
    [SerializeField]
    private TMP_Text labelCount = null;
    [SerializeField]
    private UtilState state = null;

    private protected string title;
    private protected string desc;
    private protected EItemCategory itemCategory;
    private protected string imgItemPath;
    private protected double itemCount;

    private protected bool isTooltip;

    private BaseItemData data = null;

    private void OnDisable()
    {
        if (isTooltip == true)
        {
            ItemToolTip.Instance.DeActive();
        }
    }

    public void InitItem(BaseItemData data)
    {
        this.data = data;

        title = data.Title;
        desc = data.Desc;
        itemCategory = data.ItemCategory;
        imgItemPath = data.ImgItemPath;
        itemCount = data.ItemCount;
        isTooltip = data.IsToolTip;

        if(data.ItemGrade > 0)
        {
            AtlasManager.Instance.SetSprite(bgGrade, AtlasManager.Instance.Atlas["Item"], $"Item_Frame_Grade_{data.ItemGrade}");
        }
        else
        {
            AtlasManager.Instance.SetSprite(bgGrade, AtlasManager.Instance.Atlas["Item"], "Item_Frame_Grade_1");
        }

        SpriteAtlas atlas = AtlasManager.Instance.Atlas[data.AtlasPath];            

        if (itemCount > 1)
        {
            state.ActiveState(EState.Count.ToString());

            AtlasManager.Instance.SetSprite(imgItem[(int)EState.Count], atlas, imgItemPath);

            labelCount.text = $"X {itemCount}";
        }
        else
        {
            state.ActiveState(EState.NoCount.ToString());

            AtlasManager.Instance.SetSprite(imgItem[(int)EState.NoCount], atlas, imgItemPath);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isTooltip == true)
        {
            ItemToolTip.Instance.InitItemToolTip(itemCategory, title, desc);

            Vector3 toolTipPos = CameraManager.Instance.GetCamera(CameraManager.ECamera.UI).
                WorldToScreenPoint(ItemToolTip.Instance.transform.position);

            ItemToolTip.Instance.SetPosition(
            CameraManager.Instance.GetCamera(CameraManager.ECamera.UI).ScreenToWorldPoint(
                new Vector3(
                    eventData.position.x,
                    eventData.position.y,
                    toolTipPos.z)));


            Vector3 toolTipViewPos = CameraManager.Instance.GetCamera(CameraManager.ECamera.UI).
                WorldToViewportPoint(ItemToolTip.Instance.transform.position);

            ItemToolTip.Instance.SetRotation(toolTipViewPos.x < 0.5f);
            ItemToolTip.Instance.SetAttachToolTipSize();
            ItemToolTip.Instance.Active();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (isTooltip == true)
        {
            ItemToolTip.Instance.DeActive();
        }
    }
}

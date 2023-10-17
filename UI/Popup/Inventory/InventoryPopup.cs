using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPopup : BasePopup
{
    [SerializeField]
    private UtilState state = null;

    [SerializeField]
    private GameObject allSellBtGo = null;

    [SerializeField]
    private EquipmentItemScrollController equipScrollViewController = null;
    [SerializeField]
    private EquipmentFilter equipmentFilter = null;

    [SerializeField]
    private InventoryScrollController itemScrollContoller = null;

    [SerializeField]
    private ItemSort itemSort = null;

    private EquipmentFilter.EEquipmnetFilter currentFilter = EquipmentFilter.EEquipmnetFilter.ALL;

    private void OnEnable()
    {
        SetType(EPopupType.Inventory);
    }

    public override void Init()
    {
        base.Init();
    }

    public override void Active()
    {
        base.Active();
        ActiveEquipmentScroll();
    }

    private void ActiveEquipmentScroll()
    {
        state.ActiveState("Equipment");
        allSellBtGo.SetActive(true);

        if (UserData.Instance.user.Item.InventoryDic.ContainsKey(BaseItem.EItemCategory.EQUIP) == true)
        {
            if (UserData.Instance.user.Item.InventoryDic[BaseItem.EItemCategory.EQUIP].Count == 0)
            {
                itemScrollContoller.SetItemDataInfo(null);
            }

            else
            {
                equipScrollViewController.SetEquipmentItems(equipmentFilter.GetFilterEquipmentList(EquipmentFilter.EEquipmnetFilter.ALL));

                if (equipmentFilter != null)
                {
                    for (int i = 0; i < equipmentFilter.FilterItems.Length; i++)
                    {
                        int idx = i;

                        equipmentFilter.FilterItems[idx].SetFilterCb(
                            () =>
                            {
                                currentFilter = (EquipmentFilter.EEquipmnetFilter)idx;
                                equipScrollViewController.SetEquipmentItems(
                                    equipmentFilter.GetFilterEquipmentList((EquipmentFilter.EEquipmnetFilter)idx));
                            });
                    }
                }

                if (itemSort != null)
                {
                    itemSort.ActiveUpgradeSortBt(true);

                    itemSort.SetGradeSortCb(() =>
                    {
                        equipScrollViewController.SetEquipmentItems(
                            itemSort.GetSortGradeEquipmentList(equipmentFilter.GetFilterEquipmentList(currentFilter)));
                    });

                    itemSort.SetUpGradeSortCb(() =>
                    {
                        equipScrollViewController.SetEquipmentItems(
                            itemSort.GetSortUpGradeEquipmentList(equipmentFilter.GetFilterEquipmentList(currentFilter)));
                    });
                }
            }
        }
        else
        {
            itemScrollContoller.SetItemDataInfo(null);
        }
    }

    private void ActiveItemScroll(UtilEnumSelect select)
    {
        state.ActiveState("NoEquipment");
        allSellBtGo.SetActive(false);
        if (UserData.Instance.user.Item.InventoryDic.ContainsKey(select.ItemCategory) == true)
        {
            itemScrollContoller.SetItemDataInfo(UserData.Instance.user.Item.InventoryDic[select.ItemCategory]);

            if (itemSort != null)
            {
                itemSort.ActiveUpgradeSortBt(false);
                itemSort.SetGradeSortCb(() =>
                {
                    itemScrollContoller.SetItemDataInfo(
                        itemSort.GetSortItemList(UserData.Instance.user.Item.InventoryDic[select.ItemCategory]));
                });
            }
        }
        else
        {
            itemScrollContoller.SetItemDataInfo(new List<BaseItemData>());
        }
    }

    public void OnClickCategory(UtilEnumSelect select)
    {
        switch(select.ItemCategory)
        {
            case BaseItem.EItemCategory.EQUIP:
                ActiveEquipmentScroll();
                break;

           
            case BaseItem.EItemCategory.ETC:
            case BaseItem.EItemCategory.SPEND:
                ActiveItemScroll(select);
                break;
        }
    }

    public void OnClickAllSell()
    {
        AllSellPopup allSellPopup =
                    UIManager.Instance.GetPopup(BasePopup.EPopupType.AllSell) as AllSellPopup;

        allSellPopup.SetConfirmCallBack(() => 
        {
            allSellPopup.CaculateSellItem();
        });

        allSellPopup.Active();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EquipmentFilter : MonoBehaviour
{ 
    public enum EEquipmnetFilter
    {
        ALL,
        WEAPON,
        HELM,
        GLOVE,
        ARMOR,
        ACCESSORY,
        BOOTS
    }

    [SerializeField]
    private EquipmentFilterItem[] filterItems = null;

    private List<EquipItemData> filterEquipmentList = new List<EquipItemData>();

    public EquipmentFilterItem[] FilterItems => filterItems;

    public List<EquipItemData> GetFilterEquipmentList(EEquipmnetFilter filter)
    {
        filterEquipmentList.Clear();

        if (UserData.Instance.user.Item.InventoryDic.ContainsKey(BaseItem.EItemCategory.EQUIP) == true)
        {
            switch (filter)
            {
                case EEquipmnetFilter.ALL:
                    filterEquipmentList = UserData.Instance.user.Item.InventoryDic[BaseItem.EItemCategory.EQUIP].Cast<EquipItemData>().ToList();
                    filterEquipmentList.Sort((x, y) => x.EquipItemType.CompareTo(y.EquipItemType));
                    break;
                case EEquipmnetFilter.WEAPON:
                    filterEquipmentList = UserData.Instance.user.Item.InventoryDic[BaseItem.EItemCategory.EQUIP].Cast<EquipItemData>().ToList()
                        .FindAll(x => x.EquipItemType == EquipItemData.EEquipItemType.WEAPON);
                    break;
                case EEquipmnetFilter.HELM:
                    filterEquipmentList = UserData.Instance.user.Item.InventoryDic[BaseItem.EItemCategory.EQUIP].Cast<EquipItemData>().ToList().
                        FindAll(x => x.EquipItemType == EquipItemData.EEquipItemType.HELM);
                    break;
                case EEquipmnetFilter.GLOVE:
                    filterEquipmentList = UserData.Instance.user.Item.InventoryDic[BaseItem.EItemCategory.EQUIP].Cast<EquipItemData>().ToList().
                        FindAll(x => x.EquipItemType == EquipItemData.EEquipItemType.GLOVE);
                    break;
                case EEquipmnetFilter.ARMOR:
                    filterEquipmentList = UserData.Instance.user.Item.InventoryDic[BaseItem.EItemCategory.EQUIP].Cast<EquipItemData>().ToList().
                        FindAll(x => x.EquipItemType == EquipItemData.EEquipItemType.ARMOR);
                    break;
                case EEquipmnetFilter.ACCESSORY:
                    filterEquipmentList = UserData.Instance.user.Item.InventoryDic[BaseItem.EItemCategory.EQUIP].Cast<EquipItemData>().ToList().
                        FindAll(x => x.EquipItemType == EquipItemData.EEquipItemType.ACCESSORY);
                    break;
                case EEquipmnetFilter.BOOTS:
                    filterEquipmentList = UserData.Instance.user.Item.InventoryDic[BaseItem.EItemCategory.EQUIP].Cast<EquipItemData>().ToList().
                        FindAll(x => x.EquipItemType == EquipItemData.EEquipItemType.BOOTS);
                    break;
            }
        }

        return filterEquipmentList;
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AllSellPopup : BasePopup
{
    [SerializeField]
    private Toggle[] toggles;

    [SerializeField]
    private TMP_Text labelTotalSell = null;

    private List<int> toggleOn = new List<int>();
    private List<EquipItemData> equipmentList = new List<EquipItemData>();
    private List<EquipItemData> selectGradeEquipmentList = new List<EquipItemData>();
    private double totalPrice = 0;

    private void OnEnable()
    {
        SetType(EPopupType.AllSell);
    }

    public override void Init()
    {
        base.Init();
    }

    public override void Active()
    {        
        base.Active();

        Reset();
        ResetToggle();

        labelTotalSell.text = $"총 판매 금액 : {totalPrice}";
        equipmentList = UserData.Instance.user.Item.InventoryDic[BaseItem.EItemCategory.EQUIP].Cast<EquipItemData>().ToList();
    }

    public override void Reset()
    {
        totalPrice = 0;
        toggleOn.Clear();
        equipmentList.Clear();
        selectGradeEquipmentList.Clear();
    }

    private void ResetToggle()
    {
        for(int i = 0; i < toggles.Length; i++)
        {
            if(toggles[i].isOn == true)
            {
                toggles[i].SetIsOnWithoutNotify(false);
            }
        }
    }

    public void CaculateSellItem()
    {
        //if (toggleOn != null && toggleOn.Count > 0)
        //{
        //    List<EquipItemData> tmpEquipItemList = new List<EquipItemData>();

        //    for (int i = 0; i < toggleOn.Count; i++)
        //    {
        //        if (UserData.Instance.user.Item.InventoryDic.ContainsKey(BaseItem.EItemCategory.EQUIP) == true)
        //        {
        //            selectGradeEquipmentList = equipmentList.FindAll(x => x.ItemGrade == toggleOn[i]);

        //            tmpEquipItemList.AddRange(selectGradeEquipmentList);                    
        //        }
        //    }

        //    Debug.LogError($"{CaculateSellItemPrice(tmpEquipItemList)} 판매 완료");
        //}

        Debug.LogError($"{totalPrice} 판매 완료");
    }

    private void CaculateSellItem(int idx, bool isPlus)
    {
        if (UserData.Instance.user.Item.InventoryDic.ContainsKey(BaseItem.EItemCategory.EQUIP) == true)
        {
            selectGradeEquipmentList = equipmentList.FindAll(x => x.ItemGrade == idx);

            if (selectGradeEquipmentList.Count > 0)
            {
                if (isPlus == true)
                {
                    totalPrice += CaculateSellItemPrice(selectGradeEquipmentList);
                }
                else
                {
                    totalPrice -= CaculateSellItemPrice(selectGradeEquipmentList);
                }

                labelTotalSell.text = $"총 판매 금액 : {UserData.Instance.user.Goods.GetPriceUnit(totalPrice)}";
            }
        }
    }

    private double CaculateSellItemPrice(List<EquipItemData> itemDataList)
    {
        if(itemDataList.Count == 0)
        {
            return 0;
        }

        double totalSell = 0;

        for(int i = 0; i < itemDataList.Count; i++)
        {
            if (itemDataList[i].UpgradeCnt > 0)
            {
                //강화 수치 * 10% 가산점
                totalSell += itemDataList[i].SellItemCount + (itemDataList[i].SellItemCount * ((itemDataList[i].UpgradeCnt * 10f) / 100f));
            }
            else
            {
                totalSell += itemDataList[i].SellItemCount;
            }
        }

        return totalSell;
    }

    public void OnClickCaculateSellItem(GameObject go)
    {
        int idx = 0;
        bool isPlus = false;

        if(int.TryParse(go.name, out idx) == true)
        {
            if (toggleOn.Contains(idx) == false)
            {
                toggleOn.Add(idx);
                isPlus = true;
            }
            else
            {
                toggleOn.Remove(idx);
            }
        }

        if (idx > 0)
        {
            CaculateSellItem(idx, isPlus);
        }
    }


}

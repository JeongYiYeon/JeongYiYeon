using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AttendancePopup : BasePopup
{
    private enum EDirection
    {
        Top,
        Bottom
    }

    [SerializeField]
    private UtilState state = null;

    [SerializeField]
    private TMP_Text labelDesc = null;

    [SerializeField]
    private Transform dayAttendanceItemRoot = null;
    [SerializeField]
    private Transform[] weeklyAttendanceItemRoot = null;

    [SerializeField]
    private GameObject[] attendanceItemPrefab = null;

    [SerializeField]
    private AttendanceItem[] specialRewardItem = null;    

    [SerializeField]
    private ToggleGroup categoryToggle = null;

    private bool isFirstOpen = false;                       //최초 생성인지만 확인    
        
    private Dictionary<EnumAttendance.EAttendance, List<AttendanceItemData>> dicAttendanceInfo = new Dictionary<EnumAttendance.EAttendance, List<AttendanceItemData>>();

    private void OnEnable()
    {
        StartCoroutine(ToggleOn(EnumAttendance.EAttendance.Day));
    }

    private void Awake()
    {
        InitAttendanceData();
        SetDayAttendanceItem();
        SetWeeklyAttendanceItem();

        if (isFirstOpen == false)
        {
            isFirstOpen = true;
        }
    }

    public void InitAttendanceData()
    {
        InitDayAttendanceData();
        InitWeeklyAttendanceData();
    }

    private void InitDayAttendanceData()
    {
        if (dicAttendanceInfo.ContainsKey(EnumAttendance.EAttendance.Day) == false)
        {
            List<DataAttendance> dataAttendanceList =
                  DataManager.Instance.DataHelper.Attendance.FindAll(x => x.EAttendance == EnumAttendance.EAttendance.Day);

            List<AttendanceItemData> list = new List<AttendanceItemData>();

            for (int i = 0; i < dataAttendanceList.Count; i++)
            {
                AttendanceItemData itemData = new AttendanceItemData();

                itemData.SetDay(dataAttendanceList[i].ATTENDANCE_DAY);

                DataItem rewardItemData = DataManager.Instance.DataHelper.Item.Find(x => x.UID == dataAttendanceList[i].REWARD_ITEM);

                BaseItemData rewardItem = new BaseItemData();

                rewardItem.SetDataItem(rewardItemData);
                rewardItem.SetTitle(DataManager.Instance.GetLocalization(rewardItemData.ITEM_NAME));
                rewardItem.SetDesc(DataManager.Instance.GetLocalization(rewardItemData.ITEM_DESC));
                rewardItem.SetItemCategory(Enum.Parse<BaseItem.EItemCategory>(rewardItemData.ITEM_CATEGORY));
                rewardItem.SetItemGrade(rewardItemData.ITEM_GRADE);
                rewardItem.SetAtlasPath(rewardItemData.ITEM_ICON_ATLAS);
                rewardItem.SetImgItemPath(rewardItemData.ITEM_ICON);
                rewardItem.SetViewItemCount(dataAttendanceList[i].REWARD_ITEM_COUNT);

                itemData.SetItemData(rewardItem);

                itemData.SetRecive(UserData.Instance.user.DayAttendanceCnt >= i + 1);
                list.Add(itemData);
            }

            dicAttendanceInfo.Add(EnumAttendance.EAttendance.Day, list);
        }

    }

    private void InitWeeklyAttendanceData()
    {
        if (dicAttendanceInfo.ContainsKey(EnumAttendance.EAttendance.Weekly) == false)
        {
            List<DataAttendance> dataAttendanceList =
                    DataManager.Instance.DataHelper.Attendance.FindAll(x => x.EAttendance == EnumAttendance.EAttendance.Weekly);

            List<AttendanceItemData> list = new List<AttendanceItemData>();

            for (int i = 0; i < dataAttendanceList.Count; i++)
            {
                AttendanceItemData itemData = new AttendanceItemData();
                itemData.SetDay(dataAttendanceList[i].ATTENDANCE_DAY);
                DataItem rewardItemData = DataManager.Instance.DataHelper.Item.Find(x => x.UID == dataAttendanceList[i].REWARD_ITEM);

                BaseItemData rewardItem = new BaseItemData();

                rewardItem.SetDataItem(rewardItemData);
                rewardItem.SetTitle(DataManager.Instance.GetLocalization(rewardItemData.ITEM_NAME));
                rewardItem.SetDesc(DataManager.Instance.GetLocalization(rewardItemData.ITEM_DESC));
                rewardItem.SetItemCategory(Enum.Parse<BaseItem.EItemCategory>(rewardItemData.ITEM_CATEGORY));
                rewardItem.SetItemGrade(rewardItemData.ITEM_GRADE);
                rewardItem.SetAtlasPath(rewardItemData.ITEM_ICON_ATLAS);
                rewardItem.SetImgItemPath(rewardItemData.ITEM_ICON);
                rewardItem.SetViewItemCount(dataAttendanceList[i].REWARD_ITEM_COUNT);

                itemData.SetItemData(rewardItem);

                itemData.SetRecive(UserData.Instance.user.WeeklyAttendanceCnt >= i + 1);
                list.Add(itemData);
            }

            dicAttendanceInfo.Add(EnumAttendance.EAttendance.Weekly, list);
        }
    }

    public void SetDayAttendanceItem()
    {
        if(isFirstOpen == false)
        {
            int lastIdx = dicAttendanceInfo[EnumAttendance.EAttendance.Day].Count - 1;

            for (int i = 0; i < lastIdx; i++)
            {
                GameObject go = AddressableManager.Instance.Instantiate(attendanceItemPrefab[(int)EnumAttendance.EAttendance.Day - 1].name, dayAttendanceItemRoot);
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.SetActive(true);

                AttendanceItem item = go.GetComponent<AttendanceItem>();
                item.SetData(dicAttendanceInfo[EnumAttendance.EAttendance.Day][i]);
            }

            specialRewardItem[(int)EnumAttendance.EAttendance.Day - 1].SetData(dicAttendanceInfo[EnumAttendance.EAttendance.Day][lastIdx]);
        }
    }

    public void SetWeeklyAttendanceItem()
    {
        if (isFirstOpen == false)
        {
            int lastIdx = dicAttendanceInfo[EnumAttendance.EAttendance.Weekly].Count - 1;

            for (int i = 0; i < lastIdx; i++)
            {
                Transform parent = i < 3 ? weeklyAttendanceItemRoot[(int)EDirection.Top] : weeklyAttendanceItemRoot[(int)EDirection.Bottom];

                GameObject go = AddressableManager.Instance.Instantiate(attendanceItemPrefab[(int)EnumAttendance.EAttendance.Weekly - 1].name, parent);
                go.transform.localRotation = Quaternion.identity;
                go.transform.localScale = Vector3.one;
                go.SetActive(true);

                AttendanceItem item = go.GetComponent<AttendanceItem>();
                item.SetData(dicAttendanceInfo[EnumAttendance.EAttendance.Weekly][i]);
            }

            specialRewardItem[(int)EnumAttendance.EAttendance.Weekly - 1].SetData(dicAttendanceInfo[EnumAttendance.EAttendance.Weekly][lastIdx]);
        }
    }


    private void SetLabelDesc(EnumAttendance.EAttendance category)
    {
        switch(category)
        {
            case EnumAttendance.EAttendance.Day:
                labelDesc.text = "일일 출석은 28일 기준\n28일 출석 완료 시 보너스 추가 획득";
                break;
            case EnumAttendance.EAttendance.Weekly:
                labelDesc.text = "남은 시간 표시";
                break;
            case EnumAttendance.EAttendance.Event:
                labelDesc.text = "남은 시간 표시";
                break;
        }
    }

    private IEnumerator ToggleOn(EnumAttendance.EAttendance category)
    {
        yield return new WaitForEndOfFrame();

        Transform tf = categoryToggle.transform.Find(category.ToString());

        if (tf != null)
        {
            tf.GetComponent<Toggle>().SetIsOnWithoutNotify(true);
        }
    }

    public void SetCategory(EnumAttendance.EAttendance category)
    {
        state.ActiveState(category.ToString());
        SetLabelDesc(category);
    }

    public void OnClickCategory(UtilEnumSelect category)
    {
        SetCategory(category.AttendanceType);
    }
}

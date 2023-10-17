using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Quest : MonoSingletonInScene<Quest>
{
    [SerializeField]
    private UtilState state = null;
    [SerializeField]
    private ToggleGroup categoryToggle = null;

    [SerializeField]
    private QuestScrollController scrollViewController = null;

    private Dictionary<QuestData.EQuestType, List<QuestData>> questDataDic = new Dictionary<QuestData.EQuestType, List<QuestData>>();                       // 퀘스트 목록

    private EnumQuest.EQuest currentCategory = EnumQuest.EQuest.Day;

    private void OnEnable()
    {
        StartCoroutine(ToggleOn(EnumQuest.EQuest.Day));
    }

    public void InitQuestData(List<QuestData> questList)
    {
        SetDayQuestData(questList);
        SetGuideQuestData(questList);

        SetCategory(currentCategory);
    }

    public void RefreshQuestData(List<QuestData> questList)
    {
        if(questList != null && questList.Count > 0)
        {
            for(int i = 0; i < questList.Count; i++)
            {
                int questDataidx = questDataDic[questList[i].QuestType].FindIndex(x => x.QuestUID == questList[i].QuestUID);

                if (questDataidx != -1)
                {
                    questDataDic[questList[i].QuestType][i] = questList[i];
                }
            }

            SetCategory(currentCategory);
        }
    }

    private void SetDayQuestData(List<QuestData> questList)
    {
        List<QuestData> questDataList = questList.FindAll(x => x.QuestType == QuestData.EQuestType.Day);

        if (questDataList.Count > 0)
        {
            if (questDataDic.ContainsKey(QuestData.EQuestType.Day) == false)
            {
                questDataDic.Add(QuestData.EQuestType.Day, questDataList);
            }
            else
            {
                questDataDic[QuestData.EQuestType.Day] = questDataList;
            }
        }
    }

    private void SetGuideQuestData(List<QuestData> questList)
    {
        List<QuestData> questDataList = questList.FindAll(x => x.QuestType == QuestData.EQuestType.Guide);

        if (questDataList.Count > 0)
        {
            if (questDataDic.ContainsKey(QuestData.EQuestType.Guide) == false)
            {
                questDataDic.Add(QuestData.EQuestType.Guide, questDataList);
            }
            else
            {
                questDataDic[QuestData.EQuestType.Guide] = questDataList;
            }
        }
    }
   

    private void SetQuestType(QuestData.EQuestType type)
    {
        scrollViewController.SetQuestDataInfo(questDataDic[type]);        
    }

    public void ClearRewardItem(QuestData.EQuestType type)
    {
        SortClearQuest(type);

        scrollViewController.ScrollViewRefresh();
    }

    private void SortClearQuest(QuestData.EQuestType type)
    {
        questDataDic[type].Sort((x, y) => x.ClearType - y.ClearType);
    }


    private IEnumerator ToggleOn(EnumQuest.EQuest category)
    {
        yield return new WaitForEndOfFrame();

        Transform tf = categoryToggle.transform.Find(category.ToString());

        if (tf != null)
        {
            tf.GetComponent<Toggle>().SetIsOnWithoutNotify(true);
        }
    }

    public void SetCategory(EnumQuest.EQuest category)
    {
        string stateName = QuestData.EQuestType.Day.ToString();

        state.ActiveState(stateName);

        switch (category)
        {
            case EnumQuest.EQuest.Day:
                SetQuestType(QuestData.EQuestType.Day);
                break;
            case EnumQuest.EQuest.Guide:
                SetQuestType(QuestData.EQuestType.Guide);
                break;
        }

        currentCategory = category;
    }

    public void OnClickCategory(UtilEnumSelect category)
    {        
        SetCategory(category.QuestType);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.U2D;

public class Stage : MonoBehaviour
{
    private List<StageData> stageDataList = new List<StageData>();

    private StageData currentStageData = null;

    private int stageIdx = 0;

    public StageData CurrentStageData => currentStageData;

    public void InitStage()
    {        
        for (int i = 0; i < DataManager.Instance.DataHelper.Stage.Count; i++)
        {
            StageData stageData = new StageData();
            stageData.SetUID(DataManager.Instance.DataHelper.Stage[i].UID);
            stageData.SetStageTitle(DataManager.Instance.GetLocalization(DataManager.Instance.DataHelper.Stage[i].STAGE_NAME));
            stageData.SetStagePrefabPath(DataManager.Instance.DataHelper.Stage[i].STAGE_BACK);
            stageData.SetStageType(DataManager.Instance.DataHelper.Stage[i].STAGE_TYPE);
            stageData.SetStageDifficult(DataManager.Instance.DataHelper.Stage[i].STAGE_DIFFICULT);
            stageData.SetStageRegenGroupUID(DataManager.Instance.DataHelper.Stage[i].REGEN_GROUP);
            stageDataList.Add(stageData);
        }

        stageIdx = UserData.Instance.user.Stage.StageUID - 1;
        SetStage(stageDataList[UserData.Instance.user.Stage.StageUID - 1]);
    }


    private void SetStage(StageData stageData)
    {
        currentStageData = stageData;
    }

    public void SetNextStage()
    {
        int idx = stageIdx + 1;

        if (idx > stageDataList.Count - 1)
        {
            idx = stageDataList.Count - 1;
        }

        stageIdx = idx;

        SetStage(stageDataList[idx]);
    }
}

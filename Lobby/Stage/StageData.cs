using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class StageData
{
    public enum EStageType
    {
        NONE,
        NORMAL,
        HERO,
        BOSS,
    }

    public enum EStageDifficult
    {
        EASY,
        NORMAL,
        HARD,
        HARDER,
        HELL,
    }

    //�������� UID
    private int uid;
    //�������� Ÿ��Ʋ
    private string stageTitle;
    //�������� ��� �̹��� ���
    private string stagePrefabPath;

    private EStageType type = EStageType.NONE;
    private EStageDifficult stageDifficult = EStageDifficult.EASY;

    private int stageRegenGroupUID;

    public int UID => uid;
    public string StageTitle => stageTitle;
    public string StagePrefabPath => stagePrefabPath;
    public EStageType Type => type;
    public EStageDifficult StageDifficult => stageDifficult;
    public int StageRegenGroupUID => stageRegenGroupUID;


    public void SetUID(int uid)
    {
        this.uid = uid;
    }

    public void SetStageTitle(string stageTitle)
    {
        this.stageTitle = stageTitle;
    }

    public void SetStagePrefabPath(string stagePrefabPath)
    {
        this.stagePrefabPath = stagePrefabPath;
    }

    public void SetStageType(string type)
    {
        if(Enum.TryParse<EStageType>(type, out this.type) == false)
        {
            Debug.LogError("�������� Ÿ�� ����");
        }
    }
    public void SetStageDifficult(string stageDifficult)
    {
        if (Enum.TryParse<EStageDifficult>(stageDifficult, out this.stageDifficult) == false)
        {
            Debug.LogError("�������� ���̵� Ÿ�� ����");
        }
    }

    public void SetStageRegenGroupUID(int stageRegenGroupUID)
    {
        this.stageRegenGroupUID = stageRegenGroupUID;
    }
}

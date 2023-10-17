using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnumQuest : BaseEnum
{
    public enum EQuest
    {
        None,

        Day,
        Guide,
    }

    private EQuest questType;
    public EQuest QuestType => questType;

    public void SetQuestType(EQuest type)
    {
        questType = type;
    }
}

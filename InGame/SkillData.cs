using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillData
{
    public enum ESkillCategory
    {
        NONE,

        ENEMY_COUNT,
        CHARACTER
    }

    public enum ESkillType
    {
        NONE,

        BUFF,
        ACTIVE
    }

    public enum ESkillTarget
    {
        NONE,

        SELF,
        ALLY,
        TARGET,
        AREA
    }

    public enum ESkillTargetType
    {
        NONE,

        HERO,
        ENEMY,
        ENEMY_RAIL,                 // 일직선
        ENEMY_ROUND,                // 원형
        BOSS,
        GROUND,
    }

    private string title = "";
    private string desc = "";

    private string atlasSkill = "";
    private string skillImgPath = "";
    private string skillEffectPath = "";

    private int skillGroup = 0;
    private int skillLevel = 0;
    private int skillAddLevel = 0;

    private ESkillCategory category = ESkillCategory.NONE;
    private ESkillType type = ESkillType.NONE;

    private float coolTime = 0;
    private float time = 0;
    private float interval = 0;

    private float range = 0;

    private ESkillTarget target = ESkillTarget.NONE;
    private ESkillTargetType targetType = ESkillTargetType.NONE;

    private float skillRange = 0;

    private List<BaseOptionData> skillOptions = new List<BaseOptionData>();

    private DataSkillBase dataSkill = null;

    public string Title => title;
    public string Desc => desc;
    public string AtlasSkill => atlasSkill;
    public string SkillImgPath => skillImgPath;
    public string SkillEffectPath => skillEffectPath;
    public int SkillGroup => skillGroup;
    public int SkillLevel => skillLevel;
    public int SkillAddLevel => skillAddLevel;

    public ESkillCategory Category => category;
    public ESkillType Type => type;
    public float CoolTime => coolTime;
    public float Time => time;
    public float Interval => interval;
    public float Range => range;
    public ESkillTarget Target => target;
    public ESkillTargetType TargetType => targetType;

    public float SkillRange => skillRange;

    public List<BaseOptionData> SkillOptions => skillOptions;

    public DataSkillBase DataSkill => dataSkill;


    public void SetSkillData(DataSkillBase dataSkill)
    {
        this.dataSkill = dataSkill;
    }

    public void SetTitle(string title)
    {
        this.title = title;
    }

    public void SetDesc(string desc) 
    {
        this.desc = desc;
    }

    public void SetSkillAtlas(string atlasSkill)
    {
        this.atlasSkill = atlasSkill;
    }

    public void SetSkillImgPath(string skillImgPath)
    {
        this.skillImgPath = skillImgPath;
    }

    public void SetSkillEffectPath(string skillEffectPath)
    {
        this.skillEffectPath = skillEffectPath;
    }

    public void SetSkillGroup(int skillGroup)
    {
        this.skillGroup = skillGroup;
    }

    public void SetSkillLevel(int skillLevel)
    {
        this.skillLevel = skillLevel;
    }

    public void SetSkillAddLevel(int skillAddLevel)
    {
        this.skillAddLevel = skillAddLevel;
    }

    public void SetCategory(ESkillCategory category)
    {
        this.category = category;
    }

    public void SetCategory(string category)
    {
        this.category = Enum.Parse<ESkillCategory>(category);
    }

    public void SetType(ESkillType type)
    {
        this.type = type;
    }

    public void SetType(string type)
    {
        this.type = Enum.Parse<ESkillType>(type);
    }

    public void SetCoolTime(float coolTime)
    {
        this.coolTime = coolTime;
    }
    public void SetTime(float time)
    {
        this.time = time;
    }
    public void SetInterval(float interval)
    {
        this.interval = interval;
    }
    public void SetRange(float range)
    {
        this.range = range;
    }

    public void SetTarget(ESkillTarget target)
    {
        this.target = target;
    }

    public void SetTarget(string target)
    {
        this.target = Enum.Parse<ESkillTarget>(target);
    }

    public void SetTargetType(ESkillTargetType targetType)
    {
        this.targetType = targetType;
    }

    public void SetTargetType(string targetType)
    {
        this.targetType = Enum.Parse<ESkillTargetType>(targetType);        
    }

    public void SetSkillRange(float skillRange)
    {
        this.skillRange = skillRange;
    }

    public void SetSkillOption(DataOptionSet option)
    {
        if(option == null)
        {
            return;
        }

        BaseOptionData optionData = new BaseOptionData();

        optionData.SetOptionCategory(Enum.Parse<BaseOptionData.EOptionCategory>(option.OPTION_CATEGORY));
        optionData.SetOptionType(Enum.Parse<BaseOptionData.EOptionType>(option.OPTION_TYPE));
        optionData.SetValueType(Enum.Parse<BaseOptionData.EOptionValueType>(option.VALUE_TYPE));
        optionData.SetActiveType((BaseOptionData.EOptionActiveType)option.OPTION_VALUE_1);
        optionData.SetValue((float)option.OPTION_VALUE_2);

        if (skillOptions.Contains(optionData))
        {
            return;
        }
        else
        {
            skillOptions.Add(optionData);
        }
    }

    public void SetSkillOption(BaseOptionData option)
    {
        if (skillOptions.Contains(option))
        {
            return;
        }
        else
        {
            skillOptions.Add(option);
        }
    }

}

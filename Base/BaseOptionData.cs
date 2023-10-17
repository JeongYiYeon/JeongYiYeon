using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseOptionData
{
    public enum EOptionCategory
    {
        NONE,

        ITEM,
        SKILL,
    }

    public enum EOptionType
    {
        NONE,
        ATTACK_DAM,                     // 추뎀
        ATTACK_DAM_UP,                  // 공격력 증가
        ATTACK_DAM_DOWN,                // 공격력 감소
        HP_UP,                          // 피통 증가
        HP_DOWN,                        // 피통 감소
        CRIRATE_UP,                     // 크리 확률 증가
        CRIRATE_DOWN,                   // 크리 확률 하락
        ATTACK_SPEED_UP,                // 공속 증가
        ATTACK_SPEED_DOWN,              // 공속 감소
        SKILL_LEVEL_UP,                 // 모든 스킬 렙업
        SKILL_LEVEL_DOWN,               // 모든 스킬 렙업감소
        SKILL_COOLTIME_DECR,            // 스킬 쿨감
        HP_RECOVERY,                    // 체력 회복 
        DOUBLE_ATTACK,                  // 더블 어택
        TRIPLE_ATTACK,                  // 트리플 어택
        CRIDAM_UP,                      // 크리티컬 데미지 증가
        CRIDAM_DOWN,                    // 크리티컬 데미지 하락
        GET_GOLD,                       // 골드 추가 획득
    }

    public enum EOptionValueType
    {
        NONE,

        PER,                            // % 증가
        PLUS,                           // + 증가        
    }


    public enum EOptionActiveType
    {
        NONE,

        BUFF,
        ACTIVE
    }

    private EOptionCategory category = EOptionCategory.NONE;
    private EOptionType type = EOptionType.NONE;
    private EOptionValueType valueType = EOptionValueType.NONE;
    private EOptionActiveType activeType = EOptionActiveType.NONE;
    private float value = 0f;

    public EOptionCategory Category => category;
    public EOptionType Type => type;
    public EOptionValueType ValueType => valueType;
    public EOptionActiveType ActiveType => activeType;
    public float Value => value;

    public void SetOptionCategory(EOptionCategory category)
    {
        this.category = category;
    }

    public void SetOptionType(EOptionType type)
    {
        this.type = type;
    }

    public void SetValueType(EOptionValueType valueType)
    {
        this.valueType = valueType;
    }

    public void SetActiveType(EOptionActiveType activeType)
    {
        this.activeType = activeType;
    }

    public void SetValue(float value)
    {
        this.value = value;
    }

    public double CalculateAddValue(double statValue = 0f)
    {
        if (valueType == EOptionValueType.PER)
        {
            if (statValue == 0)
            {
                return value / 100f;
            }
            else
            {
                return (statValue * (value / 100f)) - statValue;
            }
        }
        else if (valueType == EOptionValueType.PLUS)
        {
            return value;
        }
        else
        {
            return 0f;
        }
    }
}

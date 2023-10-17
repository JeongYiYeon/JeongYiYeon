using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;

public class SkillManager : MonoSingletonInScene<SkillManager>
{
    [SerializeField]
    private Transform skillRoot = null;

    [SerializeField]
    private SkillItem[] skillItems = null;

    private protected IObjectPool<Skill> skillPool;
    private Dictionary<int, List<SkillData>> skillDic = new Dictionary<int, List<SkillData>>();
    private Dictionary<int, SkillData> haveSkillDic = new Dictionary<int, SkillData>();

    public SkillItem[] SkillItems => skillItems;
    public IObjectPool<Skill> SkillPool => skillPool;
    public Dictionary<int, List<SkillData>> SkillDic => skillDic;
    public Dictionary<int, SkillData> HaveSkillDic => haveSkillDic;

    private void OnEnable()
    {     
        StartCoroutine(IEWaitGamemanager());
    }

    public void SetSkillDic(Dictionary<int, List<SkillData>> skillDic)
    {
        this.skillDic = skillDic;
    }

    private IEnumerator IEWaitGamemanager()
    {
        yield return new WaitUntil(() => GameManager.Instance != null);

        InitSkillPool();
    }

    public void SetHaveSkill(SkillData skill)
    {
        if(haveSkillDic.ContainsKey(skill.SkillGroup) == false)
        {
            haveSkillDic.Add(skill.SkillGroup, skill);
        }
        else
        {
            haveSkillDic[skill.SkillGroup] = skill;
        }
    }

    public void CacluateStat(SkillData skill, BaseOptionData skillOption)
    {
        switch (skillOption.Type)
        {
            case BaseOptionData.EOptionType.ATTACK_DAM:
            case BaseOptionData.EOptionType.ATTACK_DAM_UP:
                {
                    if (skill.Target == SkillData.ESkillTarget.SELF)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffDam((float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.Damage));
                    }

                    if (skill.Target == SkillData.ESkillTarget.ALLY)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffDam((float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.Damage));

                        for (int i = 0; i < GameManager.Instance.NpcHeroCharacters.Length; i++)
                        {
                            GameManager.Instance.NpcHeroCharacters[i].AddBuffDam(
                                (float)skillOption.CalculateAddValue(GameManager.Instance.NpcHeroCharacters[i].Damage));
                        }
                    }
                }
                break;

            case BaseOptionData.EOptionType.ATTACK_DAM_DOWN:
                {
                    if (skill.Target == SkillData.ESkillTarget.SELF)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffDam(-(float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.Damage));
                    }

                    if (skill.Target == SkillData.ESkillTarget.ALLY)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffDam(-(float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.Damage));

                        for (int i = 0; i < GameManager.Instance.NpcHeroCharacters.Length; i++)
                        {
                            GameManager.Instance.NpcHeroCharacters[i].AddBuffDam(
                                -(float)skillOption.CalculateAddValue(GameManager.Instance.NpcHeroCharacters[i].Damage));
                        }
                    }
                }
                break;

            case BaseOptionData.EOptionType.HP_UP:
                {
                    if (skill.Target == SkillData.ESkillTarget.SELF)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffHp((float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.MaxHP));
                    }

                    if (skill.Target == SkillData.ESkillTarget.ALLY)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffHp((float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.MaxHP));

                        for (int i = 0; i < GameManager.Instance.NpcHeroCharacters.Length; i++)
                        {
                            GameManager.Instance.NpcHeroCharacters[i].AddBuffHp(
                                (float)skillOption.CalculateAddValue(GameManager.Instance.NpcHeroCharacters[i].MaxHP));
                        }
                    }
                }
                break;
            case BaseOptionData.EOptionType.HP_DOWN:
                {
                    if (skill.Target == SkillData.ESkillTarget.SELF)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffHp(-(float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.MaxHP));
                    }

                    if (skill.Target == SkillData.ESkillTarget.ALLY)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffHp(-(float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.MaxHP));

                        for (int i = 0; i < GameManager.Instance.NpcHeroCharacters.Length; i++)
                        {
                            GameManager.Instance.NpcHeroCharacters[i].AddBuffHp(
                                -(float)skillOption.CalculateAddValue(GameManager.Instance.NpcHeroCharacters[i].MaxHP));
                        }
                    }
                }
                break;
            case BaseOptionData.EOptionType.CRIRATE_UP:
                {
                    if (skill.Target == SkillData.ESkillTarget.SELF)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffCri((float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.Critical));
                    }

                    if (skill.Target == SkillData.ESkillTarget.ALLY)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffCri((float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.Critical));

                        for (int i = 0; i < GameManager.Instance.NpcHeroCharacters.Length; i++)
                        {
                            GameManager.Instance.NpcHeroCharacters[i].AddBuffCri(
                                (float)skillOption.CalculateAddValue(GameManager.Instance.NpcHeroCharacters[i].Critical));
                        }
                    }
                }
                break;
            case BaseOptionData.EOptionType.CRIRATE_DOWN:
                {
                    if (skill.Target == SkillData.ESkillTarget.SELF)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffCri(-(float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.Critical));
                    }

                    if (skill.Target == SkillData.ESkillTarget.ALLY)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffCri(-(float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.Critical));

                        for (int i = 0; i < GameManager.Instance.NpcHeroCharacters.Length; i++)
                        {
                            GameManager.Instance.NpcHeroCharacters[i].AddBuffCri(
                                -(float)skillOption.CalculateAddValue(GameManager.Instance.NpcHeroCharacters[i].Critical));
                        }
                    }
                }
                break;
            case BaseOptionData.EOptionType.ATTACK_SPEED_UP:
                {
                    if (skill.Target == SkillData.ESkillTarget.SELF)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffAtkSpd((float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.OriginAtkSpd));
                    }

                    if (skill.Target == SkillData.ESkillTarget.ALLY)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffAtkSpd((float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.OriginAtkSpd));

                        for (int i = 0; i < GameManager.Instance.NpcHeroCharacters.Length; i++)
                        {
                            GameManager.Instance.NpcHeroCharacters[i].AddBuffAtkSpd(
                                (float)skillOption.CalculateAddValue(GameManager.Instance.NpcHeroCharacters[i].OriginAtkSpd));
                        }
                    }
                }
                break;
            case BaseOptionData.EOptionType.ATTACK_SPEED_DOWN:
                {
                    if (skill.Target == SkillData.ESkillTarget.SELF)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffAtkSpd(-(float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.OriginAtkSpd));
                    }

                    if (skill.Target == SkillData.ESkillTarget.ALLY)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffAtkSpd(-(float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.OriginAtkSpd));

                        for (int i = 0; i < GameManager.Instance.NpcHeroCharacters.Length; i++)
                        {
                            GameManager.Instance.NpcHeroCharacters[i].AddBuffAtkSpd(
                                -(float)skillOption.CalculateAddValue(GameManager.Instance.NpcHeroCharacters[i].OriginAtkSpd));
                        }
                    }
                }
                break;

            case BaseOptionData.EOptionType.SKILL_COOLTIME_DECR:
                if (skillOption.ActiveType != BaseOptionData.EOptionActiveType.NONE)
                {
                    if ((int)skill.Type == (int)skillOption.ActiveType)
                    {
                        float coolTime = Mathf.Clamp(skill.CoolTime - skillOption.Value, 0, skill.CoolTime - skillOption.Value);

                        skill.SetCoolTime(coolTime);
                    }
                }
                break;

            case BaseOptionData.EOptionType.HP_RECOVERY:
            case BaseOptionData.EOptionType.DOUBLE_ATTACK:
            case BaseOptionData.EOptionType.TRIPLE_ATTACK:
            case BaseOptionData.EOptionType.CRIDAM_UP:
                {
                    if (skill.Target == SkillData.ESkillTarget.SELF)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffCriDam((float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.CriticalDamage));
                    }

                    if (skill.Target == SkillData.ESkillTarget.ALLY)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffCriDam((float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.CriticalDamage));

                        for (int i = 0; i < GameManager.Instance.NpcHeroCharacters.Length; i++)
                        {
                            GameManager.Instance.NpcHeroCharacters[i].AddBuffCriDam(
                                (float)skillOption.CalculateAddValue(GameManager.Instance.NpcHeroCharacters[i].CriticalDamage));
                        }
                    }
                }
                break;
            case BaseOptionData.EOptionType.CRIDAM_DOWN:
                {
                    if (skill.Target == SkillData.ESkillTarget.SELF)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffCriDam(-(float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.CriticalDamage));
                    }

                    if (skill.Target == SkillData.ESkillTarget.ALLY)
                    {
                        GameManager.Instance.HeroCharacter.AddBuffCriDam(-(float)skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.CriticalDamage));

                        for (int i = 0; i < GameManager.Instance.NpcHeroCharacters.Length; i++)
                        {
                            GameManager.Instance.NpcHeroCharacters[i].AddBuffCriDam(
                                -(float)skillOption.CalculateAddValue(GameManager.Instance.NpcHeroCharacters[i].CriticalDamage));
                        }
                    }
                }
                break;
            case BaseOptionData.EOptionType.GET_GOLD:
                {
                    GameManager.Instance.SetMultipleDropcoin((float)skillOption.CalculateAddValue());
                }
                break;
        }
    }

    public double CacluateDamage(BaseOptionData skillOption)
    {
        switch (skillOption.Type)
        {
            case BaseOptionData.EOptionType.ATTACK_DAM:
            case BaseOptionData.EOptionType.ATTACK_DAM_UP:                
                return skillOption.CalculateAddValue(GameManager.Instance.HeroCharacter.Damage);

            default:
                return 0;
        }
    }


    #region 스킬 풀
    private protected void InitSkillPool()
    {
        skillPool = new ObjectPool<Skill>(
            createFunc: InitSkill,
            actionOnGet: GetSkill,
            actionOnRelease: ReleaseSkill,
            actionOnDestroy: DestorySkill,
            maxSize: 15
            );

    }

    private protected Skill InitSkill()
    {
        GameObject tmpSkill = AddressableManager.Instance.Instantiate("BaseSkill", skillRoot);
        tmpSkill.name = tmpSkill.name.Replace("(Clone)", "");

        Skill skill = tmpSkill.GetComponent<Skill>();

        skill.gameObject.SetActive(false);

        return skill;
    }

    private protected void GetSkill(Skill skill)
    {
        skill.gameObject.SetActive(true);
    }

    private protected void ReleaseSkill(Skill skill)
    {
        skill.gameObject.SetActive(false);
    }

    private protected void DestorySkill(Skill skill)
    {
        Destroy(skill.gameObject);
    }
    #endregion

}

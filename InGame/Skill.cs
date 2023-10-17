using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using Coffee.UIExtensions;
using UnityEngine.U2D;
using UniRx.Examples;

public class Skill : BaseCharacter
{    
    private SkillData skill = null;

    private Collider2D[] splashColls = new Collider2D[50];

    private void Awake()
    {
        StartCoroutine(IEWaitGamemanager());
    }

    private IEnumerator IEWaitGamemanager()
    {
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.IsInit);
        InitStat();
    }

    public override void FixedUpdate()
    {
        
    }

    public void SetSkill(SkillData skill)
    {
        this.skill = skill;

        if(skill != null)
        {
            SetDam(SkillManager.Instance.CacluateDamage(skill.SkillOptions[0]));

            if (skill.Target == SkillData.ESkillTarget.AREA)
            {
                AddSplashSize(1f);
            }
            else
            {
                AddSplashSize(0);
            }

            Transform skillTf = this.transform.Find(skill.SkillEffectPath);

            if (skillTf == null)
            {
                GameObject go = AddressableManager.Instance.Instantiate(skill.SkillEffectPath, this.transform);
                go.name = go.name.Replace("(Clone)", "");
            }
            else
            {
                skillTf.gameObject.SetActive(true);
            }
        }
    }

    public void ActiveSkill(GameObject go)
    {
        if(go == null)
        {
            return;
        }

        if (go.tag == ConstHelper.TAG_PLAYER)
        {
            return;
        }

        EnemyCharacter enemy = go.GetComponent<EnemyCharacter>();

        if (enemy != null)
        {
            if(SplashSize > 0)
            {
                Array.Clear(splashColls, 0, splashColls.Length);
                int layerMask = 1 << LayerMask.NameToLayer(ConstHelper.LAYER_CHARACTER);
                int colliderCnt = Physics2D.OverlapCircleNonAlloc(go.transform.position, SplashSize, splashColls, layerMask);

                if (colliderCnt > 0)
                {
                    foreach (Collider2D col in splashColls)
                    {
                        if (col == null)
                        {
                            continue;
                        }

                        if (col.tag == ConstHelper.TAG_NPC)
                        {
                            continue;
                        }
                        //총알 타입은 충돌체크 패스시킴
                        else if (col.tag == ConstHelper.TAG_PROJECTILE)
                        {
                            continue;
                        }
                        else
                        {
                            EnemyCharacter tmpEnemy = col.gameObject.transform.GetComponent<EnemyCharacter>();

                            if (tmpEnemy != null)
                            {
                                CalculateSkill(tmpEnemy);
                            }
                        }
                    }
                }
            }
            else
            {
                CalculateSkill(enemy);
            }           
        }
    }

    private void CalculateSkill(EnemyCharacter enemy)
    {
        if (enemy == null)
        {
            return;
        }

        if (enemy.TotalHP > 0)
        {
            if (enemy.gameObject.activeSelf)
            {
                bool isCritical = IsCritical();
                double multipleCriDam = isCritical == true ? TotalCriticalDamage / 100f : 1f;
                EDamageHit dmgHitType = isCritical == true ? EDamageHit.Critical : EDamageHit.Hit;

                enemy.ActiveHitEffect(isCritical);
                enemy.CheckHpBar();
                enemy.CalculateHP(this, dmgHitType, multipleCriDam);
            }
            if (enemy.TotalHP <= 0)
            {
                enemy.GetDropCoin();
                enemy.ActiveDeadEffect(() =>
                {
                    enemy.ReturnEnemy();
                });
            }
        }
        else
        {
            if (enemy.gameObject.activeSelf)
            {
                enemy.GetDropCoin();
                enemy.StartCoroutine(enemy.IEActiveDeadEffect(() =>
                {
                    enemy.ReturnEnemy();
                }));
            }
        }
    }
}

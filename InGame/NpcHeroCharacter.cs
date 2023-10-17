using Coffee.UIEffects;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NpcHeroCharacter : BaseCharacter
{
    private bool isInit = false;

    private void Awake()
    {
        StartCoroutine(IEWaitGamemanager());
    }

    public override void FixedUpdate()
    {
        if (isInit == true)
        {
            animationTime += Time.deltaTime;

            if (GameManager.Instance.IsWaveWait == true)
            {
                SetChangeState(AnimationType.Run);
            }
            else
            {
                if (isCurrentEqualState(AnimationType.Attack) == false &&
                    isCurrentEqualState(AnimationType.Hit) == false)
                {
                    SetChangeState(AnimationType.Idle);
                }
            }

            base.FixedUpdate();
        }
    }

    public void RefreshCharacter()
    {
        CharacterAnimation.SetAtlas(Character.CharacterAtlasName);
        this.transform.SetParent(GetFollowTarget(Character.PositionIdx));
        this.transform.localPosition = Vector3.zero;

        followTarget.SetFollowTarget(GetFollowTarget(Character.PositionIdx));
        followTarget.MoveFollowTarget();
    }

    private void InitCharacterInfo()
    {
        CharacterAnimation.SetAtlas(Character.CharacterAtlasName);

        fsmManager.SetFSM(AnimationType.Idle.ToString(), new StateIdle(this));
        fsmManager.SetFSM(AnimationType.Run.ToString(), new StateMove(this));
        fsmManager.SetFSM(AnimationType.Attack.ToString(), new StateAttack(this));
        fsmManager.SetFSM(AnimationType.Hit.ToString(), new StateHit(this));

        this.transform.SetParent(GetFollowTarget(Character.PositionIdx));
        this.transform.localPosition = Vector3.zero;

        followTarget.SetFollowTarget(GetFollowTarget(Character.PositionIdx));
        followTarget.MoveFollowTarget();
    }

    private IEnumerator IEWaitGamemanager()
    {
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.IsInit);

        if (AttCategory == ATT_CATEGORY.RANGE)
        {
            InitProjectilePool();
        }

        InitCharacterInfo();

        SetChangeState(AnimationType.Run);

        StartCoroutine(IECheckEnemy());

        isInit = true;
    } 

    private IEnumerator IEShoot()
    {
        if (AttCategory == ATT_CATEGORY.RANGE)
        {
            projectilePool.Get(out Projectile go);

            if (go != null)
            {
                if (go.gameObject.activeSelf == false)
                {
                    go.gameObject.SetActive(true);
                }

                StartCoroutine(go.IEMove(lastViewDir, colTargetPos));
            }
        }
        else
        {
            if (colGo != null)
            {
                EnemyCharacter enemy = colGo.GetComponent<EnemyCharacter>();

                if (enemy != null)
                {
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
                            enemy.StartCoroutine(enemy.IEActiveDeadEffect(() =>
                            {
                                enemy.ReturnEnemy();
                            }));
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
        }

        yield return new WaitForSeconds(TotalAttackSpd);

        isAttack = false;
    }

    private IEnumerator IECheckEnemy()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (IsCollision == true && isAttack == false)
            {
                if (GameManager.Instance.GameSpeed > 0 && GameManager.Instance.IsWaveWait == false)
                {
                    isAttack = true;

                    SetChangeState(AnimationType.Attack);

                    yield return StartCoroutine(IEShoot());
                }
            }
        }
    }


}

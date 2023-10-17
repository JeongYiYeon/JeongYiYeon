using Coffee.UIExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class Projectile : BaseCharacter
{
    [SerializeField]
    private AtlasImage projectileImg = null;

    [SerializeField]
    private RectTransform shadowImgTf = null;

    [SerializeField]
    private float lifeTime = 0;

    private Collider2D[] splashColls = new Collider2D[50];

    private IObjectPool<Projectile> pool = null;

    private CHARACTER_TYPE characterType = CHARACTER_TYPE.NONE;
    private CHARACTER_ATT_TYPE characterAttType = CHARACTER_ATT_TYPE.NONE;

    private float time = 0;
    private float splashRadius = 1f;

    private bool isGoalTarget = false;

    private bool isCollision = false;

    private Vector3 originTargetPos = Vector3.zero;
    public CHARACTER_ATT_TYPE CharacterAttType => characterAttType;

    public override void Reset()
    {
        base.Reset();

        time = 0;
        isGoalTarget = false;
        isCollision = false;
        projectileImg.enabled = true;

        this.transform.position = Vector3.zero;
        this.transform.rotation = Quaternion.identity;
        //shadowImgTf.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -60f);
    }

    private void Awake()
    {
        StartCoroutine(IEWaitGamemanager());
    }

    public override void FixedUpdate()
    {
        
    }

    private IEnumerator IEWaitGamemanager()
    {
        yield return new WaitUntil(() => GameManager.Instance != null && GameManager.Instance.IsInit);
        InitStat();
    }


    public void SetProjectile(IObjectPool<Projectile> pool, float lifeTime)
    {
        this.pool = pool;
        this.lifeTime = lifeTime;
    }

    public void SetCharacterType(CHARACTER_TYPE type)
    {
        this.characterType = type;
    }

    public void SetAttackType(CHARACTER_ATT_TYPE characterAttType)
    {
        this.characterAttType = characterAttType;
    }

    public void SetSplashRadius(float splashRadius)
    {
        this.splashRadius = 1f + (splashRadius / 10f);
    }

    public void SetProjectileImg(string imgPath)
    {
        AtlasManager.Instance.SetSprite(projectileImg, projectileImg.spriteAtlas, imgPath);
    }

    public void SetStartPosition(Transform posTf)
    {
        this.transform.position = posTf.position;
    }

    private void GetBezierCurve(Vector3 startPos, Vector3 targetPos, Vector2 dir, out Vector3 nextPos)
    {
        //캔버스 스캐일러의 pixelPerUnit 만큼 속도에서 나눠줌 -> moveSpd / 100f

        float distance = 0;
        float arc = 0;

        Vector3 nextPosition = Vector3.zero;

        if (dir == Vector2.left || dir == Vector2.right)
        {
            float m_HeightArc = 0.3f;

            distance = targetPos.x - startPos.x;
            float nextX = Mathf.MoveTowards(transform.position.x, targetPos.x, MoveSpd * GameManager.Instance.GameSpeed * Time.deltaTime);

            float baseY = Mathf.Lerp(startPos.y, targetPos.y, (nextX - startPos.x) / distance);

            arc = m_HeightArc * (nextX - startPos.x) * (nextX - targetPos.x) / (-0.25f * distance * distance);
            nextPosition = new Vector3(nextX, baseY + arc, transform.position.z);
        }
        else
        {
            float m_WidthArc = 0.3f;

            distance = targetPos.y - startPos.y;
            float nextY = Mathf.MoveTowards(transform.position.y, targetPos.y, MoveSpd * GameManager.Instance.GameSpeed * Time.deltaTime);

            float baseX = Mathf.Lerp(startPos.x, targetPos.x, (nextY - startPos.y) / distance);

            arc = m_WidthArc * (nextY - startPos.y) * (nextY - targetPos.y) / (-0.25f * distance * distance);
            nextPosition = new Vector3(baseX + arc, nextY, transform.position.z);
        }

        projectileImg.transform.localRotation = Quaternion.Euler(0, 0, -90f);
        projectileImg.transform.localRotation *= Quaternion.Euler(0, 0, Mathf.Atan2((nextPosition - transform.position).y,
            (nextPosition - transform.position).x) * Mathf.Rad2Deg);

        this.transform.position = nextPosition;
        shadowImgTf.position = new Vector3(nextPosition.x, targetPos.y - arc, nextPosition.z);

        nextPos = nextPosition;
    }

    public IEnumerator IEMoveInBezier(Vector3 startPos, Vector3 targetPos, Vector2 dir, Action<Vector3> callback = null)
    {
        originTargetPos = targetPos;

        Array.Clear(splashColls, 0, splashColls.Length);

        Vector3 nextPos = Vector3.zero;
        GetBezierCurve(startPos, targetPos, dir, out nextPos);

        while (this.gameObject.activeSelf == true)
        {
            if (GameManager.Instance.IsWaveWait)
            {
                ReturnProjectile();

                yield break;
            }

            if (Vector3.Distance(originTargetPos, nextPos) <= 0)
            {
                isGoalTarget = true;

                ReturnProjectile();

                yield break;
            }

            else
            {
                GetBezierCurve(startPos, originTargetPos, dir, out nextPos);
            }
            yield return null;
        }        
    }

    private IEnumerator IEMoveProjectile(Vector3 targetPos)
    {
        Array.Clear(splashColls, 0, splashColls.Length);

        int layerMask = 1 << LayerMask.NameToLayer(ConstHelper.LAYER_CHARACTER);

        Vector3 vTarget = targetPos - transform.position;
        Vector3 vDir = vTarget.normalized;

        this.transform.localRotation = Quaternion.Euler(0, 0, -90f);
        this.transform.localRotation *= Quaternion.Euler(0, 0, Mathf.Atan2(vTarget.y, vTarget.x) * Mathf.Rad2Deg);

        while (this.gameObject.activeSelf == true)
        {
            time += Time.deltaTime;

            if (time > (lifeTime / GameManager.Instance.GameSpeed))
            {
                ReturnProjectile();

                yield break;
            }

            if(GameManager.Instance.IsWaveWait)
            {
                ReturnProjectile();

                yield break;
            }

            this.transform.GetComponent<RectTransform>().anchoredPosition += (Vector2)vDir * MoveSpd * GameManager.Instance.GameSpeed * Time.deltaTime;

            int colliderCnt = Physics2D.OverlapCircleNonAlloc(this.transform.position, 1f, splashColls, layerMask);

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
                    else if (col.tag == ConstHelper.TAG_PROJECTILE)
                    {
                        continue;
                    }
                    else
                    {
                        CalculateProjectile(col.gameObject, 1f, () => 
                        {
                            if (ProjectileType != PROJECTILE_TYPE.PASS)
                            {
                                ReturnProjectile();
                            }
                        });
                        break;
                    }
                }
            }

            yield return null;
        }
    }

    public IEnumerator IECalculateProjectile()
    {
        while (this.gameObject.activeSelf == true)
        {
            int layerMask = 1 << LayerMask.NameToLayer(ConstHelper.LAYER_CHARACTER);
            int colliderCnt = Physics2D.OverlapCircleNonAlloc(this.transform.position, 1f, splashColls, layerMask);

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
                        CalculateProjectile(col.gameObject, 1f,
                            cb: () =>
                            {                                
                                isCollision = true;
                            });

                        if(isCollision == true)
                        {
                            if (ProjectileType != PROJECTILE_TYPE.PASS)
                            {
                                ReturnProjectile();
                            }

                            yield break;
                        }
                    }
                }
            }

            yield return null;
        }
    }

    private void CalculateProjectile(GameObject go, float distance, Action cb = null)
    {
        if (characterType == CHARACTER_TYPE.ENEMY || characterType == CHARACTER_TYPE.BOSS)
        {
            if (go.tag == ConstHelper.TAG_ENEMY)
            {
                return;
            }

            Vector3 targetDistance = go.transform.position - this.transform.position;

            if (targetDistance.magnitude < distance)
            {
                HeroCharacter hero = go.GetComponent<HeroCharacter>();

                if (hero != null)
                {
                    if (hero.TotalHP > 0)
                    {
                        if (hero.gameObject.activeSelf)
                        {
                            bool isCritical = IsCritical();
                            double multipleCriDam = isCritical == true ? TotalCriticalDamage / 100f : 1f;
                            EDamageHit dmgHitType = isCritical == true ? EDamageHit.Critical : EDamageHit.Hit;

                            hero.ActiveHitEffect(isCritical);
                            hero.CheckHpBar();
                            hero.CalculateHP(this, dmgHitType, multipleCriDam);
                        }

                        if (hero.TotalHP <= 0)
                        {
                            hero.ActiveDeadEffect(() =>
                            {
                                GameManager.Instance.SetGameOverAlram();
                            });
                        }
                    }
                    else
                    {
                        if (hero.gameObject.activeSelf)
                        {
                            hero.ActiveDeadEffect(() =>
                            {
                                GameManager.Instance.SetGameOverAlram();
                            });
                        }
                    }
                }                

                if (cb != null)
                {
                    cb.Invoke();
                }

                isAttack = false;
            }
        }
        else
        {
            if (go.tag == ConstHelper.TAG_PLAYER)
            {
                return;
            }

            Vector3 targetDistance = go.transform.position - this.transform.position;

            if (targetDistance.magnitude < distance)
            {
                EnemyCharacter enemy = go.GetComponent<EnemyCharacter>();

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

                            //임시
                            SoundManager.Instance.PlayFx(SoundManager.Instance.SoundDic["projecttile"]);
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

                if (cb != null)
                {
                    cb.Invoke();
                }

                isAttack = false;
            }
        }
    }

    public IEnumerator IEMove(Vector2 dir, Vector3 targetPos)
    {
        //if (characterAttType == CHARACTER_ATT_TYPE.SPLASH)
        //{
        //    yield return StartCoroutine(IEMoveInBezier(this.transform.position, targetPos, dir));
        //}
        //else
        //{
        //    yield return StartCoroutine(IEMoveProjectile(targetPos));
        //}

        StartCoroutine(IECalculateProjectile());

        if(this.gameObject.activeSelf == false)
        {
            yield break;
        }

        yield return StartCoroutine(IEMoveInBezier(this.transform.position, targetPos, dir));
    }   

    public void ReturnProjectile()
    {
        if (this.gameObject != null && this.gameObject.activeSelf == true)
        {
            pool.Release(this);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (characterAttType == CHARACTER_ATT_TYPE.SPLASH)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(originTargetPos, splashRadius);
        }
    }
#endif
}

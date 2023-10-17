using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Coffee.UIEffects;
using UnityEngine.Pool;

public class EnemyCharacter : BaseCharacter
{
    #region 드랍 코인
    [SerializeField]
    private Transform dropCoinRoot;
    [SerializeField]
    private GameObject dropCoinPrefab;
    [SerializeField]
    private RectTransform dropCoinEndPosTf;

    private IObjectPool<DropCoin> dropCoinPool;

    private const int MAX_DROPCOIN_COUNT = 5;
    #endregion

    private bool isInit = false;

    public override void FixedUpdate()
    {
        if (isInit == true)
        {
            animationTime += Time.deltaTime;

            if(IsCollision == false)
            {
                SetInputVec(Vector2.left);

                this.transform.position = Vector3.Lerp(this.transform.position,
                    this.transform.position + Vector3.left,
                    MoveSpd * GameManager.Instance.GameSpeed * Time.deltaTime);

                SetChangeState(AnimationType.Run);
            }

            base.FixedUpdate();          
        }
    }

    public void InitEnemy(CharacterData enemyData, int followIdx, float addMultiple = 0f)
    {
        InitStat(enemyData);

        if(addMultiple > 0f)
        {
            AddMultipleStat(addMultiple);
        }

        if (projectilePool == null)
        {
            if (AttCategory == ATT_CATEGORY.RANGE)
            {
                InitProjectilePool();
            }
        }

        if (dropCoinPool == null)
        {
            InitDropCoinPool();
        }

        InitCharacterInfo();

        this.gameObject.SetActive(true);

        SetChangeState(AnimationType.Run);

        this.transform.SetParent(GetFollowTarget(followIdx));
        this.transform.localPosition = Vector3.zero;

        hpGo.SetActive(false);

        //followTarget.SetFollowTarget(GetFollowTarget(followIdx));

        //followTarget.MoveFollowTarget();

        StartCoroutine(IECheckHero());        

        isInit = true;
    }

    private void InitCharacterInfo()
    {
        CharacterAnimation.SetAtlas(Character.CharacterAtlasName);

        fsmManager.SetFSM(AnimationType.Idle.ToString(), new StateIdle(this));
        fsmManager.SetFSM(AnimationType.Run.ToString(), new StateMove(this));
        fsmManager.SetFSM(AnimationType.Attack.ToString(), new StateAttack(this));
        fsmManager.SetFSM(AnimationType.Hit.ToString(), new StateHit(this));
    }

    private protected void InitDropCoinPool()
    {
        dropCoinPool = new ObjectPool<DropCoin>(
            createFunc: InitDropCoin,
            actionOnGet: GetDropCoin,
            actionOnRelease: ReleaseDropCoin,
            actionOnDestroy: DestoryDropCoin,
            defaultCapacity: MAX_DROPCOIN_COUNT,
            maxSize: MAX_DROPCOIN_COUNT
            );

    }

    private protected DropCoin InitDropCoin()
    {
        GameObject tmpCoin = AddressableManager.Instance.Instantiate(dropCoinPrefab.name, dropCoinRoot);
        tmpCoin.transform.position = Vector3.zero;

        DropCoin dropCoin = tmpCoin.GetComponent<DropCoin>();        

        dropCoin.SetDropCoin(dropCoinPool);

        return dropCoin;
    }

    private protected void GetDropCoin(DropCoin dropCoin)
    {
        Vector3 tmpPos = this.transform.position + PosOffset;

        //위지 보정 기준으로 캐릭터의 좌우, 중심~ 바닥에서 랜덤으로 코인 뿌림
        float xPos = Random.Range(tmpPos.x - 0.5f , tmpPos.x + 0.5f);
        float yPos = Random.Range(tmpPos.y - 0.5f, tmpPos.y);

        Vector3 startPos = new Vector3(xPos, yPos, tmpPos.z);
        dropCoin.SetPos(startPos, dropCoinEndPosTf);
        dropCoin.TweenDropCoin();
    }

    private protected void ReleaseDropCoin(DropCoin dropCoin)
    {
        dropCoin.gameObject.SetActive(false);
    }

    private protected void DestoryDropCoin(DropCoin dropCoin)
    {
        Destroy(dropCoin.gameObject);
    }

    public override void Reset()
    {
        this.transform.SetParent(GameManager.Instance.EnemyRoot);
        this.transform.localPosition = Vector3.zero;
        hpGo.SetActive(false);
        //followTarget.SetFollowTarget(null);

        base.Reset();

        this.gameObject.SetActive(false);
    }

    private IEnumerator IECheckHero()
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            if (IsCollision == true)
            {
                if (isAttack == false)
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

    private IEnumerator IEShoot()
    {
        if (AttCategory == ATT_CATEGORY.RANGE)
        {
            projectilePool.Get(out Projectile go);

            if (go != null)
            {
                if(go.gameObject.activeSelf == false)
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
                HeroCharacter hero = colGo.GetComponent<HeroCharacter>();

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
                            yield return StartCoroutine(IEActiveDeadEffect(
                                () => 
                                {
                                    GameManager.Instance.SetGameOverAlram();
                                }
                                ));
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
            }
        }

        yield return new WaitForSeconds(TotalAttackSpd);

        isAttack = false;
    }

    public void ReturnEnemy()
    {
        GameManager.Instance.AddKillEnemyCnt();

        Reset();
    }

    public void GetDropCoin()
    {
        NetworkManager.Instance.SendProtocol(NetworkManager.ESOCKET_PROTOCOL.REWARD, Character.DataCharacter.UID);
     
        StartCoroutine(IEGetDropCoin());
    }

    private IEnumerator IEGetDropCoin()
    {
        int rnd = Random.Range(2, MAX_DROPCOIN_COUNT + 1);

        for (int i = 0; i < rnd; i++)
        {
            dropCoinPool.Get();
            
            yield return new WaitForSeconds(0.1f / GameManager.Instance.GameSpeed);
        }
    }
}

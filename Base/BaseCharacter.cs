using Coffee.UIEffects;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

public class BaseCharacter : MonoBehaviour
{
    #region FSM
    public class StateIdle : FSM
    {
        private BaseCharacter character = null;
        private AnimationType type = AnimationType.Idle;

        public StateIdle(object target) : base(target)
        {
            character = target as BaseCharacter;
        }

        public override void Enter()
        {
            base.Enter();

            type = AnimationType.Idle;
            character.PlayAnimation(type);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }

    public class StateMove : FSM
    {
        private BaseCharacter character = null;
        private AnimationType type = AnimationType.Run;

        public StateMove(object target) : base(target)
        {
            character = target as BaseCharacter;
        }

        public override void Enter()
        {
            base.Enter();

            type = AnimationType.Run;
            character.PlayAnimation(type);
        }

        public override void Update()
        {
            base.Update();
            character.ActiveMoveEffect();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }
    public class StateAttack : FSM
    {
        private BaseCharacter character = null;
        private AnimationType type = AnimationType.Attack;

        public StateAttack(object target) : base(target)
        {
            character = target as BaseCharacter;
        }

        public override void Enter()
        {
            base.Enter();

            type = AnimationType.Attack;
            character.PlayAnimation(type);
            //임시
            character.PlayFx("melee");
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }

    public class StateHit : FSM
    {
        private BaseCharacter character = null;
        private AnimationType type = AnimationType.Hit;

        public StateHit(object target) : base(target)
        {
            character = target as BaseCharacter;
        }

        public override void Enter()
        {
            base.Enter();

            type = AnimationType.Hit;
            character.PlayAnimation(type);
        }

        public override void Update()
        {
            base.Update();
        }

        public override void Exit()
        {
            base.Exit();
        }
    }

    #endregion

    public enum EStat
    {
        ATK,
        HP,
        SPEED,
        CRIRATE,
        CRIDAM
    }

    public enum CHARACTER_TYPE
    {
        NONE = 0,
        HERO = 1,
        NPC = 2,

        ENEMY = 10,
        BOSS = 20,
    }

    public enum CHARACTER_ATT_TYPE
    {
        NONE = 0,
        NORMAL = 1,                 // 일반 공격 (단일)
        SPLASH = 2,                 // 범위 공격
    }

    public enum ATT_CATEGORY
    {
        NONE = 0,
        MELEE = 1,                  // 근거리
        RANGE = 2,                  // 원거리
    }

    public enum ATT_SHOOT_TYPE
    {
        NONE = 0,                  // 단발
        CONTINUE = 1,               // 연속
    }

    public enum PROJECTILE_TYPE
    {
        NONE = 0,
        PASS = 1,           //관통
        NONE_PASS = 2,      //비관통
    }

    public enum AnimationType
    {       
        Idle,
        Run,
        Attack,
        Hit,
        Dead,
    }

    public enum EDamageHit
    {
        Hit,
        Critical
    }

    [SerializeField]
    private CHARACTER_TYPE type;                // 캐릭터 타입
    [SerializeField]
    private CHARACTER_ATT_TYPE attType;         // 공격범위 타입
    [SerializeField]
    private ATT_CATEGORY attCategory;           // 공격사거리 타입
    [SerializeField]
    private ATT_SHOOT_TYPE attShootType;        // 공격 연사 타입

    [SerializeField]
    private PROJECTILE_TYPE projectileType = PROJECTILE_TYPE.NONE;  // 총알 관통 타입

    #region 영웅 스탯 관련
    [SerializeField]
    private double dam;              // 공격력
    [SerializeField]
    private double hp;               // 현재 체력
    [SerializeField]
    private double maxHp;            // 최대 체력
    [SerializeField]
    private float cri;              // 크리 확률
    [SerializeField]
    private double criDam;           // 크리 딜 배율
    [SerializeField]
    private float atkSpd;          // 공속
    [SerializeField]
    private float atkRange;         // 공격 범위
    [SerializeField]
    private float moveSpd;          // 이속
    [SerializeField]
    private float splashSize;       // 스플 범위 크기
    [SerializeField]
    private double supportDam;       // 서포트 일때 공격력
    [SerializeField]
    private double supportHp;        // 서포트 일때 체력
    [SerializeField]
    private double supportMaxHp;     // 서포트 일때 최대 체력
    #endregion

    #region 드랍 아이템 관련
    private BaseItemData dropItemData = null;
    #endregion

    #region 추가 스탯 관련
    [SerializeField]
    private double addStatDam;       // 스텟 추가 공격력
    [SerializeField]
    private double addStatHp;         // 스텟 추가 현재 체력
    [SerializeField]
    private double addStatMaxHp;     // 스텟 추가 최대 체력
    [SerializeField]
    private float addStatCri;        // 스텟 추가 크리 확률
    [SerializeField]
    private double addStatCriDam;    // 스텟 추가 크리 딜 배율
    #endregion

    #region 버프 스텟 관련
    [SerializeField]
    private double addBuffDam;       // 버프 추가 공격력
    [SerializeField]
    private double addBuffHp;        // 버프 추가 현재 체력
    [SerializeField]
    private double addBuffMaxHp;     // 버프 추가 최대 체력
    [SerializeField]
    private float addBuffCri;        // 버프 추가 크리 확률
    [SerializeField]
    private double addBuffCriDam;    // 버프 추가 크리 딜 배율
    [SerializeField]
    private float addBuffAtkSpd;     // 버프 추가 공속
    #endregion

    [SerializeField]
    private float gameSpd = 1f;          // 게임속도

    [SerializeField]
    private string characterAtlasName;        //캐릭터 아틀라스

    #region 캐릭 애니메이션
    [SerializeField]
    private Animator animator = null;

    [SerializeField]
    private SpriteRendererAnimation characterAnimation = null;

    [SerializeField]
    private protected FSMManager fsmManager = null;

    //현재 FSM 돌아가고 있는 애니메이션 시간
    private protected float currentAnimationTime = 0f;

    //애니메이션 체크용
    private protected float animationTime = 0f;

    private protected Coroutine corStateCheck = null;

    #endregion

    #region 체력 게이지
    [SerializeField]
    private protected GameObject hpGo = null;
    [SerializeField]
    private protected SlicedFilledImage imgHp = null;

    private float hpGaugeTime = 0f;
    #endregion

    #region 데미지 라벨
    [SerializeField]
    private protected TMP_Text[] labelDamage = null;
    #endregion

    #region 캐릭터 이펙트
    [SerializeField]
    private Transform imgCharacterTf = null;
    [SerializeField]
    private protected UtilPlayEffect effect;
    [SerializeField]
    private UIDissolve deadEffect = null;
    #endregion

    #region 투사체

    [SerializeField]
    private float projectileLifeTime;   // 투사체 생존시간

    [SerializeField]
    private Transform projectileRoot;
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform projectileStartPos;

    private string projectileImgName;

    private protected IObjectPool<Projectile> projectilePool;

    #endregion

    #region 영웅 포지션
    [SerializeField]
    private protected FollowTarget followTarget;

    [SerializeField]
    private RectTransform[] followTargets = null;

    private protected int followIdx = 0;
    #endregion

    private Vector2 inputVec;

    private protected Vector2 lastViewDir = Vector2.right;

    private CharacterData character = null;

    private Collider2D[] colls = new Collider2D[50];

    private protected string characterTag = "";

    private float dotValue = 0f;

    private protected Vector3 colTargetPos = Vector3.zero;
    private protected GameObject colGo = null;
    private Vector3 posOffset = Vector3.zero;

    private protected bool isDead = false;
    private protected bool isAttack = false;

    public CHARACTER_TYPE Type { get { return type; } }
    public CHARACTER_ATT_TYPE AttType { get { return attType; } }
    public ATT_CATEGORY AttCategory { get { return attCategory; } }
    public ATT_SHOOT_TYPE AttShootType { get { return attShootType; } }

    public PROJECTILE_TYPE ProjectileType { get { return projectileType; } }

    public double Damage { get { return dam + addStatDam; } }
    public double HP { get { return hp + addStatHp; } }
    public double MaxHP { get { return maxHp + addStatMaxHp; } }
    public float Critical { get { return cri + addStatCri; } }
    public double CriticalDamage { get { return criDam + addStatCriDam; } }

    public float OriginAtkSpd { get { return atkSpd; } }

    public double TotalDamage { get { return Damage + addBuffDam; } }
    public double TotalHP { get { return HP + addBuffHp; } }
    public double TotalMaxHP { get { return MaxHP + addBuffMaxHp; } }
    public float TotalCritical { get { return Critical + addBuffCri; } }
    public double TotalCriticalDamage { get { return CriticalDamage + addBuffCriDam; } }
    public float TotalAttackSpd { get { return Mathf.Clamp(OriginAtkSpd - addBuffAtkSpd, 0.5f, OriginAtkSpd) / gameSpd; } }

    public float AttackRange { get { return atkRange; } }
    public float MoveSpd { get { return moveSpd * gameSpd; } }
    public float SplashSize { get { return splashSize; } }

    public float ProjectileLifeTime { get { return projectileLifeTime; } }
    public double SupportDam { get { return supportDam; } }
    public double SupportHp { get { return supportHp; } }
    public double SupportMaxHp { get { return supportMaxHp; } }
    public BaseItemData DropItemData { get { return dropItemData; } }
    public string ProjectileImgName { get { return projectileImgName; } }

    public string CharacterAtlasName { get { return characterAtlasName; } }
    public SpriteRendererAnimation CharacterAnimation => characterAnimation;

    public CharacterData Character => character;

    public Vector2 InputVec => inputVec;
    public Vector3 PosOffset => posOffset;

    public bool IsCollision { get; private set; }

    public virtual void InitStat()
    {
        dam = 10;
        hp = 100;
        maxHp = 100;
        cri = 0;
        criDam = 100f;
        atkSpd = 1;
        moveSpd = 1;
        atkRange = 1f;
        splashSize = 0;
        projectileLifeTime = 1;
        gameSpd = 1f;
    }
    public virtual void InitStat(CharacterData character)
    {
        this.character = character;

        dam = character.Damage;
        hp = character.HP;
        maxHp = character.MaxHP;
        cri = character.Critical;
        criDam = UserData.Instance != null ? UserData.Instance.user.Config.BaseCriticalDam : 100f;

        atkSpd = character.AttackSpd;
        //atkRange = character.AttackRange;
        type = character.Type;

        if(type == CHARACTER_TYPE.HERO)
        {
            if(UserData.Instance.user.Character.GetHeroCharacter().DataCharacter.UID != character.DataCharacter.UID)
            {
                type = CHARACTER_TYPE.NPC;
            }
        }

        if (type == CHARACTER_TYPE.NPC)
        {
            atkRange = 3f;
        }
        else
        {
            atkRange = 1.5f;
        }

        moveSpd = character.MoveSpd;
        splashSize = character.SplashSize;
        projectileLifeTime = character.ProjectileLifeTime;
        projectileImgName = character.ProjectileImgName;
        gameSpd = 1f;
        attCategory = character.AttCategory;
        attType = character.AttType;
        attShootType = character.AttShootType;
        projectileType = character.ProjectileType;
        supportDam = character.SupportDam;
        supportHp = character.SupportHp;
        supportMaxHp = character.SupportMaxHp;

        characterAtlasName = character.CharacterAtlasName;

        characterTag = character.CharacterTag;

        foreach(EquipItemData equipmentData in character.EquipmentDic.Values.ToList())
        {
            if(equipmentData != null)
            {
                dam += equipmentData.GetEquipmentItemStat(character).atk;
                hp += equipmentData.GetEquipmentItemStat(character).hp;
                maxHp += equipmentData.GetEquipmentItemStat(character).hp;

                cri = Mathf.Clamp(cri + equipmentData.GetEquipmentItemStat(character).cri, cri + equipmentData.GetEquipmentItemStat(character).cri, 100f);
                atkSpd = Mathf.Clamp(atkSpd - equipmentData.GetEquipmentItemStat(character).atkSpd,
                    0.5f,
                    atkSpd - equipmentData.GetEquipmentItemStat(character).atkSpd);
            }
        }

        if (character.DropItemUID > 0)
        {
            DataItem tmpDropItem = DataManager.Instance.DataHelper.Item.Find(x => x.UID == character.DropItemUID);
            if (tmpDropItem != null)
            {
                dropItemData = new BaseItemData();

                dropItemData.SetDataItem(tmpDropItem);
                dropItemData.SetItemUID(tmpDropItem.UID);
                dropItemData.SetTitle(DataManager.Instance.GetLocalization(tmpDropItem.ITEM_NAME));
                dropItemData.SetDesc(DataManager.Instance.GetLocalization(tmpDropItem.ITEM_DESC));
                dropItemData.SetItemCategory(Enum.Parse<BaseItem.EItemCategory>(tmpDropItem.ITEM_CATEGORY));
                dropItemData.SetItemGrade(tmpDropItem.ITEM_GRADE);
                dropItemData.SetViewItemCount((int)character.DropItemCount);
                dropItemData.SetAtlasPath(tmpDropItem.ITEM_ICON_ATLAS);
                dropItemData.SetImgItemPath(tmpDropItem.ITEM_ICON);
            }
        }
    }
    public virtual void AddMultipleStat(float multiple)
    {
        dam *= multiple;
        hp *= multiple;
        maxHp *= multiple;
    }    

    /// <summary>
    /// 투사체, 스킬 딜
    /// </summary>
    /// <param name="dam"></param>
    public virtual void SetDam(double dam)
    {
        this.dam = dam;
    }
    /// <summary>
    /// 투사체, 스킬 크리
    /// </summary>
    /// <param name="cri"></param>
    public virtual void SetCri(float cri)
    {
        this.cri = cri;
    }
    /// <summary>
    /// 투사체, 스킬 크리 추딜
    /// </summary>
    /// <param name="criDam"></param>
    public virtual void SetCriDam(double criDam)
    {
        this.criDam = criDam;
    }

    public virtual void AddDam(double dam)
    {
        this.dam += dam;
    }
    public virtual void AddHp(double hp)
    {
        this.hp += hp;
        this.maxHp += hp;
    }
    public virtual void AddCri(float cri)
    {
        this.cri += cri;
    }
    public virtual void AddCriDam(double criDam)
    {
        this.criDam += criDam;
    }
    public virtual void AddStatDam(double addStatDam)
    {
        this.addStatDam = addStatDam;
    }
    public virtual void AddStatHp(double addStatHp)
    {
        this.addStatHp = addStatHp;
        this.addStatMaxHp = addStatHp;
    }
    public virtual void AddStatCri(float addStatCri)
    {
        this.addStatCri = addStatCri;
    }
    public virtual void AddStatCriDam(double addStatCriDam)
    {
        this.addStatCriDam = addStatCriDam;
    }
    public virtual void AddAtkSpd(float atkSpd)
    {
        this.atkSpd = Mathf.Clamp(this.atkSpd - atkSpd, 0.5f, this.atkSpd);
    }
    public virtual void AddAtkRange(float atkRange)
    {
        this.atkRange = atkRange;
    }
    public virtual void AddMoveSpd(float moveSpd)
    {
        this.moveSpd = moveSpd;
    }
    public virtual void AddSplashSize(float splashSize)
    {
        this.splashSize = splashSize;
    }  

    public virtual void AddBuffDam(double addBuffDam)
    {
        this.addBuffDam = addBuffDam;
    }

    public virtual void AddBuffHp(double addBuffHp)
    {
        this.addBuffHp = addBuffHp;
        this.addBuffMaxHp = addBuffHp;
    }

    public virtual void AddBuffCri(float addBuffCri)
    {
        this.addBuffCri = addBuffCri;
    }

    public virtual void AddBuffCriDam(double addBuffCriDam)
    {
        this.addBuffCriDam = addBuffCriDam;
    }
    public virtual void AddBuffAtkSpd(float addBuffAtkSpd)
    {
        this.addBuffAtkSpd = addBuffAtkSpd;
    }

    public virtual void Reset()
    {
        hp = maxHp;
        hpGaugeTime = 0f;
        if (imgHp != null)
        {
            imgHp.fillAmount = 1f;
        }

        if (hpGo != null)
        {
            hpGo.SetActive(false);
        }

        if (characterAnimation != null)
        {
            characterAnimation.ChangeColor("FFFFFF");
        }

        isDead = false;
        isAttack = false;
        SetSize(1f);
    }

    void OnEnable()
    {
        Messenger<float>.AddListener(ConstHelper.MessengerString.MSG_GAMESPEED, SetAddGameSpd);
    }

    void OnDisable()
    {
        Messenger<float>.RemoveListener(ConstHelper.MessengerString.MSG_GAMESPEED, SetAddGameSpd);
    }

    public virtual void FixedUpdate()
    {
        if (inputVec != Vector2.zero)
        {
            lastViewDir = GetDirection(inputVec.normalized);
        }

        Vector2 dir = lastViewDir.normalized;
        IsCollision = isCollision(out colTargetPos, out colGo);

        CharacterAnimation.FlipX(dir.x < -0.5f);

        hpGaugeTime += Time.deltaTime * gameSpd;

        if (hpGaugeTime <= 1f)
        {
            imgHp.fillAmount = Mathf.Lerp((float)(TotalHP / TotalMaxHP), imgHp.fillAmount, hpGaugeTime);
        }
        else
        {
            hpGaugeTime = 0f;
        }
    }

    public double CalculateHP(BaseCharacter attacker, EDamageHit hit, double multipleCriDam)
    {
        double dam = attacker.TotalDamage * multipleCriDam;

        if (UserData.Instance.user?.Config.BattleBaseDam >= dam)
        {
            dam = UserData.Instance.user.Config.BattleBaseDam;
        }

        ActiveDamageLabel(hit, dam);

        if (addBuffHp > 0)
        {
            addBuffHp -= dam;

            //추가 버프 체력에서 깎이면 추가 체력에서 나머지 뻄
            if (addBuffHp < 0)
            {                  
                // -라서 더해줌
                if (addStatHp > 0)
                {
                    addStatHp += addBuffHp;
                }
                else
                {
                    hp += addBuffHp;
                }

                addBuffHp = 0;
            }
        }

        else
        {
            if (addStatHp > 0)
            {
                addStatHp -= dam;

                //추가 체력에서 깎이면 원래 체력에서 나머지 뻄
                if (addStatHp < 0)
                {
                    hp += addStatHp;
                    addStatHp = 0;
                }
            }
            else
            {
                hp -= dam;
            }
        }

        return TotalHP;
    }

    public void ActiveDamageLabel(EDamageHit hit, double damage)
    {
        labelDamage[(int)hit].text = UserData.Instance.user?.Goods.GetPriceUnit(damage);
        labelDamage[(int)hit].transform.localRotation = Quaternion.Euler(0, -labelDamage[(int)hit].transform.parent.eulerAngles.y, 0);
        labelDamage[(int)hit].GetComponent<RectTransform>().anchoredPosition = new Vector2(0, characterAnimation.Sprite.textureRect.height);

        Sequence seq = DOTween.Sequence();
        seq.AppendCallback(() => { labelDamage[(int)hit].gameObject.SetActive(true); });
        seq.Append(labelDamage[(int)hit].transform.DOLocalMoveY(labelDamage[(int)hit].GetComponent<RectTransform>().anchoredPosition.y + 30f, 1f));
        seq.AppendCallback(() => { labelDamage[(int)hit].gameObject.SetActive(false); });
        seq.Play();
    }

    public void CheckHpBar()
    {
        SetChangeState(AnimationType.Hit);

        if (hpGo.activeSelf == false)
        {
            StartCoroutine(IECheckHPBar());
        }
    }

    public IEnumerator IECheckHPBar()
    {
        float time = 0f;

        hpGo.SetActive(true);

        while (hpGo.activeSelf)
        {
            time += Time.deltaTime * GameManager.Instance.GameSpeed;

            if (time >= 3f)
            {
                time = 0f;
                hpGo.SetActive(false);                
            }

            yield return null;
        }
    }

    public bool IsCritical()
    {
        float randomTotalRate = 100f;

        float pick = UnityEngine.Random.value * randomTotalRate;

        if (pick <= TotalCritical)
        {
            return true;
        }
        {
            return false;
        }
    }

    private protected bool isCurrentEqualState(AnimationType type)
    {
        return animator.GetCurrentAnimatorStateInfo(0).IsName(type.ToString());
    }

    private protected void SetChangeState(AnimationType type)
    {
        if(corStateCheck != null)
        {
            StopCoroutine(corStateCheck);
            corStateCheck = null;
        }

        bool isLoop = false;

        if(type == AnimationType.Idle || type == AnimationType.Run)
        {
            isLoop = true;
        }

        corStateCheck = StartCoroutine(IESetChangeState(type, isLoop));

        //스테이트가 바뀔때 체력바 위치, 콜라이더도 동기화
        if (characterAnimation.Sprite != null)
        {
            SyncHpBar();
            SyncCollider();
        }
    }

    private void SyncHpBar()
    {
        hpGo.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, characterAnimation.Sprite.textureRect.height);
    }

    private void SyncCollider()
    {
        this.GetComponent<CircleCollider2D>().radius = (CharacterAnimation.Sprite.textureRect.width / 2f) * imgCharacterTf.localScale.x;
        this.GetComponent<CircleCollider2D>().offset = new Vector2(0, this.GetComponent<CircleCollider2D>().radius);

        posOffset = new Vector3(0.5f - this.GetComponent<RectTransform>().pivot.x,
            0.5f - this.GetComponent<RectTransform>().pivot.y, 0);
    }

    private IEnumerator IESetChangeState(AnimationType type, bool isLoop)
    {
        while (true)
        {
            if (isCurrentEqualState(type) == false)
            {
                animationTime = 0;
                SetState(type.ToString());
            }
            else
            {
                if (isLoop == false)
                {
                    if (animationTime > currentAnimationTime)
                    {
                        animationTime = 0;
                        SetState(AnimationType.Idle.ToString());
                        yield break;
                    }
                }
            }

            yield return null;
        }
    }

    public void SetAddGameSpd(float gameSpd)
    {
        this.gameSpd = gameSpd;
    }

    public string CurrentStateName()
    {
        return fsmManager.CurrentStateName();
    }

    public void SetState(string key)
    {
        fsmManager.SetState(key);
    }

    public void PlayAnimation(AnimationType type)
    {
        if(isCurrentEqualState(type) == false)
        {
            animationTime = 0;
        }

        animator.SetTrigger(type.ToString());

        currentAnimationTime = animator.GetCurrentAnimatorStateInfo(0).length;
    }

    public void PlayFx(string key)
    {
        SoundManager.Instance.PlayFx(SoundManager.Instance.SoundDic[key]);
    }

    public void ActiveHitEffect(bool isCritical)
    {
        float speed = GameManager.Instance == null ? 0f : GameManager.Instance.GameSpeed;

        effect.PlayEffect(speed,
            isCritical == true ? EDamageHit.Critical.ToString() : EDamageHit.Hit.ToString(),
            isForce: true);
    }

    public void ActiveHitEffect(Vector3 location, bool isCritical)
    {
        effect.transform.position = location;
        ActiveHitEffect(isCritical);
    }

    public void ActiveMoveEffect()
    {
        float speed = GameManager.Instance == null ? 0f : GameManager.Instance.GameSpeed;

        effect.PlayEffect(speed, AnimationType.Run.ToString());
    }

    public void ActiveDeadEffect()
    {
        float speed = GameManager.Instance == null ? 0f : GameManager.Instance.GameSpeed;

        effect.PlayEffect(speed, AnimationType.Dead.ToString());
    }

    public void SetSize(float scale)
    {
        imgCharacterTf.localScale = new Vector3(scale, scale, scale);    
    }

    public void SetInputVec(Vector2 inputVec)
    {
        this.inputVec = inputVec;
    }

    private bool isCollision(out Vector3 targetPos, out GameObject colGo)
    {
        dotValue = Mathf.Cos(Mathf.Deg2Rad * 90f);

        Array.Clear(colls, 0, colls.Length);

        int layerMask = 1 << LayerMask.NameToLayer(ConstHelper.LAYER_CHARACTER);

        int hits = Physics2D.OverlapCircleNonAlloc(this.transform.position + posOffset, atkRange, colls, layerMask);

        if (hits > 0)
        {
            foreach (Collider2D col in colls)
            {
                if (col == null)
                {
                    continue;
                }

               //자신이랑 같은 타입이면 패스
                if (col.tag == characterTag)
                {
                    continue;
                }
                //총알 타입은 충돌체크 패스시킴
                else if (col.tag == ConstHelper.TAG_PROJECTILE)
                {
                    continue;
                }
                //NPC 제외
                else if (col.tag == ConstHelper.TAG_NPC)
                {
                    continue;
                }
                else
                {
                    if(GameManager.Instance == null)
                    {
                        targetPos = Vector3.zero;
                        colGo = null;


                        return false;
                    }

                    if (GameManager.Instance.IsWaveWait == false)
                    {
                        BaseCharacter tmpCharacter = col.transform.GetComponent<BaseCharacter>();
                        Vector3 tmpCharacterPos = tmpCharacter.transform.position + tmpCharacter.PosOffset;

                        Vector3 targetDistance = tmpCharacterPos - (this.transform.position + posOffset);

                        if (targetDistance.magnitude < atkRange)
                        {
                            if (Vector2.Dot(lastViewDir, (Vector2)targetDistance.normalized) > dotValue)
                            {
                                //targetPos = col.transform.position;
                                targetPos = tmpCharacterPos;
                                colGo = col.gameObject;

                                return true;
                            }
                            else
                            {
                                targetPos = Vector3.zero;
                                colGo = null;

                                return false;
                            }
                        }
                        else
                        {
                            targetPos = Vector3.zero;
                            colGo = null;

                            return false;
                        }
                    }
                    else
                    {
                        targetPos = Vector3.zero;
                        colGo = null;
                        return false;
                    }
                }
            }

            targetPos = Vector3.zero;
            colGo = null; 
            return false;
        }
        else
        {
            targetPos = Vector3.zero;
            colGo = null;
            return false;
        }
    }

    public Vector2 GetDirection(Vector2 vector)
    {
        Vector2 dir = Vector2.zero;

        if (vector.y > 0.5f)
        {
            dir = Vector2.up;
        }
        else if (vector.y < -0.5f)
        {
            dir = Vector2.down;
        }
        else if (vector.x < -0.5f)
        {
            dir = Vector2.left;
        }
        else if (vector.x > 0.5f)
        {
            dir = Vector2.right;
        }
        //else
        //{
        //    dir = Vector2.right;
        //}

        return dir;
    }

    public void ActiveDeadEffect(Action cb = null)
    {
        StartCoroutine(IEActiveDeadEffect(cb));
    }

    public IEnumerator IEActiveDeadEffect(Action cb = null)
    {
        if (isDead == false)
        {
            isDead = true;

            if (this.gameObject != null && this.gameObject.activeSelf == true)
            {
                ActiveDeadEffect();
                deadEffect.Play();

                yield return new WaitForSeconds(deadEffect.effectPlayer.duration);

                deadEffect.effectFactor = 0;
                deadEffect.Stop();
            }

            if(cb != null)
            {
                cb.Invoke();
            }
        }
    }

    #region 투사체 풀
    private protected void InitProjectilePool()
    {
        projectilePool = new ObjectPool<Projectile>(
            createFunc: InitProjectile,
            actionOnGet: GetProjectile,
            actionOnRelease: ReleaseProjectile,
            actionOnDestroy: DestoryProjectile,
            maxSize: 15
            );

    }

    private protected Projectile InitProjectile()
    {
        Projectile projectile = InitProjectile(ProjectileLifeTime);
        return projectile;
    }

    private protected Projectile InitProjectile(float lifeTime)
    {
        Projectile _projectile = AddressableManager.Instance.Instantiate(projectilePrefab.name, projectileRoot).GetComponent<Projectile>();
        _projectile.SetProjectile(projectilePool, lifeTime);
        _projectile.SetCharacterType(Type);
        _projectile.SetAttackType(AttType);
        _projectile.AddMoveSpd(2f);
        _projectile.SetSplashRadius(SplashSize);
        _projectile.gameObject.SetActive(false);

        return _projectile;
    }

    private protected void GetProjectile(Projectile projectile)
    {
        projectile.SetProjectileImg(projectileImgName);
        projectile.SetDam(TotalDamage);
        projectile.SetCri(TotalCritical);
        projectile.SetCriDam(TotalCriticalDamage);
        projectile.SetStartPosition(projectileStartPos);
        projectile.gameObject.SetActive(true);
    }

    private protected void ReleaseProjectile(Projectile projectile)
    {
        projectile.Reset();
        projectile.gameObject.SetActive(false);
    }

    private protected void DestoryProjectile(Projectile projectile)
    {
        Destroy(projectile.gameObject);
    }
    #endregion


    public RectTransform GetFollowTarget(int idx)
    {
        followIdx = idx - 1;
        return followTargets[followIdx];
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        if (isCollision(out colTargetPos, out colGo) == false)
        {
            Handles.color = Color.blue;
        }
        else
        {
            Handles.color = Color.red;
        }

        Vector3 pos = this.transform.position + posOffset;

        Handles.DrawSolidArc(pos, Vector3.forward, GetDirection(lastViewDir.normalized), -90f, atkRange);
        Handles.DrawSolidArc(pos, Vector3.forward, GetDirection(lastViewDir.normalized), 90f, atkRange);
    }

#endif
}

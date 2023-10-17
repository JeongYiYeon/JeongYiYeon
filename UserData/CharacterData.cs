using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData
{
    private DataCharacter datacharacter = null;

    private string name;
    private string desc;

    private float dam;              // 공격력
    private float hp;               // 현재 체력
    private float maxHp;            // 최대 체력
    private float cri;              // 크리 확률
    private float atkSpd;           // 공속
    private float atkRange;         // 공격 범위
    private float moveSpd;          // 이속
    private float splashSize;       // 스플 범위 크기
    private float projectileLifeTime;   // 투사체 생존시간
    private string projectileImgName;   // 투사체 이름
    private float supportDam;       // 서포트 일때 공격력
    private float supportHp;        // 서포트 일때 체력    
    private float supportMaxHp;     // 서포트 일때 최대 체력
    private int dropItemUID;        // 드랍 아이템 UID
    private double dropItemCount;      // 드랍 아이템 갯수

    private BaseCharacter.CHARACTER_TYPE type;                // 캐릭터 타입
    private BaseCharacter.CHARACTER_ATT_TYPE attType;         // 공격범위 타입
    private BaseCharacter.ATT_CATEGORY attCategory;           // 공격사거리 타입
    private BaseCharacter.ATT_SHOOT_TYPE attShootType;        // 공격 연사 타입
    private BaseCharacter.PROJECTILE_TYPE projectileType = BaseCharacter.PROJECTILE_TYPE.NONE;  // 총알 관통 타입

    private string characterAtlasName;        //캐릭터 아틀라스
    private string characterTag = "";

    private List<int> skillGroupList = new List<int>();

    private int upgradeCnt = 0;
    private int positionIdx = 0;
    private int tmpPositionIdx = 0;

    private Dictionary<EquipItemData.EEquipItemType, EquipItemData> equipmentDic = new Dictionary<EquipItemData.EEquipItemType, EquipItemData>()
    {
        { EquipItemData.EEquipItemType.WEAPON, null },
        { EquipItemData.EEquipItemType.HELM, null },
        { EquipItemData.EEquipItemType.GLOVE, null },
        { EquipItemData.EEquipItemType.ARMOR, null },
        { EquipItemData.EEquipItemType.ACCESSORY, null },
        { EquipItemData.EEquipItemType.BOOTS, null },
    };

    public DataCharacter DataCharacter => datacharacter;

    public string Name => name;
    public string Desc => desc;

    public float Damage { get { return dam; } }
    public float HP { get { return hp; } }
    public float MaxHP { get { return maxHp; } }
    public float Critical { get { return cri; } }
    public float AttackSpd { get { return atkSpd; } }
    public float AttackRange { get { return atkRange; } }
    public float MoveSpd { get { return moveSpd; } }
    public float SplashSize { get { return splashSize; } }
    public float ProjectileLifeTime { get { return projectileLifeTime; } }
    public string ProjectileImgName { get { return projectileImgName; } }
    public float SupportDam { get { return supportDam; } }
    public float SupportHp { get { return supportHp; } }
    public float SupportMaxHp { get { return supportMaxHp; } }

    public int DropItemUID { get { return dropItemUID; } }

    public double DropItemCount { get { return dropItemCount; } }

    public BaseCharacter.CHARACTER_TYPE Type { get { return type; } }
    public BaseCharacter.CHARACTER_ATT_TYPE AttType { get { return attType; } }
    public BaseCharacter.ATT_CATEGORY AttCategory { get { return attCategory; } }
    public BaseCharacter.ATT_SHOOT_TYPE AttShootType { get { return attShootType; } }
    public BaseCharacter.PROJECTILE_TYPE ProjectileType { get { return projectileType; } }
    public string CharacterAtlasName { get { return characterAtlasName; } }
    public string CharacterTag { get { return characterTag; } }

    public int UpgradeCnt => upgradeCnt;
    public int PositionIdx => positionIdx;
    public int TmpPositionIdx => tmpPositionIdx;                    // 임시 포지션 인덱스
    public Dictionary<EquipItemData.EEquipItemType, EquipItemData> EquipmentDic => equipmentDic;
    public List<int> SkillGroupList => skillGroupList;

    public void SetDataCharacter(DataCharacter datacharacter)
    {
        this.datacharacter = datacharacter;

        name = DataManager.Instance.GetLocalization(datacharacter.CHA_NAME);
        desc = DataManager.Instance.GetLocalization(datacharacter.CHA_DESC);

        dam = (float)datacharacter.BASE_ATTACK_DAM;
        hp = (float)datacharacter.BASE_HP;
        maxHp = (float)datacharacter.BASE_HP;
        cri = (float)datacharacter.BASE_CRIRATE;
        atkSpd = (float)datacharacter.BASE_ATTACK_SPEED;
        atkRange = 100f;
        moveSpd = (float)datacharacter.BASE_MOVE_SPEED;
        splashSize = 1f;
        projectileLifeTime = (float)datacharacter.PROJECTILE_LIFETIME;
        projectileImgName = datacharacter.PROJECTILE_FILE;
        supportDam = (float)datacharacter.SUPPORT_ATTACK_DAM;
        supportHp = (float)datacharacter.SUPPORT_HP;
        supportMaxHp = (float)datacharacter.SUPPORT_HP;
        dropItemUID = datacharacter.DROP_ITEM;
        dropItemCount = datacharacter.DROP_ITEM_COUNT;

        type = Enum.Parse<BaseCharacter.CHARACTER_TYPE>(datacharacter.CHA_TYPE);
        attCategory = Enum.Parse<BaseCharacter.ATT_CATEGORY>(datacharacter.CHA_ATTACK_CATEGORY);
        attType = Enum.Parse<BaseCharacter.CHARACTER_ATT_TYPE>(datacharacter.CHA_ATTACK_TYPE);
        attShootType = Enum.Parse<BaseCharacter.ATT_SHOOT_TYPE>(datacharacter.CHA_ATTACK_SHOOT);
        projectileType = Enum.Parse<BaseCharacter.PROJECTILE_TYPE>(datacharacter.PROJECTILE_PASS);
        characterAtlasName = datacharacter.CHA_FILE;

        skillGroupList.Clear();

        if (datacharacter.BASE_SKILL_LIST_1 > 0)
        {
            skillGroupList.Add(datacharacter.BASE_SKILL_LIST_1);
        }
        if (datacharacter.BASE_SKILL_LIST_2 > 0)
        {
            skillGroupList.Add(datacharacter.BASE_SKILL_LIST_2);
        }

        if (type == BaseCharacter.CHARACTER_TYPE.ENEMY || type == BaseCharacter.CHARACTER_TYPE.BOSS)
        {
            characterTag = ConstHelper.TAG_ENEMY;
        }
        else
        {
            characterTag = ConstHelper.TAG_PLAYER;
        }
    }

    public void SetUpgradeCnt(int upgradeCnt)
    {
        this.upgradeCnt = upgradeCnt;

        if(upgradeCnt > 0)
        {
            AddUpgradeStat(upgradeCnt);
        }
    }

    public void SetPositionIdx(int positionIdx)
    {
        this.positionIdx = positionIdx;
    }

    public void SetTempPositionIdx(int tmpPositionIdx)
    {
        this.tmpPositionIdx = tmpPositionIdx;
    }

    public void SetHeroEquipment(EquipItemData.EEquipItemType type, EquipItemData equipItemData)
    {
        equipmentDic[type] = equipItemData;
    }

    private void AddUpgradeStat(int upgradeCnt)
    {
        if(datacharacter == null)
        {
            return;
        }

        List<DataCharacterEnchant> tmpEnchantDataList = DataManager.Instance.DataHelper.CharacterEnchant.FindAll(x => x.CHARACTER_UID == datacharacter.UID);

        if(tmpEnchantDataList != null && tmpEnchantDataList.Count > 0)
        {
            float addAtk = 0;
            float addHp = 0;
            float addCri = 0;
            float addAtkSpd = 0;

            for (int i = 0; i < tmpEnchantDataList.Count; i++) 
            {
                if (tmpEnchantDataList[i].LATE_CHARACTER_LEVEL <= upgradeCnt)
                {
                    addAtk += (float)tmpEnchantDataList[i].ATTACK_DAM_UP;
                    addHp += (float)tmpEnchantDataList[i].HP_UP;
                    addCri += (float)tmpEnchantDataList[i].CRIRATE_UP;
                    addAtkSpd += (float)tmpEnchantDataList[i].ATTACK_SPEED_UP;
                }
            }

            dam = (float)datacharacter.BASE_ATTACK_DAM + addAtk;
            hp = (float)datacharacter.BASE_HP + addHp;
            maxHp = (float)datacharacter.BASE_HP + addHp;
            cri = Mathf.Clamp((float)datacharacter.BASE_CRIRATE + addCri, (float)datacharacter.BASE_CRIRATE + addCri, 100f);
            atkSpd = Mathf.Clamp((float)datacharacter.BASE_ATTACK_SPEED - addAtkSpd, 0.5f, (float)datacharacter.BASE_ATTACK_SPEED - addAtkSpd);
        }
    }
}

using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : Singleton<ConfigManager>
{
    //��ȭ �ִ� ����
    [JsonProperty("MONEY_MAX_JEWEL")]
    private double maxCash = 0;
    [JsonProperty("MONEY_MAX_GOLD")]
    private double maxGold = 0;
    [JsonProperty("MONEY_MAX_ENERGY")]
    private double maxEnergy = 0;

    //�ּ� ������
    [JsonProperty("BATTLE_BASE_DAMAGE")]
    private double battleBaseDam = 1;

    //�ּ� ũ��������
    [JsonProperty("SYSTEM_CRITICAL_DAM_BASE")]
    private double baseCriticalDam = 1;

    //��ų �� �ƽ���
    [JsonProperty("SYSTEM_SKILL_LEVEL_MAX")]
    private double skillLevelMax = 1;

    //ũ�� �ƽ���
    [JsonProperty("SYSTEM_CRITICAL_RATE_LIMITE")]
    private double criticalRateLimit = 1f;

    //���� ���� �ִ밪
    [JsonProperty("SYSTEM_LEVEL_STATUS_ATTACK_MAX")]
    private double statusAtkMax = 1;
    [JsonProperty("SYSTEM_LEVEL_STATUS_HP_MAX")]
    private double statusHpMax = 1;
    [JsonProperty("SYSTEM_LEVEL_STATUS_ATTACK_SPEED_MAX")]
    private double statusAtkSpdMax = 1;
    [JsonProperty("SYSTEM_LEVEL_STATUS_CRIRATE_MAX")]
    private double statusCriMax = 1;
    [JsonProperty("SYSTEM_LEVEL_STATUS_CRIDAM_MAX")]
    private double statusCriDamMax = 1;

    //����Ʈ �ɷ�ġ ���� ����
    [JsonProperty("SYSTEM_LEVEL_STATUS_SUPPORT_RATE")]
    private double statusSupportRate = 1f;

    //ĳ���� Ÿ�� �� ���� ����
    [JsonProperty("CHA_HERO_LEVEL_MAX")]
    private double heroLevelMax = 1;

    //������ �ִ� ��ȭ ����
    [JsonProperty("SYSTEM_ITEM_LEVEL_MAX")]
    private double itemLevelMax = 1;

    //�κ��丮 �ƽ� ��
    [JsonProperty("SYSTEM_INVEN_MAX")]
    private double maxInvenItemCount = 1;

    //�����н�
    [JsonProperty("SYSTEM_GAMEPASS_NOW_SEASON")]
    private double gamePassSeason = 1;
    [JsonProperty("SYSTEM_GAMEPASS_LEVEL_MAX")]
    private double gamePassLvMax = 1;
    //�����н� ���� �Ұ��� �ð� ( ���� �н� ������ - �ش� ��)
    [JsonProperty("SYSTEM_GAMEPASS_BUY_LIMITE")]
    private double buyLimiteGamePass = 1f;

    //���� �ڵ� ���� �ð� (��)
    [JsonProperty("SYSTEM_MAIL_DELETE_TIME")]
    private double deleteMailDay = 1;

    //2��� ���� �ð� (��)
    [JsonProperty("SYSTEM_2XSPEED_LIMIT_TIME")]
    private double gameSpeedLimitTime = 1;

    //������ġ ��
    [JsonProperty("SYSTEM_HERO_ARANGE_FIX_GRID")]
    private double heroPos = 0;

    private Dictionary<string, double> statusGoldDic = new Dictionary<string, double>();
    private Dictionary<string, double> statusValueDic = new Dictionary<string, double>();

    public double MaxCash => maxCash;
    public double MaxGold => maxGold;
    public int MaxEnergy => (int)maxEnergy;
    public double BattleBaseDam => battleBaseDam;
    public double BaseCriticalDam => baseCriticalDam;

    public int SkillLevelMax => (int)skillLevelMax;
    public float CriticalRateLimit => (float)criticalRateLimit;

    public int StatusAtkMax => (int)statusAtkMax;
    public int StatusHpMax => (int)statusHpMax;
    public int StatusAtkSpdMax => (int)statusAtkSpdMax;
    public int StatusCriMax => (int)statusCriMax;
    public int StatusCriDamMax => (int)statusCriDamMax;

    public float StatusSupportRate => (float)statusSupportRate;

    public int HeroLevelMax => (int)heroLevelMax;
    public int ItemLevelMax => (int)itemLevelMax;


    public int GamePassSeason => (int)gamePassSeason;

    public int GamePassLvMax => (int)gamePassLvMax;

    public int MaxInvenItemCount => (int)maxInvenItemCount;

    public float BuyLimiteGamePass => (int)buyLimiteGamePass;

    public int DeleteMailDay => (int)deleteMailDay;
    public float GameSpeedLimitTime => (float)gameSpeedLimitTime;

    public int HeroPos => (int)heroPos;

    public Dictionary<string, double> StatusGoldDic => statusGoldDic;
    public Dictionary<string, double> StatusValueDic => statusValueDic;
    public void SetStatusGoldDic(Dictionary<string, double> statusGoldDic)
    {
        this.statusGoldDic = statusGoldDic;
    }

    public void SetStatusValueDic(Dictionary<string, double> statusValueDic)
    {
        this.statusValueDic = statusValueDic;
    }
}

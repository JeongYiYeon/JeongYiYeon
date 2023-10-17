using Coffee.UIExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class StatItem : MonoBehaviour
{
    [SerializeField]
    private AtlasImage imgStat = null;
    [SerializeField]
    private TMP_Text labelStatLevel = null;

    [SerializeField]
    private TMP_Text labelStatName = null;
    [SerializeField]
    private TMP_Text labelStatValue = null;

    [SerializeField]
    private TMP_Text labelUpgradeCost = null;

    [SerializeField]
    private Transform btStatupTf = null;

    [SerializeField]
    private UtilEnumSelect statEnum = null;

    [SerializeField]
    private UIParticle statUpEffect = null;

    private bool isClick = false;

    private int statLv = 0;
    private int nextStatLv { get { return statLv + 1; } }

    private double statValue = 0;
    private double nextStatValue = 0;

    private double statCost = 0;

    private float time = 0f;
    private const float intervalTime = 0.1f;

    void OnEnable()
    {
        Messenger<BaseCharacter>.AddListener(ConstHelper.MessengerString.MSG_STATUP_RESET, RefreshStatItem);
        Messenger.AddListener(ConstHelper.MessengerString.MSG_STATUP, StatUp);
    }

    void OnDisable()
    {
        Messenger<BaseCharacter>.RemoveListener(ConstHelper.MessengerString.MSG_STATUP_RESET, RefreshStatItem);
        Messenger.RemoveListener(ConstHelper.MessengerString.MSG_STATUP, StatUp);
    }

    private void Awake()
    {
        string statName = "";

        RefreshStatLv();

        switch (statEnum.StatType)
        {
            case BaseCharacter.EStat.ATK:
                statName = "공격력";
                statValue = GetStatValue(BaseCharacter.EStat.ATK.ToString(), statLv);
                nextStatValue = GetStatValue(BaseCharacter.EStat.ATK.ToString(), nextStatLv);
                statCost = GetGoldCostValue(BaseCharacter.EStat.ATK.ToString());
                break;

            case BaseCharacter.EStat.HP:
                statName = "체력";
                statValue = GetStatValue(BaseCharacter.EStat.HP.ToString(), statLv);
                nextStatValue = GetStatValue(BaseCharacter.EStat.HP.ToString(), nextStatLv);
                statCost = GetGoldCostValue(BaseCharacter.EStat.HP.ToString());
                break;

            case BaseCharacter.EStat.SPEED:
                statName = "공격속도";
                KeyValuePair<string, double> tmpSpdStat = UserData.Instance.user.Config.StatusValueDic.FirstOrDefault(x => x.Key.Contains("SPEED"));
                statValue = tmpSpdStat.Value * statLv;
                nextStatValue = tmpSpdStat.Value * nextStatLv;
                statCost = GetGoldCostValue(BaseCharacter.EStat.SPEED.ToString());
                break;

            case BaseCharacter.EStat.CRIRATE:
                statName = "치명타 확률";
                KeyValuePair<string, double> tmpCriStat = UserData.Instance.user.Config.StatusValueDic.FirstOrDefault(x => x.Key.Contains("CRIRATE"));
                statValue = tmpCriStat.Value * statLv;
                nextStatValue = tmpCriStat.Value * nextStatLv;
                statCost = GetGoldCostValue(BaseCharacter.EStat.CRIRATE.ToString());

                break;
            case BaseCharacter.EStat.CRIDAM:
                statName = "치명타 데미지";
                KeyValuePair<string, double> tmpCriDamStat = UserData.Instance.user.Config.StatusValueDic.FirstOrDefault(x => x.Key.Contains("CRIDAM"));
                statValue = tmpCriDamStat.Value * statLv;
                nextStatValue = UserData.Instance.user.Config.BaseCriticalDam + (tmpCriDamStat.Value * nextStatLv);
                statCost = GetGoldCostValue(BaseCharacter.EStat.CRIDAM.ToString());

                break;
        }

        labelStatName.text = statName;
        RefreshLabel();
    }

    private void RefreshStatLv()
    {
        switch (statEnum.StatType)
        {
            case BaseCharacter.EStat.ATK:
                statLv = UserData.Instance.user.Stat.AtkLv;
                break;
            case BaseCharacter.EStat.HP:
                statLv = UserData.Instance.user.Stat.HpLv;
                break;
            case BaseCharacter.EStat.SPEED:
                statLv = UserData.Instance.user.Stat.SpeedLv;
                break;
            case BaseCharacter.EStat.CRIRATE:
                statLv = UserData.Instance.user.Stat.CrirateLv;                
                break;
            case BaseCharacter.EStat.CRIDAM:              
                statLv = UserData.Instance.user.Stat.CriDamLv;
                break;
        }
    }

    private void RefreshLabel()
    {
        string tmpUnit = "";

        if (statEnum.StatType == BaseCharacter.EStat.CRIRATE || statEnum.StatType == BaseCharacter.EStat.CRIDAM)
        {
            tmpUnit = "%";
        }

        labelStatLevel.text = $"Lv.{nextStatLv}";
        labelStatValue.text = UserData.Instance.user.Goods.GetPriceUnit(nextStatValue) + tmpUnit;
        labelUpgradeCost.text = UserData.Instance.user.Goods.GetPriceUnit(statCost);

        Messenger.Broadcast(ConstHelper.MessengerString.MSG_TOTAL_COMBAT_POWER, MessengerMode.DONT_REQUIRE_LISTENER);
    }

    private void FixedUpdate()
    {
        if(isClick == true)
        {
            time += Time.deltaTime;

            if (time > intervalTime)
            {
                StatUp();
                time = 0f;
            }
        }
    }

    private void RefreshStatItem(BaseCharacter character)
    {
        RefreshStatLv();

        switch (statEnum.StatType)
        {
            case BaseCharacter.EStat.ATK:
                if (UserData.Instance.user.Config.StatusAtkMax > statLv)
                {
                    statValue = GetStatValue(BaseCharacter.EStat.ATK.ToString(), statLv);
                    nextStatValue = GetStatValue(BaseCharacter.EStat.ATK.ToString(), nextStatLv);
                    statCost = GetGoldCostValue(BaseCharacter.EStat.ATK.ToString());

                    character.AddStatDam(statValue);
                }
                break;
            case BaseCharacter.EStat.HP:
                if (UserData.Instance.user.Config.StatusHpMax > statLv)
                {
                    statValue = GetStatValue(BaseCharacter.EStat.HP.ToString(), statLv);
                    nextStatValue = GetStatValue(BaseCharacter.EStat.HP.ToString(), nextStatLv);
                    statCost = GetGoldCostValue(BaseCharacter.EStat.HP.ToString());

                    //체력 증가 한거 다시 동기화
                    character.AddStatHp(statValue);
                }
                break;
            case BaseCharacter.EStat.SPEED:
                if (UserData.Instance.user.Config.StatusAtkSpdMax > statLv)
                {
                    KeyValuePair<string, double> tmpStat = UserData.Instance.user.Config.StatusValueDic.FirstOrDefault(x => x.Key.Contains("SPEED"));

                    statValue = tmpStat.Value * statLv;
                    nextStatValue = tmpStat.Value * nextStatLv;

                    character.AddAtkSpd((float)tmpStat.Value);

                    statCost = GetGoldCostValue(BaseCharacter.EStat.SPEED.ToString());
                }
                break;

            case BaseCharacter.EStat.CRIRATE:
                if (UserData.Instance.user.Config.CriticalRateLimit > statValue &&
                    UserData.Instance.user.Config.StatusCriMax > statLv)
                {
                    KeyValuePair<string, double> tmpStat = UserData.Instance.user.Config.StatusValueDic.FirstOrDefault(x => x.Key.Contains("CRIRATE"));

                    statValue = tmpStat.Value * statLv;
                    nextStatValue = tmpStat.Value * nextStatLv;

                    character.AddStatCri((float)statValue);

                    statCost = GetGoldCostValue(BaseCharacter.EStat.CRIRATE.ToString());

                }
                break;

            case BaseCharacter.EStat.CRIDAM:

                if (UserData.Instance.user.Config.StatusCriDamMax > statLv)
                {
                    KeyValuePair<string, double> tmpStat = UserData.Instance.user.Config.StatusValueDic.FirstOrDefault(x => x.Key.Contains("CRIDAM"));

                    statValue = tmpStat.Value * statLv;
                    nextStatValue = UserData.Instance.user.Config.BaseCriticalDam + (tmpStat.Value * nextStatLv);

                    character.AddStatCriDam((float)statValue);

                    statCost = GetGoldCostValue(BaseCharacter.EStat.CRIDAM.ToString());
                }
                break;
        }

        RefreshLabel();
    }

    private void StatUp()
    {
        if(statEnum == null)
        {
            return;
        }

        HeroCharacter character = GameManager.Instance.HeroCharacter;

        double tmpStatCost = GetGoldCostValue(statEnum.StatType.ToString());

        if (UserData.Instance.IsEnoughGoods(UserData.EGoodsType.GOLD, tmpStatCost) == false)
        {
            string tmpGold = UserData.Instance.user.Goods.GetPriceUnit(tmpStatCost - UserData.Instance.user.Goods.Gold);
            LoadingManager.Instance.ActiveOneLineAlram($"{tmpGold} 골드가 부족 합니다.");
        }
        else
        {
            NetworkManager.Instance.SendProtocol(NetworkManager.ESOCKET_PROTOCOL.STAT_UP, statEnum.StatType.ToString());

            PlayStatUpEffect();
            StatUpTween();
        }
    }

    private double GetStatValue(string stat, int statLv)
    {
        // 기본증가값 + ((레벨 * 레벨 증가값) * 레벨 구간 가중치)
        KeyValuePair<string, double> tmpStatusBase = GetStatusValueData($"{stat}_VALUE_BASE", statLv);
        KeyValuePair<string, double> tmpStatusLv = GetStatusValueData($"{stat}_VALUE_LEVELUP", statLv);
        KeyValuePair<string, double> tmpStatusFactor = GetStatusValueData($"{stat}_VALUE_LEVELFACTOR", statLv);

        return tmpStatusBase.Value + ((statLv * tmpStatusLv.Value) * tmpStatusFactor.Value);
    }

    private KeyValuePair<string, double> GetStatusValueData(string key, int statLv)
    {
        foreach (KeyValuePair<string, double> item in UserData.Instance.user.Config.StatusValueDic.Where(x => x.Key.Contains(key)))
        {
            string[] tmpValueEnum = item.Key.Split("_");

            int tmpMin = int.Parse(tmpValueEnum[tmpValueEnum.Length - 2]);
            int tmpMax = int.Parse(tmpValueEnum[tmpValueEnum.Length - 1]);

            if (tmpMin <= statLv && tmpMax >= statLv)
            {
                return item;                
            }
        }

        return new KeyValuePair<string, double>();
    }

    private double GetGoldCostValue(string stat)
    {
        // 기본증가값 + ((레벨 * 레벨 증가값) * 레벨 구간 가중치)
        KeyValuePair<string, double> tmpGoldBase = GetGoldCostData($"{stat}_GOLD_BASE");
        KeyValuePair<string, double> tmpGoldLv = GetGoldCostData($"{stat}_GOLD_LEVELUP");
        KeyValuePair<string, double> tmpGoldFactor = GetGoldCostData($"{stat}_GOLD_LEVELFACTOR");

        return tmpGoldBase.Value + ((nextStatLv * tmpGoldLv.Value) * tmpGoldFactor.Value);
    }
    private KeyValuePair<string, double> GetGoldCostData(string key)
    {
        foreach (KeyValuePair<string, double> item in UserData.Instance.user.Config.StatusGoldDic.Where(x => x.Key.Contains(key)))
        {
            string[] tmpValueEnum = item.Key.Split("_");

            int tmpMin = int.Parse(tmpValueEnum[tmpValueEnum.Length - 2]);
            int tmpMax = int.Parse(tmpValueEnum[tmpValueEnum.Length - 1]);

            if (tmpMin <= nextStatLv && tmpMax >= nextStatLv)
            {
                return item;
            }
        }

        return new KeyValuePair<string, double>();
    }

    private void PlayStatUpEffect()
    {
        statUpEffect.Play();
    }

    private void StatUpTween()
    {
        Sequence seq = DOTween.Sequence();

        seq.Append(imgStat.transform.DOScale(0.95f, 0.05f));
        seq.Join(labelStatValue.transform.DOScale(0.95f, 0.05f));
        seq.Join(btStatupTf.DOScale(0.95f, 0.05f));

        seq.Append(imgStat.transform.DOScale(1.05f, 0.1f));
        seq.Join(labelStatValue.transform.DOScale(2.5f, 0.1f));
        seq.Join(btStatupTf.DOScale(1.05f, 0.1f));

        seq.Append(imgStat.transform.DOScale(1f, 0.1f));
        seq.Join(labelStatValue.transform.DOScale(1f, 0.1f));
        seq.Join(btStatupTf.DOScale(1f, 0.1f));

        seq.Play();
    }

    public void OnClickStatUp()
    {
        StatUp();
    }

    public void OnClickDown()
    {
        isClick = true;
    }

    public void OnClickUp()
    {
        isClick = false;
        time = 0f;
    }
}

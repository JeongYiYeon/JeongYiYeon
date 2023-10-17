using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdBuffPopup : BasePopup
{
    private enum EAdBuffType
    {
        Attack,
        Support,
    }

    [SerializeField]
    private AdBuffItem[] adBuffItems = null;

    private void OnEnable()
    {
        SetType(EPopupType.AdBuff);
    }

    public override void Init()
    {
        base.Init();

        //고정임
        adBuffItems[(int)EAdBuffType.Attack].SetBuffList(SkillManager.Instance.SkillDic[1]);
        adBuffItems[(int)EAdBuffType.Attack].SetBuffList(SkillManager.Instance.SkillDic[4]);

        adBuffItems[(int)EAdBuffType.Support].SetBuffList(SkillManager.Instance.SkillDic[2]);
        adBuffItems[(int)EAdBuffType.Support].SetBuffList(SkillManager.Instance.SkillDic[3]);

        adBuffItems[(int)EAdBuffType.Attack].SetConfirmCallback(() =>
        {
            AdmobManager.Instance.OnClickShowRewardAD(async() =>
            {
                await UniTask.Delay(200);

                adBuffItems[(int)EAdBuffType.Attack].SetRemainBuffTime(SkillManager.Instance.SkillDic[1][0].Time);
                adBuffItems[(int)EAdBuffType.Attack].RefreshState();
                adBuffItems[(int)EAdBuffType.Attack].ActiveBuff();
            });
        });

        adBuffItems[(int)EAdBuffType.Support].SetConfirmCallback(() =>
        {
            AdmobManager.Instance.OnClickShowRewardAD(async () =>
            {
                await UniTask.Delay(200);

                adBuffItems[(int)EAdBuffType.Support].SetRemainBuffTime(SkillManager.Instance.SkillDic[2][0].Time);
                adBuffItems[(int)EAdBuffType.Support].RefreshState();
                adBuffItems[(int)EAdBuffType.Support].ActiveBuff();
            });
        });

        adBuffItems[(int)EAdBuffType.Attack].InitAdBuffItem();
        adBuffItems[(int)EAdBuffType.Support].InitAdBuffItem();
    }

    public override void Active()
    {
        base.Active();
    }
}

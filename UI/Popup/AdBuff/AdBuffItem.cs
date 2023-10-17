using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Coffee.UIExtensions;

public class AdBuffItem : MonoBehaviour
{
    private enum EBuff
    {
        BuffOff,
        BuffOn,
    }    

    [SerializeField]
    private TMP_Text labelBuffDesc = null;
    [SerializeField]
    private TMP_Text labelBuffTime = null;

    [SerializeField]
    private UtilState state = null;
    [SerializeField]
    private TMP_Text labelBuffRemainTime = null;

    //버프시간 게임매니저로 옮겨야됨
    private float remainBuffTime = 0f;
    private float time = 0f;
    private Action onConfirmCallback;

    private List<SkillData> buffList = new List<SkillData>();


    private void FixedUpdate()
    {
        if(remainBuffTime - time < 0)
        {
            time = 0f;
            remainBuffTime = 0f;
            Reset();
        }

        time += Time.deltaTime * GameManager.Instance.GameSpeed;

        SetBuffRemainTime(time);
    }

    private void Reset()
    {
        for(int i = 0; i < buffList.Count; i++) 
        {
            for(int j = 0; j < buffList[i].SkillOptions.Count; j++)
            {
                switch(buffList[i].SkillOptions[j].Type)
                {
                    case BaseOptionData.EOptionType.ATTACK_DAM:
                        break;
                    case BaseOptionData.EOptionType.SKILL_COOLTIME_DECR:
                        break;
                    case BaseOptionData.EOptionType.GET_GOLD:
                        GameManager.Instance.SetMultipleDropcoin(1f);
                        break;
                    case BaseOptionData.EOptionType.HP_UP:
                        break;
                }
            }
        }
    }

    #region Set   

    public void SetBuffList(List<SkillData> data)
    {        
        for(int i = 0; i < data.Count; i++) 
        {
            if (buffList.Exists(x => x == data[i]) == false)
            {
                buffList.Add(data[i]);
            }
        }
    }

    public void SetRemainBuffTime(float remainBuffTime)
    {
        this.remainBuffTime = remainBuffTime;
    }

    public void InitAdBuffItem()
    {
        labelBuffDesc.text = $"{buffList[0].Title}\n{buffList[1].Title}";

        int min = (int)(buffList[0].Time / 60f);

        labelBuffTime.text = $"적용시간 {min}분";

        RefreshState();
    }

    public void RefreshState()
    {
        if (remainBuffTime > 0f)
        {
            state.ActiveState(EBuff.BuffOn.ToString());
        }
        else
        {
            state.ActiveState(EBuff.BuffOff.ToString());
        }

    }

    public void SetBuffRemainTime(float time)
    {
        int min = (int)((remainBuffTime - time) / 60f);
        int second = (int)((remainBuffTime - time) - (min * 60f));

        labelBuffRemainTime.text = string.Format("{0:00} 분 {1:00} 초 남음", min, second);
    }

    public void ActiveBuff()
    {
        for (int i = 0; i < buffList.Count; i++)
        {
            SkillManager.Instance.CacluateStat(buffList[i], buffList[i].SkillOptions[0]);
        }
    }

    public void SetConfirmCallback(Action cb)
    {
        if (cb != null)
        {
            onConfirmCallback = cb;
        }
    }

    #endregion

    #region OnClick
    public void OnClickConfirm()
    {
        if (onConfirmCallback != null)
        {
            onConfirmCallback.Invoke();
        }
    }
    #endregion

}

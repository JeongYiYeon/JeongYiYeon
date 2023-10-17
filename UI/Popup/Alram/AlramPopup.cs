using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AlramPopup : BasePopup
{
    [SerializeField]
    private TMP_Text labelTitle = null;
    [SerializeField]
    private TMP_Text labelDesc = null;

    [SerializeField]
    private TMP_Text[] labelConfirmBt = null;
    [SerializeField]
    private TMP_Text labelCancelBt = null;

    [SerializeField]
    private UtilState state = null;

    private void OnEnable()
    {
        SetType(EPopupType.Alram);
    }

    public override void Init()
    {
        base.Init();
    }

    public override void ResetLabel()
    {
        base.ResetLabel(); 

        if (labelTitle != null)
        {
            labelTitle.text = "팝업";
        }

        if (labelDesc != null)
        {
            labelDesc.text = "설명";
        }

        if (labelConfirmBt != null && labelConfirmBt.Length > 0)
        {
            for (int i = 0; i < labelConfirmBt.Length; i++)
            {
                labelConfirmBt[i].text = "확인";
            }
        }

        if (labelCancelBt != null)
        {
            labelCancelBt.text = "취소";
        }
    }

    public override void Active()
    {        
        base.Active();

        labelTitle.text = title;
        labelDesc.text = desc;
        labelConfirmBt[(int)btType].text = confirmBtTitle;
        labelCancelBt.text = cancelBtTitle;

        if (state != null)
        {
            state.ActiveState(btType.ToString());
        }
    }

    public override void OnClickConfirm()
    {
        base.OnClickConfirm();
    }

    public override void OnClickCancel()
    {
        base.OnClickCancel();
    }
}

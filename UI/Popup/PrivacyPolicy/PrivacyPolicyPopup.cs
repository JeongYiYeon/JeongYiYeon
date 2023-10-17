using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrivacyPolicyPopup : BasePopup
{
    private enum PolicyType
    {
        TermsofUse,
        PrivacyPolicy
    }

    [SerializeField]
    private Policy[] policys = null;

    private void OnEnable()
    {
        SetType(EPopupType.PrivacyPolicy);

        StartCoroutine(IECheckAgree());
    }

    public override void Init()
    {
        base.Init();
        //policys[(int)PolicyType.TermsofUse].SetPolicyDesc();
        //policys[(int)PolicyType.PrivacyPolicy].SetPolicyDesc();
    }

    private IEnumerator IECheckAgree()
    {
        while (true)
        {
            if(policys[(int)PolicyType.TermsofUse].IsCheck && 
               policys[(int)PolicyType.PrivacyPolicy].IsCheck)
            {
                OnClickConfirm();
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
    }
}

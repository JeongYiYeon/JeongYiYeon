using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CsSetting : MonoBehaviour
{
    [SerializeField]
    private Policy policy = null;
    [SerializeField]
    private CustomerSupport cs = null;
    public bool isActivePolicy => policy.gameObject.activeSelf;
    public bool isCustomerSupport => cs.gameObject.activeSelf;

    private void OnDisable()
    {
        DeActivePolicy();
        DeActiveCS();
    }

    public void DeActiveCS()
    {
        cs.gameObject.SetActive(false);
    }

    public void DeActivePolicy()
    {
        policy.gameObject.SetActive(false);
    }

    private void SetTermOfUse()
    {
        policy.SetPolicyTitle("이용 약관");
        policy.SetPolicyDesc("이용 약관 설명이 적혀있습니다.");
    }

    private void SetPrivacyPolicy()
    {
        policy.SetPolicyTitle("개인 정보 처리 방침");
        policy.SetPolicyDesc("개인 정보 처리 방침 설명이 적혀있습니다.");
    }

    private void SetCancellationPolicy()
    {
        policy.SetPolicyTitle("청약 철회");
        policy.SetPolicyDesc("청약철회 설명이 적혀있습니다.");
    }

    #region OnClick
    public void OnClickPolicy(UtilEnumSelect category)
    {
        switch (category.PolicyType)
        {
            case EnumSetting.EPolicyType.TermOfUse:
                SetTermOfUse();
                break;
            case EnumSetting.EPolicyType.PrivacyPolicy:
                SetPrivacyPolicy();
                break;
            case EnumSetting.EPolicyType.CancellationPolicy:
                SetCancellationPolicy();
                break;
        }

        policy.gameObject.SetActive(true);
    }

    public void OnClickContact()
    {
        cs.gameObject.SetActive(true);
    }
    #endregion

}

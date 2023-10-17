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
        policy.SetPolicyTitle("�̿� ���");
        policy.SetPolicyDesc("�̿� ��� ������ �����ֽ��ϴ�.");
    }

    private void SetPrivacyPolicy()
    {
        policy.SetPolicyTitle("���� ���� ó�� ��ħ");
        policy.SetPolicyDesc("���� ���� ó�� ��ħ ������ �����ֽ��ϴ�.");
    }

    private void SetCancellationPolicy()
    {
        policy.SetPolicyTitle("û�� öȸ");
        policy.SetPolicyDesc("û��öȸ ������ �����ֽ��ϴ�.");
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

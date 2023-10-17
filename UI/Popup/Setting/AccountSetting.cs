using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class AccountSetting : MonoBehaviour
{
    [SerializeField]
    private TMP_Text labelAppleLoginLink = null;
    [SerializeField]
    private TMP_Text labelGoogleLoginLink = null;
    [SerializeField]
    private TMP_Text labelFBLoginLink = null;

    public void InitAccoutSetting()
    {
        labelAppleLoginLink.text = "연동 안됨";
        labelGoogleLoginLink.text = "연동 안됨";
        labelFBLoginLink.text = "연동 안됨";
    }

    #region OnClick
    public void OnClickAppleLogin()
    {
#if UNITY_IOS
        FirebaseManager.Instance.SignInApple();
#endif
        labelAppleLoginLink.text = "연동 됨";
    }

    public void OnClickGoogleLogin()
    {
        FirebaseManager.Instance.SignInGoogle(() =>
        {
            labelGoogleLoginLink.text = "연동 됨";
        });
    }

    public void OnClickFBLogin()
    {
        StartCoroutine(FirebaseManager.Instance.IESignInFaceBook());
        labelFBLoginLink.text = "연동 됨";
    }
    #endregion
}

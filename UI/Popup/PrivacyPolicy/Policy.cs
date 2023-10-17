using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Policy : MonoBehaviour
{
    [SerializeField]
    private TMP_Text labelPolicyTitle = null;
    [SerializeField]
    private TMP_Text labelPolicyDesc = null;

    [SerializeField]
    private GameObject imgCheckGo = null;

    private bool isCheck = false;

    #region Get
    public bool IsCheck => isCheck;
    #endregion

    #region Set
    public void SetPolicyTitle(string title)
    {
        labelPolicyTitle.text = title;
    }

    public void SetPolicyDesc(string desc)
    {
        labelPolicyDesc.text = desc;
    }
    #endregion

    #region OnClick
    public void OnClickConfirm()
    {
        if(isCheck == false)
        {
            isCheck = true;
        }
        else
        {
            isCheck = false;
        }

        imgCheckGo.SetActive(isCheck);
    }
    #endregion
}

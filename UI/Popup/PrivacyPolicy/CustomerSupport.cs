using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class CustomerSupport : MonoBehaviour
{
    [Serializable]
    public class CSData
    {
        public string uid { get; private set; }
        public string email { get; private set; }
        public string csTitle { get; private set; }
        public string csContents { get; private set; }

        public void SetUID(string uid)
        {
            this.uid = uid;
        }

        public void SetEmail(string email)
        {
            this.email = email;
        }

        public void SetCSTitle(string csTitle)
        {
            this.csTitle = csTitle;
        }

        public void SetCSContents(string csContents)
        {
            this.csContents = csContents;
        }
    }

    [SerializeField]
    private TMP_Text labelUID = null;

    [SerializeField]
    private TMP_InputField inputEmail = null;
    [SerializeField]
    private TMP_InputField inputTitle = null;
    [SerializeField]
    private TMP_InputField inputContents = null;

    private void Awake()
    {
        InitCS();
    }

    private void InitCS()
    {
        labelUID.text = $"UID : {UserData.Instance.user.UserID}";
        inputEmail.text = "";
        inputTitle.text = "";
        inputContents.text = "";
    }

    private CSData SetCsData()
    {
        CSData data = new CSData();
        data.SetUID(UserData.Instance.user.UserID);
        data.SetEmail(inputEmail.text);
        data.SetCSTitle(inputTitle.text);
        data.SetCSContents(inputContents.text);

        return data;
    }

    public void OnClickSend()
    {
        //JsonConvert.SerializeObject(SetCsData());

        Debug.LogError("Àü¼Û : " + JsonConvert.SerializeObject(SetCsData()));
    }
}

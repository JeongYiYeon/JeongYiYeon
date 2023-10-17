using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    private DataHelper dataHelper = new DataHelper();

    public DataHelper DataHelper => dataHelper;

    public string GetLocalization(string enumID, params object[] values)
    {
        if(string.IsNullOrEmpty(enumID))
        {
            return string.Empty;
        }

        string localization = "";

        DataLocalization dataLocalization = dataHelper.Localization.Find(x => x.ENUM_ID == enumID);

        if (dataLocalization != null)
        {
            //���߿� �������� ���� ���� ������ߵ�
            localization = string.Format(dataLocalization.KR, values);
        }

        return localization;
    }

    public void SetDataHelper(DataHelper dataHelper)
    {
        this.dataHelper = CopyReciveData(dataHelper, this.dataHelper);
    }

    public void ResetDataHelper()
    {
        dataHelper = null;
        dataHelper = new DataHelper();
    }

    private DataHelper CopyReciveData(DataHelper newDataHelper, DataHelper originDataHelper)
    {
        JsonSerializer serializer = new JsonSerializer();

        StringReader _reciveData = new StringReader(
            JsonConvert.SerializeObject(newDataHelper,
                Formatting.Indented,
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }));

        if (_reciveData != null)
        {
            serializer.Populate(_reciveData, originDataHelper);
            newDataHelper = originDataHelper;
        }

        return newDataHelper;
    }
}

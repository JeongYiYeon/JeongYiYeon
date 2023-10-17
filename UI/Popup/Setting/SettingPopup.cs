using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SettingPopup : BasePopup
{
    [SerializeField]
    private UtilState state = null;

    [SerializeField]
    private ToggleGroup settingCategoryToggle = null;

    [SerializeField]
    private GameSetting gameSetting = null;
    [SerializeField]
    private AccountSetting accountSetting = null;
    [SerializeField]
    private CsSetting csSetting = null;

    public CsSetting CsSetting => csSetting;

    private void OnEnable()
    {
        StartCoroutine(ToggleOn(EnumSetting.ESettingCategory.GameSetting));
    }

    private void OnDisable()
    {
        csSetting.DeActiveCS();
        csSetting.DeActivePolicy();
    }

    private void Awake()
    {
        InitSetting();
    }

    private void InitSetting()
    {
        gameSetting.InitGameSetting();
    }

    private IEnumerator ToggleOn(EnumSetting.ESettingCategory category)
    {
        yield return new WaitForEndOfFrame();

        Transform tf = settingCategoryToggle.transform.Find(category.ToString());

        if (tf != null)
        {
            tf.GetComponent<Toggle>().SetIsOnWithoutNotify(true);
        }
    }

    public void SetSettingCategory(EnumSetting.ESettingCategory category)
    {
        state.ActiveState(category.ToString());
    }

    public void OnClickSettingCategory(UtilEnumSelect category)
    {
        SetSettingCategory(category.SettingCategory);
    }
}

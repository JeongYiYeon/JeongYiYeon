using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class GameSetting : MonoBehaviour
{
    [SerializeField]
    private Image imgBgmIcon = null;
    [SerializeField]
    private Image imgFxIcon = null;
    [SerializeField]
    private Slider sliderBgm = null;
    [SerializeField]
    private Slider sliderFX = null;

    [SerializeField]
    private TMP_Text labelFreeCash = null;
    [SerializeField]
    private TMP_Text labelPurchaseCash = null;
    [SerializeField]
    private TMP_Text labelGold = null;

    [SerializeField]
    private TMP_Text labelUID = null;
    [SerializeField]
    private TMP_Text labelGameVersion = null;

    public void InitGameSetting()
    {
        sliderBgm.value = SoundManager.Instance.BGM.volume;
        sliderFX.value = SoundManager.Instance.FX.volume;

        SetSoundIcon(SoundManager.ESound.BGM);
        SetSoundIcon(SoundManager.ESound.FX);

        SetUIDLabel();
        SetGoodsLabel();
        SetGameVersionLabel();
    }

    private void SetGoodsLabel()
    {
        labelFreeCash.text = UserData.Instance.user.Goods.FreeCash.ToString("#,##0");
        labelPurchaseCash.text = UserData.Instance.user.Goods.PuchaseCash.ToString("#,##0");
        labelGold.text = UserData.Instance.user.Goods.Gold.ToString("#,##0");
    }

    private void SetUIDLabel()
    {
        labelUID.text = $"UID : {UserData.Instance.user.UserID}";
    }

    private void SetGameVersionLabel()
    {
        labelGameVersion.text = $"게임 버전  : {Application.version}";
    }

    private void SetSoundIcon(SoundManager.ESound type)
    {
        string spritePath = "";

        switch (type)
        {
            case SoundManager.ESound.BGM:

                spritePath = sliderBgm.value == 0f ? "Texture/Character/hero" : "Texture/Character/sol";
                imgBgmIcon.sprite = Resources.Load<Sprite>(spritePath);

                break;

            case SoundManager.ESound.FX:

                spritePath = sliderFX.value == 0f ? "Texture/Character/hero" : "Texture/Character/sol";
                imgFxIcon.sprite = Resources.Load<Sprite>(spritePath);

                break;
        }
    }

    public void OnClickUIDCopy()
    {
        GUIUtility.systemCopyBuffer = UserData.Instance.user.UserID.ToString();
    }

    public void OnClickBgmVolume()
    {
        SoundManager.Instance.SetVolume(SoundManager.ESound.BGM, sliderBgm.value);
        SetSoundIcon(SoundManager.ESound.BGM);
    }

    public void OnClickFXVolume()
    {
        SoundManager.Instance.SetVolume(SoundManager.ESound.FX, sliderFX.value);
        SetSoundIcon(SoundManager.ESound.FX);
    }

}

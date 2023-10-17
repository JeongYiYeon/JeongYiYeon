using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using System;
using Coffee.UIExtensions;

public class Charge : MonoBehaviour
{
    [SerializeField]
    private AtlasImage imgGoods = null;

    [SerializeField]
    private TMP_Text labelDescCharge = null;
    [SerializeField]
    private TMP_Text labelRemainCount = null;

    [SerializeField]
    private GameObject priceGoodsGo = null;
    [SerializeField]
    private AtlasImage imgPriceGoods = null;
    [SerializeField]
    private TMP_Text labelGoodsPrice = null;

    private Action onConfirmCallback;

    private void Awake()
    {
        priceGoodsGo.SetActive(false);
    }

    #region Set   

    public void SetGoods(string spritePath)
    {
        imgGoods.sprite = Resources.Load<Sprite>(spritePath);
    }

    public void SetCharge(int chargeEnergy, int chargeCnt, int chargeMaxCnt)
    {
        labelDescCharge.text = $"{chargeEnergy} 충전";
        labelRemainCount.text = $"남은 충전 횟수 : {chargeMaxCnt - chargeCnt}";
    }

    public void SetPurchase(string spriteName, int price)
    {
        if(string.IsNullOrEmpty(spriteName) || price <= 0)
        {
            priceGoodsGo.SetActive(false);
            return;
        }

        priceGoodsGo.SetActive(true);

        AtlasManager.Instance.SetSprite(imgPriceGoods, imgPriceGoods.spriteAtlas, spriteName);

        labelGoodsPrice.text = price.ToString();
    }

    public void SetConfirmCallback(Action cb)
    {
        if (cb != null)
        {
            onConfirmCallback = cb;
        }
    }

    #endregion

    #region OnClick
    public void OnClickConfirm()
    {
        if (onConfirmCallback != null)
        {
            onConfirmCallback.Invoke();
        }
    }
    #endregion

}

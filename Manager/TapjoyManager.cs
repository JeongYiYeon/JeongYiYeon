using System;
using System.Collections;
using System.Collections.Generic;
using TapjoyUnity;
using UnityEngine;

public class TapjoyManager : MonoSingleton<TapjoyManager>
{
    private TJPlacement placemnet = null;

    private void Awake()
    {
        if (!Tapjoy.IsConnected)
        {
            Tapjoy.Connect();

            Tapjoy.OnConnectSuccess += HandleConnectSuccess;
        }
    }

    private void HandleConnectSuccess()
    {
        InitPlacement();
    }

    private void InitPlacement()
    {
        placemnet = TJPlacement.CreatePlacement("TapjoyAdReward");

        if (Tapjoy.IsConnected)
        {
            placemnet.RequestContent();

            TJPlacement.OnRequestSuccess += HandlePlacementRequestSuccess;
            TJPlacement.OnRequestFailure += HandlePlacementRequestFailure;
            TJPlacement.OnContentReady += HandlePlacementContentReady;
            TJPlacement.OnContentDismiss += HandlePlacementContentDismiss;
            TJPlacement.OnPurchaseRequest += HandleOnPurchaseRequest;
            TJPlacement.OnRewardRequest += HandleOnRewardRequest;
        }
    }

    private void HandleOnRewardRequest(TJPlacement placement, TJActionRequest request, string itemId, int quantity)
    {
        Debug.LogError("플레이스먼트 보상");
    }

    private void HandleOnPurchaseRequest(TJPlacement placement, TJActionRequest request, string productId)
    {
        Debug.LogError("구매");
    }

    private void HandlePlacementContentDismiss(TJPlacement placement)
    {
        Debug.LogError("플레이스먼트 오류");
    }

    private void HandlePlacementContentReady(TJPlacement placement)
    {
        Debug.LogError("플레이스먼트 준비");
    }

    private void HandlePlacementRequestFailure(TJPlacement placement, string error)
    {
        Debug.LogError("플레이스먼트 요청 실패");
    }

    private void HandlePlacementRequestSuccess(TJPlacement placement)
    {
        Debug.LogError("플레이스먼트 요청 성공");
    }

    public void OnClickShowTapjoy()        
    {
        if (placemnet.IsContentReady())
        {
            Debug.LogError("컨텐츠 보이기");

            placemnet.ShowContent();
        }
        else
        {
            // 표시 할 콘텐츠가 없거나 아직 다운로드되지 않은 상황을 처리합니다.
            Debug.LogError("없음");
            if (Tapjoy.IsConnected)
            {
                Debug.LogError("요청");
                placemnet.RequestContent();
            }
        }
    }
}

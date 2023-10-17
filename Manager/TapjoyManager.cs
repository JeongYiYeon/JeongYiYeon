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
        Debug.LogError("�÷��̽���Ʈ ����");
    }

    private void HandleOnPurchaseRequest(TJPlacement placement, TJActionRequest request, string productId)
    {
        Debug.LogError("����");
    }

    private void HandlePlacementContentDismiss(TJPlacement placement)
    {
        Debug.LogError("�÷��̽���Ʈ ����");
    }

    private void HandlePlacementContentReady(TJPlacement placement)
    {
        Debug.LogError("�÷��̽���Ʈ �غ�");
    }

    private void HandlePlacementRequestFailure(TJPlacement placement, string error)
    {
        Debug.LogError("�÷��̽���Ʈ ��û ����");
    }

    private void HandlePlacementRequestSuccess(TJPlacement placement)
    {
        Debug.LogError("�÷��̽���Ʈ ��û ����");
    }

    public void OnClickShowTapjoy()        
    {
        if (placemnet.IsContentReady())
        {
            Debug.LogError("������ ���̱�");

            placemnet.ShowContent();
        }
        else
        {
            // ǥ�� �� �������� ���ų� ���� �ٿ�ε���� ���� ��Ȳ�� ó���մϴ�.
            Debug.LogError("����");
            if (Tapjoy.IsConnected)
            {
                Debug.LogError("��û");
                placemnet.RequestContent();
            }
        }
    }
}

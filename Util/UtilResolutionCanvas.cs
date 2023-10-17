using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

[RequireComponent(typeof(CanvasScaler))]
public class UtilResolutionCanvas : MonoBehaviour
{    
    private CanvasScaler canvasScaler;
    private RectTransform rectTransform;

    public bool IsSync = false;

    private void Start()
    {
        IsSync = false;

        canvasScaler = GetComponent<CanvasScaler>();
        rectTransform = GetComponent<RectTransform>();

        //60프레임 고정
        Application.targetFrameRate = 60;

        SyncCanvasScaler();
    }

#if UNITY_EDITOR
    private void Update()
    {
        SyncCanvasScaler();
    }
#endif

    private void SyncCanvasScaler()
    {
        var resolution = rectTransform.sizeDelta;
        var aspect = resolution.x / resolution.y;

        var min = canvasScaler.referenceResolution;
        var minAspect = min.x / min.y;

        canvasScaler.matchWidthOrHeight = aspect < minAspect ? 0 : 1;

        IsSync = true;
    }
}

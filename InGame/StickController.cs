using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StickController : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField]
    private RectTransform stickBgTf;
    [SerializeField]
    private RectTransform stickTf;

    private Vector3 inputVec;

    private bool isTouch = false;

    public Vector3 InputVec => inputVec;

    void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.W))
        {
            isTouch = true;
            OnTouch(Vector3.up * 1000f, CameraManager.Instance.GetCamera(CameraManager.ECamera.UI));
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            isTouch = true;
            OnTouch(Vector3.left * 1000f, CameraManager.Instance.GetCamera(CameraManager.ECamera.UI));
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            isTouch = true;
            OnTouch(Vector3.down * 1000f, CameraManager.Instance.GetCamera(CameraManager.ECamera.UI));
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            isTouch = true;
            OnTouch(Vector3.right * 1000f, CameraManager.Instance.GetCamera(CameraManager.ECamera.UI));
        }
        if (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.S) || Input.GetKeyUp(KeyCode.D))
        {
            stickTf.anchoredPosition = Vector3.zero;
            inputVec = Vector3.zero;
            isTouch = false;

        }
#endif
    }

    private void OnTouch(Vector2 vecTouch, Camera camera)
    {
        Vector2 pos;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(stickBgTf, vecTouch, camera, out pos) == true)
        {
            pos.x /= stickBgTf.sizeDelta.x;
            pos.y /= stickBgTf.sizeDelta.y;

            inputVec = new Vector3(pos.x * 2, pos.y * 2, 0);
            inputVec = inputVec.magnitude > 1f ? inputVec.normalized : inputVec;

            stickTf.anchoredPosition = new Vector3(inputVec.x * (stickBgTf.sizeDelta.x / 2), inputVec.y * (stickBgTf.sizeDelta.y / 2), 0);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isTouch == true)
        {
            OnTouch(eventData.position, eventData.pressEventCamera);
        }
    }    

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouch = true;
        OnTouch(eventData.position, eventData.pressEventCamera);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        stickTf.anchoredPosition = Vector3.zero;
        inputVec = Vector3.zero;
        isTouch = false;
        GameManager.Instance.HeroCharacter.SetState(BaseCharacter.AnimationType.Idle.ToString());
    }
}

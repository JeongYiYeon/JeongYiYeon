using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoSingleton<CameraManager>
{
    public enum ECamera
    {
        Main,
        Bg,
        UI,
        Game,
    }

    [SerializeField]
    private Camera[] cameras = null;

    private float resolutionWidth = 720f;
    private float resolutionHeight = 1080f;

    public float ResolutionWidth => resolutionWidth;
    public float ResolutionHeight => resolutionHeight;

    private void OnEnable()
    {
        SetCameraSize();
    }

#if UNITY_EDITOR
    private void FixedUpdate()
    {
        SetCameraSize();
    }
#endif

    public Camera GetCamera(ECamera camera)
    {
        return cameras[(int)camera];
    }

    public void SetCanvasCamera(Canvas[] canvas)
    {       
        this.transform.SetParent(null);
        SceneManager.MoveGameObjectToScene(this.gameObject, SceneManager.GetActiveScene());
        this.transform.SetParent(transform);

        for (int i = 0; i < canvas.Length; i++)
        {
            canvas[i].worldCamera = GetCamera((ECamera)i + 1);
        }

        DontDestroyOnLoad(this);
    }

    private void SetCameraSize()
    {
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].orthographicSize = Screen.width / (100f * 2f);
        }
    }

    public void SetResolutionWidth(float resolutionWidth)
    {
        this.resolutionWidth = resolutionWidth;
    }

    public void SetResolutionHeight(float resolutionHeight)
    {
        this.resolutionHeight = resolutionHeight;
    }
}

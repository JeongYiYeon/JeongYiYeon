using Coffee.UIExtensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class BackgroundController : MonoBehaviour
{
    [SerializeField]
    private RectTransform[] cloudTf = new RectTransform[MAXLENGTH];

    [SerializeField]
    private RectTransform[] distantViewTf = new RectTransform[MAXLENGTH];

    [SerializeField]
    private RectTransform[] middleGroundTf = new RectTransform[MAXLENGTH];

    [SerializeField]
    private RectTransform[] foreGroundTf = new RectTransform[MAXLENGTH];

    [SerializeField]
    private RectTransform[] nearGroundTf = new RectTransform[MAXLENGTH];

    [SerializeField]
    private RectTransform[] groundTf = new RectTransform[MAXLENGTH];

    private const int MAXLENGTH = 4;

    private float speed = 1f;

    private float screenXSize = 1f;
    private float screenYSize = 1f;

    private float leftXPos = 0f;
    private float rightXPos = 0f;

    private void OnEnable()
    {
        ResetPostion();

        screenYSize = CameraManager.Instance.GetCamera(CameraManager.ECamera.Bg).orthographicSize;
        screenXSize = screenYSize * CameraManager.Instance.GetCamera(CameraManager.ECamera.Bg).aspect;

        //¾ÞÄ¿ 0.5±âÁØ
        leftXPos = -(screenXSize * 2);
        rightXPos = screenXSize * 2 * MAXLENGTH;

    }

    private void Awake()
    {
        StartCoroutine(IEMoveCloud());
    }

    public void SetSpeed(float speed)
    {
        this.speed = speed;
    }

    public void ChangeAtlas(SpriteAtlas atlas)
    {
        Component[] atlasImg = this.transform.GetComponentsInChildren(typeof(AtlasImage), true);

        if (atlasImg.Length > 0)
        {
            for (int i = 0; i < atlasImg.Length; i++)
            {
                AtlasImage tmp = (AtlasImage)atlasImg[i];

                AtlasManager.Instance.SetSprite(tmp, atlas, tmp.spriteName);                
            }
        }
    }

    private IEnumerator IEMoveCloud()
    {
        if (cloudTf == null)
        {
            yield break;
        }

        while (true)
        {
            for(int i = 0; i < 2; i++)
            {
                cloudTf[i].Translate(Vector3.left * Time.deltaTime * speed / 10f, Space.World);

                if (cloudTf[i].position.x < leftXPos)
                {
                    ResetPosition(cloudTf[i]);
                }
            }

            yield return null;
        }
    }


    public void MoveBackGround(Vector2 dir)
    {
        if (distantViewTf == null)
        {
            return;
        }

        if (middleGroundTf == null)
        {
            return;
        }

        if (foreGroundTf == null)
        {
            return;
        }

        if(nearGroundTf == null)
        {
            return;
        }

        if(groundTf == null)
        {
            return;
        }

        for(int i = 0; i < MAXLENGTH; i++) 
        {
            CacluateBgPosition(distantViewTf[i], dir, speed / 8f);
            CacluateBgPosition(middleGroundTf[i], dir, speed / 4f);
            CacluateBgPosition(foreGroundTf[i], dir, speed / 2f);
            CacluateBgPosition(nearGroundTf[i], dir, speed / 3f);
            CacluateBgPosition(groundTf[i], dir, speed);
        }
    }

    private void CacluateBgPosition(RectTransform rt, Vector2 dir, float speed)
    {
        rt.Translate(-dir * Time.deltaTime * speed, Space.World);

        if (rt.position.x < leftXPos)
        {           
            ResetPosition(rt);
        }
    }

    private void ResetPosition(RectTransform rt)
    {
        Vector3 nextPos = rt.position;
        nextPos = new Vector3(nextPos.x + rightXPos, nextPos.y, nextPos.z);
        rt.position = nextPos;
    }

    private void ResetPostion()
    {
        if(CameraManager.Instance == null)
        {
            return;
        }

        if (distantViewTf != null)
        {
            for (int i = 0; i < MAXLENGTH; i++)
            {
                ResetPosition(distantViewTf[i]);
            }
        }

        if (middleGroundTf != null)
        {
            for (int i = 0; i < MAXLENGTH; i++)
            {
                ResetPosition(middleGroundTf[i]);
            }

        }

        if (foreGroundTf != null)
        {
            for (int i = 0; i < MAXLENGTH; i++)
            {
                ResetPosition(foreGroundTf[i]);
            }

        }

        if (nearGroundTf != null)
        {
            for (int i = 0; i < MAXLENGTH; i++)
            {
                ResetPosition(nearGroundTf[i]);
            }
        }

        if(groundTf != null)
        {
            for (int i = 0; i < MAXLENGTH; i++)
            {
                ResetPosition(groundTf[i]);
            }
        }
    }
}

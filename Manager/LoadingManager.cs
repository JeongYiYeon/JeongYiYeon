using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadingManager : MonoSingleton<LoadingManager>
{
    public enum EState
    {
        None,

        SceneLoading,               // 씬 로딩
        OneLineAlram,               // 한줄 경고 표시
        Loading,                    // 로딩 UI (버튼 같은거 눌렀을때 용)
        NetworkSync                 // 네트워크 동기화 싱크
    }


    public enum EScene
    {
        INTRO,
        LOBBY,
    }

    [SerializeField]
    private UtilState state = null;

    [Header("로딩씬")]
    [SerializeField]
    private Image imgSceneLoading = null;
    [SerializeField]
    private TMP_Text labelSceneLoading = null;

    [SerializeField]
    private Image imgSceneLoadingGauge = null;


    [Header("한줄 알림창")]
    [SerializeField]
    private Transform oneLineAlramTf = null;
    [SerializeField]
    private TMP_Text labelOneLineAlram = null;

    [Header("로딩창")]
    [SerializeField]
    private Transform imgLoadingTf = null;

    private float time = 0;
    private EScene scene = EScene.INTRO;

    private EState currentState = EState.None;

    private Sequence oneLineAlramSeq = null;
    private Coroutine corOneLineAlram = null;

    public void LoadScene(EScene scene, Action cb = null)
    {
        this.scene = scene;

        DontDestroyOnLoad(CameraManager.Instance.gameObject);

        StartCoroutine(IELoadScene(GetSceneName(), cb));
    }

    public void ActiveOneLineAlram(string msg)
    {
        if(corOneLineAlram != null)
        {
            oneLineAlramSeq.Kill();
            StopCoroutine(corOneLineAlram);
            oneLineAlramSeq = null;
            corOneLineAlram = null;
            Reset(EState.OneLineAlram);
        }

        currentState = EState.OneLineAlram;

        corOneLineAlram = StartCoroutine(IEOneLineAlram(msg));
    }

    public void ActiveLoading()
    {
        currentState = EState.Loading;

        state.ActiveState(EState.Loading.ToString());

        StartCoroutine(IELoading());
    }

    public void ActiveNetworkSync()
    {
        currentState = EState.NetworkSync;

        state.ActiveState(EState.NetworkSync.ToString());        
    }

    public void Reset(EState state)
    {
        if (state == EState.SceneLoading)
        {
            time = 0;
            imgSceneLoadingGauge.fillAmount = 0;
            this.state.DeActiveState(EState.SceneLoading.ToString());
        }
        else if (state == EState.OneLineAlram)
        {
            oneLineAlramTf.localPosition = new Vector3(0, 150f, 0);
            this.state.DeActiveState(EState.OneLineAlram.ToString());
        }
        else if (state == EState.Loading)
        {
            imgLoadingTf.localRotation = Quaternion.identity;
            this.state.DeActiveState(EState.Loading.ToString());
        }
        else if (state == EState.NetworkSync)
        {
            this.state.DeActiveState(EState.NetworkSync.ToString());
        }

        currentState = EState.None;
    }

    private IEnumerator IELoadScene(string sceneName, Action cb = null)
    {
        currentState = EState.SceneLoading;
        state.ActiveState(EState.SceneLoading.ToString());

        imgSceneLoading.sprite = Resources.Load<Sprite>($"Texture/Background/bg_0{UnityEngine.Random.Range(0, 4)}");
        labelSceneLoading.text = DataManager.Instance.GetLocalization($"SYS_LOADING_GUIDE_0{UnityEngine.Random.Range(1, 10)}");

        if (scene == EScene.INTRO)
        {
            if(AddressableManager.Instance.IsSceneLoad == false)
            {
                var loadingScene = AddressableManager.Instance.LoadScene(sceneName);
                while (loadingScene.Status != AsyncOperationStatus.Succeeded)
                {
                    yield return null;
                }
            }

            AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;

            op.completed += (result) =>
            {
                if (result.isDone == true)
                {
                    imgSceneLoadingGauge.fillAmount = 1f;

                    Reset(EState.SceneLoading);

                    if (cb != null)
                    {
                        cb.Invoke();
                    }
                }
            };

            while (op.isDone == false)
            {
                time += Time.deltaTime;

                imgSceneLoadingGauge.fillAmount = time;

                if (op.progress >= 0.9f)
                {
                    if (imgSceneLoadingGauge.fillAmount >= 1f)
                    {
                        op.allowSceneActivation = true;
                    }
                }

                yield return null;
            }
        }

        else
        {
            var loadingScene = AddressableManager.Instance.LoadScene(sceneName);

            while (loadingScene.Status != AsyncOperationStatus.Succeeded)
            {
                time += Time.deltaTime;
                imgSceneLoadingGauge.fillAmount = time;

                yield return null;
            }

            imgSceneLoadingGauge.fillAmount = 1f;
            Reset(EState.SceneLoading);

            if (cb != null)
            {
                cb.Invoke();
            }            
        }
    }

    private string GetSceneName()
    {
        string name = "";

        switch(scene)
        {
            case EScene.INTRO:
                return name = "100_Intro";
            case EScene.LOBBY:
                return name = "200_Lobby";

            default:
                return name;
        }
    }

    private IEnumerator IEOneLineAlram(string msg)
    {
        oneLineAlramSeq = DOTween.Sequence()
            .SetAutoKill(false)
            .OnStart(() =>
            {
                oneLineAlramTf.localPosition = new Vector3(0, 150f, 0);

                labelOneLineAlram.text = msg;

                state.ActiveState(EState.OneLineAlram.ToString());
            });

        oneLineAlramSeq.Append(oneLineAlramTf.DOLocalMoveY(0, 0.4f));
        oneLineAlramSeq.Restart();

        yield return new WaitForSeconds(oneLineAlramSeq.Duration());

        yield return new WaitForSeconds(1f);

        Reset(EState.OneLineAlram);
    }

    private IEnumerator IELoading()
    {
        float time = 0f;

        while(currentState == EState.Loading)
        {
            if(currentState == EState.None)
            {
                yield break;
            }
            
            time += Time.deltaTime;

            imgLoadingTf.localRotation = Quaternion.Euler(0, 0, 10f * time);

            yield return null;
        }
    }
}

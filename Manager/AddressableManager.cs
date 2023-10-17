using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class AddressableManager : MonoSingleton<AddressableManager>
{
    private bool isPatchComplete = false;

    private bool isComplete = false;

    private double patchSize = 0;

    private bool isSceneLoad = true;

    private SceneInstance loadedScene;

    private Dictionary<string, long> progressDic = new Dictionary<string, long>();

    private Dictionary<string, IResourceLocation> addressableDataDic = new Dictionary<string, IResourceLocation>();

    public bool IsPatchComplete => isPatchComplete;

    public bool IsComplete => isComplete;
    public double PatchSize => patchSize;

    public Dictionary<string, long> ProgressDic => progressDic;

    public bool IsSceneLoad => isSceneLoad;

    public void GetCheckAddressable()
    {
        StartCoroutine(Patch());
    }

    public GameObject Instantiate(string key, Transform parent)
    {
        if (addressableDataDic.ContainsKey(key))
        {
            return Addressables.InstantiateAsync(addressableDataDic[key], parent).WaitForCompletion();
        }
        else
        {
            return null;
        }
    }

    public AsyncOperationHandle LoadScene(string key)
    {
        AsyncOperationHandle<SceneInstance> handle;

        if (isSceneLoad == true)
        {
            handle = Addressables.LoadSceneAsync(key, LoadSceneMode.Single);
            handle.Completed += OnSceneLoaded;
        }
        else
        {
            handle = Addressables.UnloadSceneAsync(loadedScene);
            handle.Completed += OnSceneUnloaded;
        }

        return handle;
    }

    private void OnSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            loadedScene = obj.Result;
            isSceneLoad = false;
        }
        else
        {
            Debug.LogError("로드 실패");
        }
    }
    private void OnSceneUnloaded(AsyncOperationHandle<SceneInstance> obj)
    {
        if (obj.Status == AsyncOperationStatus.Succeeded)
        {
            isSceneLoad = true;
            loadedScene = new SceneInstance();
        }
        else
        {
            Debug.LogError("언로드 실패");
        }
    }

    public T Load <T>(string key)
    {
        return Load<T>(addressableDataDic[key]);
    }

    public T Load<T>(IResourceLocation location)
    {
        return Addressables.LoadAssetAsync<T>(location).WaitForCompletion();
    }

    public AsyncOperationHandle<IList<IResourceLocation>> GetDatasLocations<T>(string labels)
    {
        return Addressables.LoadResourceLocationsAsync(labels, typeof(T));        
    }

    private IEnumerator IESetAddressableDataDic(List<string> labels)
    {
        for (int i = 0; i < labels.Count; i++)
        {
            var location = Addressables.LoadResourceLocationsAsync(labels[i], typeof(GameObject));

            yield return location;

            if(location.Result.Count > 0)
            {
                for (int j = 0; j < location.Result.Count; j++)
                {
#if UNITY_EDITOR
                    string[] tmpInternalId = location.Result[j].InternalId.Split("/");
                    string[] tmpKey = tmpInternalId[tmpInternalId.Length - 1].Split(".");
#else
                    string[] tmpKey = location.Result[j].InternalId.Split(".");
#endif

                    if (addressableDataDic.ContainsKey(tmpKey[0]) == false)
                    {
                        addressableDataDic.Add(tmpKey[0], location.Result[j]);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        isComplete = true;
    }

    private IEnumerator CheckUpdate(List<string> labels)
    {
        patchSize = 0;

        foreach (var label in labels)
        {
            var handle = Addressables.GetDownloadSizeAsync(label);

            yield return handle;

            patchSize += handle.Result;

            Addressables.Release(handle);
        }

        if (patchSize > 0)
        {
            Debug.LogError(patchSize / Mathf.Pow(1024f, 2) + "MB");
        }
    }

    private IEnumerator Patch()
    {
        var labels = new List<string>()
        {
            "Labels_Animation", "Labels_Atlas", "Labels_Effect", "Labels_JsonData",
            "Labels_Material","Labels_Prefab","Labels_Sprite"
        };

        yield return CheckUpdate(labels);

        if (patchSize > 0)
        {
            AlramPopup popup = UIManager.Instance.GetPopup(BasePopup.EPopupType.Alram) as AlramPopup;
            popup.SetButtonType(BasePopup.EButtonType.Two);
            popup.SetTitle("리소스 다운로드");
            popup.SetDesc($"{Math.Round(patchSize / Mathf.Pow(1024f, 2), 2)} MB 리소스를 다운로드 합니다.");
            popup.SetConfirmBtLabel("다운로드");
            popup.SetCancelBtLabel("앱 종료");
            popup.SetConfirmCallBack(() =>
            {
                AddressableDownload(labels);

                popup.DeActive();
            });
            popup.SetCancelCallback(() =>
            {
                Application.Quit();
            });

            popup.Active();
        }
        else
        {
            Debug.LogError("받을꺼 없음");

            isPatchComplete = true;
        }


        yield return new WaitUntil(() => isPatchComplete);
        yield return StartCoroutine(IESetAddressableDataDic(labels));
    }

    private void AddressableDownload(List<string> labels)
    {
        StartCoroutine(IEDownload(labels));
        StartCoroutine(Intro.Instance.IEAssetDownload());
    }


    private IEnumerator IEDownload(List<string> labels)
    {
        foreach (var label in labels)
        {
            var tmpDownload = Addressables.GetDownloadSizeAsync(label);

            yield return tmpDownload;

            if (tmpDownload.Result == 0)
            {
                Addressables.Release(tmpDownload);
                continue;
            }
            else
            {
                progressDic.Add(label, 0);

                var handle = Addressables.DownloadDependenciesAsync(label, false);

                while (!handle.GetDownloadStatus().IsDone)
                {
                    progressDic[label] = handle.GetDownloadStatus().DownloadedBytes;
                    yield return new WaitForEndOfFrame();
                }

                progressDic[label] = handle.GetDownloadStatus().TotalBytes;

                Addressables.Release(tmpDownload);
                Addressables.Release(handle);

                Debug.LogError(label + "다운 완료");
            }
        }

        isPatchComplete = true;
    }

    public void ClearCache()
    {
        StartCoroutine(IEClearCache());
    }

    private IEnumerator IEClearCache()
    {
        foreach (var tmp in Addressables.ResourceLocators)
        {
            var async = Addressables.ClearDependencyCacheAsync(tmp.Keys, false);
            yield return async;
            Addressables.Release(async);
        }

        Caching.ClearCache();

        isPatchComplete = false;
        isComplete = false;
        addressableDataDic.Clear();

        LoadingManager.Instance.LoadScene(LoadingManager.EScene.INTRO);
    }
}

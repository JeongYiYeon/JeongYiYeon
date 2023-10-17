using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine.U2D;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.ResourceLocations;
using System.Linq;

public class Intro : MonoSingletonInScene<Intro>
{
    [SerializeField]
    private Image dataLoadingProgressGauge = null;

    [SerializeField]
    private TMP_Text labelDataLoading = null;

    private IList<IResourceLocation> locations = new List<IResourceLocation>();

    private int loadingDataIdx = 0;

    private void Awake()
    {
        DataManager.Instance.ResetDataHelper();

        UserData.Instance.InitUser();

        SoundManager.Instance.SetVolume(SoundManager.ESound.BGM, UserData.Instance.user.BGMVolume);
        SoundManager.Instance.SetVolume(SoundManager.ESound.FX, UserData.Instance.user.FXVolume);
        labelDataLoading.text = $"리소스 확인 중....";

#if UNITY_ANDROID
        FirebaseManager.Instance.InitGPGS(() => 
        {
            AddressableManager.Instance.GetCheckAddressable();

            StartCoroutine(IEDataLoading());
        });
#else
        AddressableManager.Instance.GetCheckAddressable();
        StartCoroutine(IEDataLoading());
#endif

        //StartCoroutine(IEStartIntro());

    }

    private IEnumerator IEDataLoading()
    {
        yield return new WaitUntil(() => AddressableManager.Instance.IsComplete);

        var jsonDatas = AddressableManager.Instance.GetDatasLocations<TextAsset>("Labels_JsonData");

        yield return jsonDatas;

        locations = jsonDatas.Result;

        StartCoroutine(IEDataLoadingGauge());

        for (int i = 0; i < locations.Count; i++)
        {
            var getJson = AddressableManager.Instance.Load<TextAsset>(locations[i]);
            DataManager.Instance.SetDataHelper(JsonConvert.DeserializeObject<DataHelper>(getJson.text));
            loadingDataIdx++;          
        }

        yield return null;

    }

    private IEnumerator IEDataLoadingGauge()
    {
        float time = 0;

        while (true)
        {
            time += Time.deltaTime;

            dataLoadingProgressGauge.fillAmount = (float)loadingDataIdx / locations.Count;

            labelDataLoading.text = $"데이터 로드 중 {(int)(dataLoadingProgressGauge.fillAmount * 100)}%";

            if (loadingDataIdx == locations.Count)
            {
                yield return StartCoroutine(IELoadSound());
                yield return StartCoroutine(IELoadAtlas());

                yield return new WaitForSeconds(1f);

                FirebaseManager.Instance.InitFirebase(() => 
                {
                    FirebaseManager.Instance.SignInGuest();
                });
                
                //FacebookManager.Instance.InitFB();

                yield return new WaitUntil(() => !string.IsNullOrEmpty(UserData.Instance.user.UDID));

                yield return UniTask.ToCoroutine(async () =>
                await NetworkPacketUser.Instance.TaskSiginUp());

                yield break;
            }

            yield return new WaitForEndOfFrame();
        }
    }



    public IEnumerator IEAssetDownload()
    {
        double total = 0;
        dataLoadingProgressGauge.fillAmount = 0.0f;
        labelDataLoading.text = "0%";

        while (true)
        {
            total += AddressableManager.Instance.ProgressDic.Sum(tmp => tmp.Value);
            dataLoadingProgressGauge.fillAmount = (float)(total / AddressableManager.Instance.PatchSize);
            labelDataLoading.text = $"리소스 다운로드 중 {(int)(dataLoadingProgressGauge.fillAmount * 100)}%";

            if (total == AddressableManager.Instance.PatchSize && AddressableManager.Instance.IsPatchComplete == true)
            {
                dataLoadingProgressGauge.fillAmount = 1f;
                labelDataLoading.text = $"리소스 다운로드 완료";
                yield break;
            }

            total = 0.0f;
            yield return new WaitForEndOfFrame();
        }
    }


    private IEnumerator IELoadAtlas()
    {
        var atlasData = AddressableManager.Instance.GetDatasLocations<SpriteAtlas>("Labels_Atlas");

        yield return atlasData;

        if (atlasData.Result.Count > 0)
        {
            Dictionary<string, SpriteAtlas> uiAtlasDic = new Dictionary<string, SpriteAtlas>();

            for (int i = 0; i < atlasData.Result.Count; i++)
            {
                var getAtlas = AddressableManager.Instance.Load<SpriteAtlas>(atlasData.Result[i]);

                uiAtlasDic.Add(getAtlas.name, getAtlas);
            }

            AtlasManager.Instance.SetAtlas(uiAtlasDic);
        }

        yield return null;
    }

    private IEnumerator IELoadSound()
    {
        var soundData = AddressableManager.Instance.GetDatasLocations<AudioClip>("Labels_Sound");

        yield return soundData;

        if (soundData.Result.Count > 0)
        {
            Dictionary<string, AudioClip> soundDic = new Dictionary<string, AudioClip>();

            for (int i = 0; i < soundData.Result.Count; i++)
            {
                var getSound = AddressableManager.Instance.Load<AudioClip>(soundData.Result[i]);

                soundDic.Add(getSound.name, getSound);
            }

            SoundManager.Instance.SetSoundDic(soundDic);
        }

        yield return null;
    }


    private IEnumerator IEStartIntro()
    {
        yield return new WaitForSeconds(1f);

        PrivacyPolicyPopup privacyPolicyPopup = 
            UIManager.Instance.GetPopup(BasePopup.EPopupType.PrivacyPolicy) as PrivacyPolicyPopup;

        privacyPolicyPopup.SetBackKeyOn(false);
        privacyPolicyPopup.SetDestory(true);
        privacyPolicyPopup.SetConfirmCallBack(
            () =>
            {
                LoadingManager.Instance.LoadScene(LoadingManager.EScene.LOBBY);

                privacyPolicyPopup.DeActive();
            });

        privacyPolicyPopup.Active();
    }
}

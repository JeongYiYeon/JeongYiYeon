using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_ANDROID
using GooglePlayGames;
#endif

public class CheatManager : MonoSingleton<CheatManager>
{
    private void OnEnable()
    {
#if ENABLE_CHEAT
        this.gameObject.SetActive(true);
#else
        this.gameObject.SetActive(false);
#endif
    }

#if ENABLE_CHEAT
    public void OnClickGetMaterial()
    {
        NetworkPacketCheat.Instance.TaskGetMaterial().Forget();
    }
    public void OnClickGetGoods()
    {
        NetworkPacketCheat.Instance.TaskGetGoods().Forget();
    }

    public void OnClickUserReset()
    {
        NetworkPacketUser.Instance.TaskResetUser().Forget();
    }

    public void OnClickGPGSLogout()
    {
#if UNITY_ANDROID
            PlayGamesPlatform.Instance.SignOut();
#endif
    }
#endif

}

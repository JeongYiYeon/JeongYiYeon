using Cysharp.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NetworkPacketCharacter : Singleton<NetworkPacketCharacter>
{
    private const string requestPath = "/character";

    private const string Position = requestPath + "/position";
    private const string ChangeHero = requestPath + "/hero";
    private const string UpgradeHero = requestPath + "/enchant";

    Dictionary<string, List<CharacterRequestParameter>> requestForm = new Dictionary<string, List<CharacterRequestParameter>>();

    private class CharacterRequestParameter
    {
        private int uid;
        private int position;

        public int UID => uid;
        public int POSITION => position;

        public void SetUID(int uid)
        {
            this.uid = uid;
        }

        public void SetPosition(int position) 
        {
            this.position = position;
        }
    }


    public async UniTask TaskPosition(Dictionary<int, CharacterData> heroPosInfoDic, Action successCb = null)
    {
        requestForm.Clear();

        List<CharacterRequestParameter> requestParameter = new List<CharacterRequestParameter>();

        foreach(CharacterData characterData in heroPosInfoDic.Values)
        {
            if (characterData != null)
            {
                CharacterRequestParameter parameter = new CharacterRequestParameter();
                parameter.SetUID(characterData.DataCharacter.UID);
                parameter.SetPosition(characterData.PositionIdx);
                requestParameter.Add(parameter);
            }
        }

        requestForm.Add("LIST", requestParameter);

        await NetworkManager.Instance.SendRequest<JArray>(
            Position,
            form: NetworkManager.Instance.SerializeObject(requestForm),
            _successCb: (arrayData) => 
            {
                if(arrayData != null && arrayData.Count > 0)
                {
                    foreach(JObject character in arrayData)
                    {
                        int characterUID = character.Value<int>("UID");

                        int idx = UserData.Instance.user.Character.CharacterDatas.FindIndex(x => x.DataCharacter.UID == characterUID);

                        if(idx != -1)
                        {
                            UserData.Instance.user.Character.CharacterDatas[idx].SetUpgradeCnt(character.Value<int>("ENCHANTED_COUNT"));
                            UserData.Instance.user.Character.CharacterDatas[idx].SetPositionIdx(character.Value<int>("POSITION"));
                        }                       
                    }

                    if(successCb != null)
                    {
                        successCb.Invoke();
                    }
                }
            });
    }
    public async UniTask TaskChangeHero(int changeCharacterUID, Action successCb = null)
    {
        CharacterRequestParameter parameter = new CharacterRequestParameter();
        parameter.SetUID(changeCharacterUID);
        
        await NetworkManager.Instance.SendRequest<JArray>(
            ChangeHero,
            form: NetworkManager.Instance.SerializeObject(parameter),
            _successCb: (arrayData) =>
            {
                if (arrayData != null && arrayData.Count > 0)
                {
                    foreach (JObject character in arrayData)
                    {
                        int characterUID = character.Value<int>("UID");

                        int idx = UserData.Instance.user.Character.CharacterDatas.FindIndex(x => x.DataCharacter.UID == characterUID);
                        if (idx != -1)
                        {
                            UserData.Instance.user.Character.CharacterDatas[idx].SetUpgradeCnt(character.Value<int>("ENCHANTED_COUNT"));
                            UserData.Instance.user.Character.CharacterDatas[idx].SetPositionIdx(character.Value<int>("POSITION"));

                            UserData.Instance.user.Character.SetSelectCharacter(UserData.Instance.user.Character.CharacterDatas[idx]);
                        }                        
                    }

                    if (successCb != null)
                    {
                        successCb.Invoke();
                    }
                }
            });

    }
    public async UniTask TaskUpgradeHero(int upgradeCharacterUID, Action successCb = null)
    {
        CharacterRequestParameter parameter = new CharacterRequestParameter();
        parameter.SetUID(upgradeCharacterUID);

        await NetworkManager.Instance.SendRequest<JObject>(
            UpgradeHero,
            form: NetworkManager.Instance.SerializeObject(parameter),
            _successCb: (jsonData) =>
            {
                if (jsonData != null)
                {
                    JObject character = jsonData.Value<JObject>("CHARACTER");

                    int characterUID = character.Value<int>("UID");

                    int idx = UserData.Instance.user.Character.CharacterDatas.FindIndex(x => x.DataCharacter.UID == characterUID);
                    if (idx != -1)
                    {
                        UserData.Instance.user.Character.CharacterDatas[idx].SetUpgradeCnt(character.Value<int>("ENCHANTED_COUNT"));
                        UserData.Instance.user.Character.CharacterDatas[idx].SetPositionIdx(character.Value<int>("POSITION"));

                        if (UserData.Instance.user.Character.CharacterDatas[idx].PositionIdx == 6)
                        {
                            UserData.Instance.user.Character.SetSelectCharacter(UserData.Instance.user.Character.CharacterDatas[idx]);
                        }
                    }

                    JObject getMoney = jsonData.Value<JObject>("MONEY");

                    if (getMoney != null)
                    {
                        UserData.Instance.user.SetGoods(
                            NetworkManager.Instance.CopyData<UserData.UserGoods>(getMoney, UserData.Instance.user.Goods));
                    }

                    JArray itemArray = jsonData.Value<JArray>("ITEM");

                    if (itemArray != null && itemArray.Count > 0)
                    {
                        foreach (JObject item in itemArray)
                        {
                            UserData.Instance.user.Item.SetItem(item);
                        }
                    }

                    if (successCb != null)
                    {
                        successCb.Invoke();
                    }
                }
            });
    }

}

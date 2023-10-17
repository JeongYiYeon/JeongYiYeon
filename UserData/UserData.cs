using Coffee.UIExtensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class UserData : Singleton<UserData>
{
    [Flags]
    public enum EGoodsType 
    {
        NONE = 0,

        FREECASH = 1,
        PURCHASECASH = 2,

        GOLD = 4,

        CASH = FREECASH | PURCHASECASH,
    }

    [JsonObject]
    public class UserGoods
    {
        #region 유저 굿즈 정보

        private double totalCash { get { return freeCash + purchaseCash; } }

        [JsonProperty("CASH_FREE", NullValueHandling = NullValueHandling.Ignore)]
        private double freeCash = 0;

        [JsonProperty("CASH", NullValueHandling = NullValueHandling.Ignore)]
        private double purchaseCash = 0;

        [JsonProperty("GOLD", NullValueHandling = NullValueHandling.Ignore)]
        private double gold = 0;

        public double FreeCash => freeCash;
        public double PuchaseCash => purchaseCash;

        public double TotalCash => totalCash;
        public double Gold => gold;


        #endregion

        public UserGoods()
        {

        }
        public AtlasImage GetGoodsImage(int itemUID, AtlasImage goodsImg)
        {
            if (goodsImg == null)
            {
                return null;
            }

            DataItem item = DataManager.Instance.DataHelper.Item.Find(x => x.UID == itemUID);

            AtlasManager.Instance.SetSprite(goodsImg, AtlasManager.Instance.Atlas[item.ITEM_ICON_ATLAS], item.ITEM_ICON);

            return goodsImg;
        }

        public EGoodsType GetGoodsType(int itemUID)
        {
            switch (itemUID)
            {
                case 1:
                    return EGoodsType.PURCHASECASH;
                case 2:
                    return EGoodsType.FREECASH;
                case 3:
                    return EGoodsType.GOLD;
                default:
                    return EGoodsType.NONE;
            }
        }

        public string GetGoodsString(EGoodsType goodsType)
        {
            string tmpValue = "";
            double value = 0;

            switch (goodsType)
            {
                case EGoodsType.FREECASH:
                case EGoodsType.PURCHASECASH:
                case EGoodsType.CASH:
                    value = totalCash;
                    break;

                case EGoodsType.GOLD:
                    value = gold;
                    break;
            }

            tmpValue = GetPriceUnit(value);

            return tmpValue;
        }

        public void AddGoods(EGoodsType goodsType, double count)
        {
            switch (goodsType)
            {
                case EGoodsType.FREECASH:
                case EGoodsType.PURCHASECASH:
                case EGoodsType.CASH:
                    if (goodsType == EGoodsType.FREECASH)
                    {
                        freeCash += count;
                    }
                    else if (goodsType == EGoodsType.PURCHASECASH)
                    {
                        purchaseCash += count;
                    }
                    break;

                case EGoodsType.GOLD:
                    gold += count;
                    break;
            }
        }

        public void SubtractGoods(EGoodsType goodsType, double count)
        {
            switch (goodsType)
            {
                case EGoodsType.CASH:

                    purchaseCash -= count;

                    //유료 재화부터 소비하고 남은거 무료 재화에서 소비
                    if (purchaseCash < 0)
                    {
                        freeCash += purchaseCash;
                        purchaseCash = 0;
                    }

                    Debug.LogError($"남은 무료 재화 :{freeCash} 남은 유료 재화 : {purchaseCash}");
                    break;

                case EGoodsType.GOLD:
                    gold -= count;
                    break;
            }
        }
                
        public string GetPriceUnit(double value)
        {
            //아스키 65 = A 90 = Z
            string[] tmpValueString = value.ToString("E").Split('+');
            string AsciiUnit = "";
            double tmpValue = 0;

            if (tmpValueString.Length < 2)
            {
                return value.ToString("0.##");
            }
            else
            {
                if(double.TryParse(tmpValueString[1], out tmpValue) == false)
                {
                    return value.ToString("0.##");
                }
                else
                {
                    if (tmpValue < 3)
                    {
                        return value.ToString("0.##");
                    }

                    int tmpAsciiIdx = (int)tmpValue / 3;

                    char[] asciiArray = null;

                    //27부터는 AA시작 해야됨
                    if(tmpAsciiIdx > 26)
                    {
                        asciiArray = new char[2];

                        int tmpFirstAscii = tmpAsciiIdx / 27;
                        int tmpSecondAscii = tmpAsciiIdx % 27;

                        asciiArray[0] = (char)(64 + tmpFirstAscii);
                        asciiArray[1] = (char)(65 + tmpSecondAscii);    // 나머지가 0일때 A부터 표현
                    }
                    else
                    {
                        asciiArray = new char[1];

                        asciiArray[0] = (char)(64 + tmpAsciiIdx);
                    }

                    AsciiUnit = new string(asciiArray);

                    double tmpOriginValue = double.Parse(tmpValueString[0].Replace("E", "")) * Math.Pow(10, tmpValue);

                    return string.Format($"{(Math.Truncate(tmpOriginValue) / Math.Pow(1000, tmpAsciiIdx)).ToString("0.##")}{AsciiUnit}");
                }
            }
        }
    }

    [JsonObject]
    public class UserCharacter
    {
        #region 영웅정보

        private List<CharacterData> characterDatas = new List<CharacterData>();

        //현재 선택한 영웅 캐릭터
        private CharacterData selectCharacter;

        public List<CharacterData> CharacterDatas => characterDatas;
        public CharacterData SelectCharacter => selectCharacter;

        #endregion

        public UserCharacter()
        {

        }

        public void SetAddCharacterData(CharacterData characterData)
        {
            if(characterDatas.Find(x => x.DataCharacter.UID == characterData.DataCharacter.UID) == null)
            {
                characterDatas.Add(characterData);
            }
            else
            {
                int idx = characterDatas.FindIndex(x => x.DataCharacter.UID == characterData.DataCharacter.UID);

                foreach(KeyValuePair< EquipItemData.EEquipItemType, EquipItemData> data in characterDatas[idx].EquipmentDic)
                {
                    if (data.Value != null)
                    {
                        characterData.SetHeroEquipment(data.Key, data.Value);
                    }
                }

                characterDatas[idx] = characterData;
            }
        }

        public void SetCharacterDatas(List<CharacterData> characterDatas)
        {
            this.characterDatas = characterDatas;
        }

        public void SetHeroCharacter(CharacterData characterData)
        {
            for(int i = 0; i < characterDatas.Count; i++) 
            {
                if (characterDatas[i].DataCharacter.UID == characterData.DataCharacter.UID 
                    && characterDatas[i].Type == BaseCharacter.CHARACTER_TYPE.HERO)
                {
                    characterDatas[i] = characterData;
                    break;
                }
            }
        }

        public void SetSelectCharacter(CharacterData selectCharacter)
        {
            this.selectCharacter = selectCharacter;
        }        

        public CharacterData GetHeroCharacter()
        {
            if (characterDatas == null || characterDatas.Count == 0)
            {
                return null;
            }

            CharacterData characterData = characterDatas.Find(x => x.PositionIdx == UserData.Instance.user.Config.HeroPos);

            if(characterData != null)
            {
                return characterData;
            }
            else
            {
                return null;
            }
        }

        public List<CharacterData> GetFollowCharacters()
        {
            if (characterDatas == null || characterDatas.Count == 0)
            {
                return null;
            }

            List<CharacterData> characterDataList = characterDatas.FindAll(x => 
            x.PositionIdx != UserData.Instance.user.Config.HeroPos && x.PositionIdx != 0);

            if (characterDataList != null && characterDataList.Count > 0)
            {
                return characterDataList;
            }
            else
            {
                return null;
            }
        }
    }

    [JsonObject]
    public class UserItem
    {
        private Dictionary<BaseItem.EItemCategory, List<BaseItemData>> inventoryDic = new Dictionary<BaseItem.EItemCategory, List<BaseItemData>>();
                
        public Dictionary<BaseItem.EItemCategory, List<BaseItemData>> InventoryDic => inventoryDic;

        public UserItem()
        {

        }

        public void SetItem(JObject itemJObject)
        {
            if (itemJObject != null)
            {
                DataItem itemData = DataManager.Instance.DataHelper.Item.Find(x => x.UID == itemJObject.Value<int>("IID"));

                if(itemData == null)
                {
                    return;
                }

                BaseItem.EItemCategory type = BaseItem.EItemCategory.None;

                if (System.Enum.TryParse<BaseItem.EItemCategory>(itemData.ITEM_CATEGORY, false, out type) == true)
                {
                    if (type == BaseItem.EItemCategory.EQUIP)
                    {
                        EquipItemData equipmentItem = JsonConvert.DeserializeObject<EquipItemData>(itemJObject.ToString());

                        equipmentItem.SetDataItem(itemData);
                        equipmentItem.SetTitle(DataManager.Instance.GetLocalization(itemData.ITEM_NAME));
                        equipmentItem.SetDesc(DataManager.Instance.GetLocalization(itemData.ITEM_DESC));
                        equipmentItem.SetItemCategory(type);
                        equipmentItem.SetItemGrade(itemData.ITEM_GRADE);
                        equipmentItem.SetAtlasPath(itemData.ITEM_ICON_ATLAS);
                        equipmentItem.SetImgItemPath(itemData.ITEM_ICON);

                        SetEquipmentItem(equipmentItem, itemData);
                    }
                    else if (type == BaseItem.EItemCategory.ETC)
                    {
                        BaseItemData item = JsonConvert.DeserializeObject<BaseItemData>(itemJObject.ToString());

                        item.SetDataItem(itemData);
                        item.SetTitle(DataManager.Instance.GetLocalization(itemData.ITEM_NAME));
                        item.SetDesc(DataManager.Instance.GetLocalization(itemData.ITEM_DESC));
                        item.SetItemCategory(type);
                        item.SetItemGrade(itemData.ITEM_GRADE);
                        item.SetAtlasPath(itemData.ITEM_ICON_ATLAS);
                        item.SetImgItemPath(itemData.ITEM_ICON);

                        SetMaterialItem(item, itemData);
                    }
                }
            }
        }
       
        public BaseItemData GetItem(int itemUID)
        {
            BaseItemData itemData = null;

            foreach(List<BaseItemData> itemList in inventoryDic.Values.ToList())
            {
                itemData = itemList.Find(x => x.ItemUID == itemUID);

                if (itemData != null)
                {
                    return itemData;
                }
            }

            return null;
        }

        public double GetItemCount(int itemUID)
        {
            BaseItemData itemData = GetItem(itemUID);

            if (itemData == null)
            {
                return 0;
            }
            else
            {
                return itemData.ItemCount;
            }
        }

        public void SetMaterialItemDic(BaseItemData materialItem)
        {
            if (inventoryDic.ContainsKey(BaseItem.EItemCategory.ETC) == false)
            {
                inventoryDic.Add(BaseItem.EItemCategory.ETC, new List<BaseItemData>());
            }            

            inventoryDic[BaseItem.EItemCategory.ETC].Add(materialItem);
        }

        public void SetMaterialItem(BaseItemData item, DataItem dataItem)
        {
            item.SetAtlasPath(dataItem.ITEM_ICON_ATLAS);
            item.SetImgItemPath(dataItem.ITEM_ICON);
            item.SetItemMaxCount(dataItem.ITEM_MAX);
            item.SetItemStackCount(dataItem.ITEM_STACK);
            item.SetSellPossible(dataItem.SELL_POSSIBLE);

            if (dataItem.SELL_POSSIBLE == true)
            {
                item.SetSellGoodsType(dataItem.SELL_ITEM_UID);
                item.SetSellItemCount(dataItem.SELL_ITEM_COUNT);
            }

            SetMaterialItemDic(item);
        }

        public void SetEquipmentItemDic(EquipItemData itemData)
        {
            if(inventoryDic.ContainsKey(BaseItem.EItemCategory.EQUIP) == false)
            {
                inventoryDic.Add(BaseItem.EItemCategory.EQUIP, new List<BaseItemData>());
            }

            int tmpDataIdx = inventoryDic[BaseItem.EItemCategory.EQUIP].FindIndex(x => x.GetUID == itemData.GetUID);

            if (tmpDataIdx == -1)
            {
                inventoryDic[BaseItem.EItemCategory.EQUIP].Add(itemData);
            }
            else
            {
                inventoryDic[BaseItem.EItemCategory.EQUIP][tmpDataIdx] = itemData;
            }
        }

        public void SetEquipmentItem(EquipItemData equipItemData, DataItem dataItem)
        {
            EquipItemData.EEquipItemType type = EquipItemData.EEquipItemType.NONE;

            if (System.Enum.TryParse<EquipItemData.EEquipItemType>(dataItem.ITEM_TYPE, false, out type) == true)
            {
                //EquipItemData equipItemData = (EquipItemData)item;

                equipItemData.SetItemMaxCount(dataItem.ITEM_MAX);
                equipItemData.SetItemStackCount(dataItem.ITEM_STACK);
                equipItemData.SetSellPossible(dataItem.SELL_POSSIBLE);

                if (dataItem.SELL_POSSIBLE == true)
                {
                    equipItemData.SetSellGoodsType(dataItem.SELL_ITEM_UID);
                    equipItemData.SetSellItemCount(dataItem.SELL_ITEM_COUNT);
                }
                equipItemData.SetEquipItemType(type);
                equipItemData.SetDam(dataItem.MAIN_ATTACK_DAM);
                equipItemData.SetHp(dataItem.MAIN_HP);

                if (dataItem.OPTION_1 > 0)
                {
                    equipItemData.SetOption(dataItem.OPTION_1);
                }
                if (dataItem.OPTION_2 > 0)
                {
                    equipItemData.SetOption(dataItem.OPTION_2);
                }
                if (dataItem.OPTION_3 > 0)
                {
                    equipItemData.SetOption(dataItem.OPTION_3);
                }
                if (dataItem.OPTION_4 > 0)
                {
                    equipItemData.SetOption(dataItem.OPTION_4);
                }
                if (dataItem.OPTION_5 > 0)
                {
                    equipItemData.SetOption(dataItem.OPTION_5);
                }

                if(equipItemData.EquipCharacterUID > 0)
                {
                    SetHeroEquipment(equipItemData.EquipCharacterUID, type, equipItemData);
                }

                SetEquipmentItemDic(equipItemData);
            }
        }

    }

    [JsonObject]
    public class UserStage
    {
        [JsonProperty("UID")]
        private int stageUid;

        //[JsonProperty("IS_CLEAR")]
        //private bool isClear = false;

        [JsonProperty("ENTER_BOSS_ROOM")]
        private bool isEnterBoss = false;

        [JsonProperty("ENTER_TIME")]
        private DateTime stageEnterTime = DateTime.MinValue;

        public int StageUID => stageUid;
        //public bool IsClear => isClear;
        public bool IsEnterBoss => isEnterBoss;
        public DateTime StageEnterTime => stageEnterTime;

        public UserStage() 
        {
            stageUid = 1;
            //isClear = false;
            isEnterBoss = false;
        }
    }

    [JsonObject]
    public class UserStat
    {
        [JsonProperty("ATK")]
        private int _atkLv;
        [JsonProperty("HP")]
        private int _hpLv;
        [JsonProperty("SPEED")]
        private int _speedLv;
        [JsonProperty("CRIRATE")]
        private int _crirateLv;
        [JsonProperty("CRIDAM")]
        private int _criDamLv;

        public int AtkLv => _atkLv;
        public int HpLv => _hpLv; 
        public int SpeedLv => _speedLv;
        public int CrirateLv => _crirateLv;
        public int CriDamLv => _criDamLv;

        public UserStat()
        {

        }
    }

    public class User
    {
        private UserGoods goods;
        private UserCharacter character;
        private UserItem item;
        private UserStage stage;
        private UserStat stat;

        private ConfigManager config;

        #region 유저 정보

        //파베 토큰값
        private string udid = "";

        [JsonProperty("UID")]
        private string userid = "";

        [JsonProperty("SESSION_TOKEN")]
        private string sessionToken = "";
        [JsonProperty("RE_SESSION_TOKEN")]
        private string reSessionToken = "";

        private DateTime loginTime = DateTime.MinValue;

        private DateTime serverTime = DateTime.MinValue;

        //일일 출석 보상 카운트수
        private int dayAttendanceCnt = 0;
        //주간 출석 보상 카운트수
        private int weeklyAttendanceCnt = 0;

        public UserGoods Goods => goods;        
        public UserCharacter Character => character;        
        public UserItem Item => item;        
        public UserStage Stage => stage;
        public UserStat Stat => stat;

        public ConfigManager Config => config;


        public string UDID => udid;
        public string UserID => userid;
        public string SessionToken => sessionToken;
        public string ReSessionToken => reSessionToken;

        public DateTime LoginTime => loginTime;
        public DateTime ServerTime => serverTime;


        public int DayAttendanceCnt => dayAttendanceCnt;
        public int WeeklyAttendanceCnt => weeklyAttendanceCnt;

        #endregion               

        #region 셋팅, 저장값
        private float bgmVolume = 0.5f;
        private float fxVolume = 0.5f;
       
        public float BGMVolume => bgmVolume;
        public float FXVolume => fxVolume;
        #endregion

        public User()
        {
            goods = new UserGoods();
            character = new UserCharacter();
            item = new UserItem();
            stage = new UserStage();     
            stat = new UserStat();
        }

        public void SetLoginTime(DateTime loginTime)
        {
            //최초 로그인시 서버 타임이랑 동기화
    
            if (loginTime != DateTime.MinValue) 
            {
                this.loginTime = loginTime;
                serverTime = loginTime;
            }
            else
            {
                this.loginTime = DateTime.UtcNow;
                serverTime = DateTime.UtcNow;
            }
        }

        public void SetSessionToken(string sessionToken)
        {
            this.sessionToken = sessionToken;
        }

        public void SetReSessionToken(string reSessionToken)
        {
            this.reSessionToken = reSessionToken;
        }

        public void SetUDID(string udid)
        {
            this.udid = udid;
        }

        public void SetGoods(UserGoods goods)
        {
            this.goods = goods;
        }

        public void SetCharacter(UserCharacter character)
        {
            this.character = character;
        }

        public void SetItem(UserItem item)
        {
            this.item = item;
        }

        public void SetStage(UserStage stage)
        {
            this.stage = stage;
        }

        public void SetStat(UserStat stat) 
        {
            this.stat = stat;
        }

        public void SetConfig(ConfigManager config)
        {
            this.config = config;
        }

        public void SetDayAttendanceCount(int dayAttendanceCnt)
        {
            this.dayAttendanceCnt = dayAttendanceCnt;
        }

        public void SetWeeklyAttendanceCount(int weeklyAttendanceCnt)
        {
            this.weeklyAttendanceCnt = weeklyAttendanceCnt;
        }
    }

    private User _user = null;

    public User user => _user;

    public void InitUser()
    {
        _user = new User();
    }

    public void SetUser(User user)
    {
        _user = user;
    }

    public bool IsEnoughGoods(EGoodsType goodsType, double count)
    {
        double goods = 0;

        switch (goodsType)
        {
            case EGoodsType.CASH:
                goods = user.Goods.TotalCash - count;
                break;
            case EGoodsType.GOLD:
                goods = user.Goods.Gold - count;
                break;
        }

        return goods >= 0;
    }

    public void SetSelectCharacter(CharacterData selectCharacter)
    {
        user.Character.SetSelectCharacter(selectCharacter);
    }

    public static void SetHeroEquipment(int characterUID, EquipItemData.EEquipItemType equipType, EquipItemData equipItemData)
    {
        int idx = Instance.user.Character.CharacterDatas.FindIndex(x => x.DataCharacter.UID == characterUID);

        Instance.user.Character.CharacterDatas[idx].SetHeroEquipment(equipType, equipItemData);
    }

    public void SetHeroEquipment(int itemUID)
    {
        DataItem item = DataManager.Instance.DataHelper.Item.Find(x => x.UID == itemUID);
        if (item != null)
        {
            if (item.ITEM_CATEGORY != BaseItem.EItemCategory.EQUIP.ToString())
            {
                return;
            }
            else
            {
                EquipItemData.EEquipItemType type = EquipItemData.EEquipItemType.NONE;

                if (System.Enum.TryParse<EquipItemData.EEquipItemType>(item.ITEM_TYPE, false, out type) == true)
                {
                    EquipItemData equipItemData = new EquipItemData();

                    equipItemData.SetDataItem(item);
                    equipItemData.SetItemUID(item.UID);
                    equipItemData.SetTitle(DataManager.Instance.GetLocalization(item.ITEM_NAME));
                    equipItemData.SetDesc(DataManager.Instance.GetLocalization(item.ITEM_DESC));
                    equipItemData.SetItemCategory(BaseItem.EItemCategory.EQUIP);
                    equipItemData.SetItemGrade(item.ITEM_GRADE);
                    equipItemData.SetAtlasPath(item.ITEM_ICON_ATLAS);
                    equipItemData.SetImgItemPath(item.ITEM_ICON);
                    equipItemData.SetItemMaxCount(item.ITEM_MAX);
                    equipItemData.SetItemStackCount(item.ITEM_STACK);
                    equipItemData.SetSellPossible(item.SELL_POSSIBLE);

                    if (item.SELL_POSSIBLE == true)
                    {
                        equipItemData.SetSellGoodsType(item.SELL_ITEM_UID);
                        equipItemData.SetSellItemCount(item.SELL_ITEM_COUNT);
                    }
                    equipItemData.SetEquipItemType(type);
                    equipItemData.SetDam(item.MAIN_ATTACK_DAM);
                    equipItemData.SetHp(item.MAIN_HP);

                    if (item.OPTION_1 > 0)
                    {
                        equipItemData.SetOption(item.OPTION_1);
                    }
                    if (item.OPTION_2 > 0)
                    {
                        equipItemData.SetOption(item.OPTION_2);
                    }
                    if (item.OPTION_3 > 0)
                    {
                        equipItemData.SetOption(item.OPTION_3);
                    }
                    if (item.OPTION_4 > 0)
                    {
                        equipItemData.SetOption(item.OPTION_4);
                    }
                    if (item.OPTION_5 > 0)
                    {
                        equipItemData.SetOption(item.OPTION_5);
                    }

                    if (equipItemData.EquipCharacterUID > 0)
                    {
                        SetHeroEquipment(user.Character.SelectCharacter.DataCharacter.UID, type, equipItemData);
                    }
                }
            }
        }
    }

    public (CharacterData otherEquipmentHero, EquipItemData otherEquipItemData) GetOtherHeroEquipment(EquipItemData equipmentItemData)
    {
        foreach (CharacterData characterData in user.Character.CharacterDatas)
        {
            if (characterData.DataCharacter.UID != user.Character.SelectCharacter.DataCharacter.UID)
            {
                foreach (EquipItemData equipItemData in characterData.EquipmentDic.Values.ToList())
                {
                    if (equipItemData != null)
                    {
                        if (equipItemData.GetUID == equipmentItemData.GetUID)
                        {
                            return (characterData, equipItemData);
                        }
                    }
                }
            }
        }

        return (null, null);
    }
    public bool IsHeroEquipment(CharacterData selectCharacter, EquipItemData.EEquipItemType type)
    {
        if (selectCharacter.EquipmentDic.ContainsKey(type))
        {
            if (selectCharacter.EquipmentDic[type] == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }    
}

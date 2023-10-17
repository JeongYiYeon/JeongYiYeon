using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


/////////코드 임의로 수정하지마세요 제네레이터로만 생성합니다.//////////
//////////////////////안되면 Yeon 에게 문의주세요///////////////////////


    public class DataShop
    {
        public int UID { get; set; }
        public string GOODS_NAME { get; set; }
        public string GOODS_DESC { get; set; }

        [JsonProperty("EnumShop.EShopSubCategory")]
        public EnumShop.EShopSubCategory EShopSubCategory { get; set; }
        public int SLOT_NUM { get; set; }

        [JsonProperty("EnumShop.EEquipmentGachaCount")]
        public EnumShop.EEquipmentGachaCount EEquipmentGachaCount { get; set; }
        public string BUY_TYPE { get; set; }
        public int BUY_TYPE_VALUE { get; set; }
        public int DATE_START { get; set; }
        public int DATE_END { get; set; }
        public int BUY_ITEM { get; set; }
        public int BUY_ITEM_COUNT { get; set; }
        public string PAYMENT_CODE_AOS { get; set; }
        public string PAYMENT_CODE_IOS { get; set; }
        public int PAYMENT_PRICE { get; set; }
        public int REWARD_ITEM { get; set; }
        public int REWARD_ITEM_COUNT { get; set; }
        public bool REWARD_LINK_POSSIBLE { get; set; }
        public int REWARD_LINK_GROUP { get; set; }
        public string ITEM_ICON_ATLAS { get; set; }
        public string PACKAGE_ICON { get; set; }
        public string GOODS_ICON { get; set; }
    }


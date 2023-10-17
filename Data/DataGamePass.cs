using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


/////////코드 임의로 수정하지마세요 제네레이터로만 생성합니다.//////////
//////////////////////안되면 Yeon 에게 문의주세요///////////////////////


    public class DataGamePass
    {
        public int UID { get; set; }
        public int GAMEPASS_GROUP { get; set; }
        public int DATE_START { get; set; }
        public int DATE_END { get; set; }

        [JsonProperty("EnumShop.EGamePass")]
        public EnumShop.EGamePass EGamePass { get; set; }
        public int PAYMENT_CODE { get; set; }
        public int PAYMENT_PRICE { get; set; }
        public int GAMEPASS_LEVEL { get; set; }
        public int GAMEPASS_POINT_MIN { get; set; }
        public int GAMEPASS_POINT_MAX { get; set; }
        public int REWARD_ITEM { get; set; }
        public int REWARD_ITEM_COUNT { get; set; }
    }


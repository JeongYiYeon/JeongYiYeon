using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


/////////코드 임의로 수정하지마세요 제네레이터로만 생성합니다.//////////
//////////////////////안되면 Yeon 에게 문의주세요///////////////////////


    public class DataChallenge
    {
        public int UID { get; set; }
        public string CHALLENGE_NAME { get; set; }
        public string CHALLENGE_DESC { get; set; }
        public string CHALLENGE_BACK { get; set; }
        public string STAGE_TYPE { get; set; }
        public int BEFORE_STAGE { get; set; }
        public int NEXT_STAGE { get; set; }
        public int REGEN_GROUP { get; set; }
        public int TIME_LIMIT { get; set; }
        public int DAY_ENTER_COUNT { get; set; }
        public int ENTER_ITEM { get; set; }
        public int ENTER_ITEM_COUNT { get; set; }
        public int CLEAR_REWARD_GROUP { get; set; }
        public string STAGE_BGM { get; set; }
    }


using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


/////////코드 임의로 수정하지마세요 제네레이터로만 생성합니다.//////////
//////////////////////안되면 Yeon 에게 문의주세요///////////////////////


    public class DataRewardLink
    {
        public int UID { get; set; }
        public string REWARD_TYPE { get; set; }
        public string REWARD_TYPE_VALUE { get; set; }
        public int REWARD_GROUP { get; set; }
        public int REWARD_GROUP_STEP { get; set; }
        public int REWARD_ITEM { get; set; }
        public int REWARD_ITEM_COUNT { get; set; }
        public int REWARD_CHARACTER_UID { get; set; }
        public double REWARD_ITEM_RATE { get; set; }
        public bool MAIL_POSSIBLE { get; set; }
        public string MAIL_TITLE { get; set; }
        public string MAIL_TEXT { get; set; }
    }


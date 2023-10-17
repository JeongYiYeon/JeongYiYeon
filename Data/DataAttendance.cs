using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


/////////코드 임의로 수정하지마세요 제네레이터로만 생성합니다.//////////
//////////////////////안되면 Yeon 에게 문의주세요///////////////////////


    public class DataAttendance
    {
        public int UID { get; set; }
        public int ATTENDANCE_GROUP { get; set; }

        [JsonProperty("EnumAttendance.EAttendance")]
        public EnumAttendance.EAttendance EAttendance { get; set; }
        public int ATTENDANCE_DAY { get; set; }
        public int REWARD_ITEM { get; set; }
        public int REWARD_ITEM_COUNT { get; set; }
    }


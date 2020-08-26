/**************************************************
*文件名：PollId
*描   述：
*创建者：lrh
*时间：2018-5-22 11:30:30
*
****************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GNL.Common.Protocol.Domain
{
    public class PollId
    {
        public const string PM1_0 = "a22000";
        public const string PM2_5 = "a22001";
        public const string PM5 = "a22002";
        public const string PM10 = "a22003";
        public const string TSP = "a22004";
        public const string WindSpeed = "a22010";
        public const string WindAngle = "a22011";
        public const string AtomsPressure = "a22012";
        public const string Temp = "a22013";
        public const string Humi = "a22014";
        public const string Anion = "a22015";
        public const string O2 = "a27000";
        public const string SO2 = "a27001";
        public const string CO = "a27002";
        public const string O3 = "a27003";
        public const string NO2 = "a27004";
        public const string TVOC = "a27005";
        public const string SO2_ = "a28000";
        public const string CO_ = "a28001";
        public const string NO_ = "a28002";
        public const string NO2_ = "a28003";
        public const string CH4_ = "a28004";
        public const string H2_ = "a28005";

        public static string[] PollIdList
        {
            get
            {
                var type = typeof(PollId);
                var pros = type.GetFields(System.Reflection.BindingFlags.Static);
                return pros.Select(o => o.GetValue(null).ToString()).ToArray();
            }
        }
    }
}
